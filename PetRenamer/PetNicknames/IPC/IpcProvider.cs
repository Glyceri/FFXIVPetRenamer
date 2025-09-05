using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Dalamud.Utility;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System;
using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;

#pragma warning disable IDE0130 // Namespace does not match folder structure (Named like this for easier IPC access)
namespace PetRenamer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal class IpcProvider : IIpcProvider
{
    public bool Enabled { get; set; } = false;

    private const string ApiNamespace       = "PetRenamer.";
    private const uint   MajorVersion       = 4;
    private const uint   MinorVersion       = 0;
    private const float  ReleaseInterval    = 8.0f;    // A minimum of 8 seconds has to pass to release data.

    private string  lastData                = "[unprepared]";
    private float   releaseTimer            = ReleaseInterval;
    private bool    hasDataChange           = false;

    private readonly DalamudServices    DalamudServices;
    private readonly IDataWriter        DataWriter;
    private readonly IDataParser        DataReader;


    // Notifications
    private readonly ICallGateProvider<object>          Ready;
    private readonly ICallGateProvider<object>          Disposing;
    private readonly ICallGateProvider<string, object>  PlayerDataChanged;

    // Functions
    private readonly ICallGateProvider<bool>            EnabledFunction;
    private readonly ICallGateProvider<(uint, uint)>?   ApiVersion;
    private readonly ICallGateProvider<string>          GetPlayerData;

    // Actions
    private readonly ICallGateProvider<string, object>  SetPlayerData;
    private readonly ICallGateProvider<ushort, object>  ClearPlayerIPCData;


    /* ------------------------ READ ME ------------------------
     * 
     * {ApiNamespace} = "PetRenamer."
     * 
     * Pet Nicknames data is tied to the PLAYER not to a pet. 
     * (I don't even want to list the billion reasons as to why that is the ONLY way to make this plugin work.)
     * 
     * Notifications:
     *      - OnReady:
     *          This triggers when the plugin enables. When subscribed, receiving this message means the plugin is active.
     *          
     *      - OnDisposing:
     *          This triggers when the plugin disables. When subscribed, receiving this message means the plugin is inactive.
     *          
     *      - OnPlayerDataChanged (string):
     *          This triggers when the local player data has changed. When subscribed, you receive a string with all the data of this player.
     *
     * Functions:
     *      - IsEnabled <bool>:
     *          Call this function to see if the plugin is enabled. If it errors out or you receive a false value it means the plugins IPC is not ready.
     *          
     *      - ApiVersion <(uint, uint)>:
     *          Call this function to receive back the current IPC API version. (<uint> Majour Version, <uint> Minor Version).
     *          For this release it should be (4, 0).
     *
     *      - GetPlayerData <string>:
     *          Call this function to receive the local players pet data. This data is in the form of a string and will return [string.empty] when no local player is found.
     *          (This is not an expensive function, but please use it sparingly.)
     *          
     * Actions:
     *      - SetPlayerData <string>:
     *          Applies the data to the database
     *          (You can never set the data of the current active local player.)
     *          
     *      - ClearPlayerData <ushort>:
     *          Call this action to clear the IPC data of the given ObjectIndex.
     *          
     * ----------------------END READ ME -----------------------
     */

    public IpcProvider(DalamudServices dalamudServices, IDalamudPluginInterface petNicknamesPlugin, IDataParser dataReader, IDataWriter dataWriter)
    {
        DalamudServices = dalamudServices;
        DataReader      = dataReader;
        DataWriter      = dataWriter;

        // Notifiers
        Ready                   = petNicknamesPlugin.GetIpcProvider<object>                                 ($"{ApiNamespace}OnReady");
        Disposing               = petNicknamesPlugin.GetIpcProvider<object>                                 ($"{ApiNamespace}OnDisposing");
        PlayerDataChanged       = petNicknamesPlugin.GetIpcProvider<string, object>                         ($"{ApiNamespace}OnPlayerDataChanged");

        // Functions
        ApiVersion              = petNicknamesPlugin.GetIpcProvider<(uint, uint)>                           ($"{ApiNamespace}ApiVersion");
        EnabledFunction         = petNicknamesPlugin.GetIpcProvider<bool>                                   ($"{ApiNamespace}IsEnabled");
        GetPlayerData           = petNicknamesPlugin.GetIpcProvider<string>                                 ($"{ApiNamespace}GetPlayerData");

        // Actions
        SetPlayerData           = petNicknamesPlugin.GetIpcProvider<string, object>                         ($"{ApiNamespace}SetPlayerData");
        ClearPlayerIPCData      = petNicknamesPlugin.GetIpcProvider<ushort, object>                         ($"{ApiNamespace}ClearPlayerData");
    }

    public void OnUpdate(IFramework framework)
    {
        // Timer
        float deltaTime = (float)framework.UpdateDelta.TotalSeconds;

        releaseTimer += deltaTime;

        // Clamper
        if (releaseTimer > 1000.0)
        {
            releaseTimer = 1000.0f;
        }

        // Timer Passe Check
        if (releaseTimer < ReleaseInterval)
        {
            return;
        }

        // Has Dirty Data check
        if (!hasDataChange)
        {
            return;
        }

        // Reset
        hasDataChange = false;
        releaseTimer  = 0;

        // Notify IPC
        OnDataChanged();
    }

    public void Prepare()
    {
        if (Enabled)
        {
            return;
        }

        RegsterActions();
        RegisterFunctions();

        Enabled = true;

        NotifyReady();
        NotifyDataChanged();
    }

    private void RegsterActions()
    {
        SetPlayerData!.RegisterAction(SetPlayerDataDetour);
        ClearPlayerIPCData!.RegisterAction(ClearIPCDataDetour);
    }

    private void RegisterFunctions()
    {
        ApiVersion!.RegisterFunc(VersionDetour);
        EnabledFunction!.RegisterFunc(EnabledDetour);
        GetPlayerData!.RegisterFunc(GetPlayerDataDetour);
    }

    // Actions
    public void SetPlayerDataDetour(string data)
    {
        try
        {
            _ = DalamudServices.Framework.Run(() =>
            {
                IDataParseResult result = DataReader.ParseData(data);
                
                _ = DataReader.ApplyParseData(result, ParseSource.IPC);
            });
        }
        catch (Exception e)
        {
            DalamudServices.PluginLog.Error(e, "Error in Set Player Data");
        }
    }

    public unsafe void ClearIPCDataDetour(ushort objectIndex)
    {
        try
        {
            _ = DalamudServices.Framework.Run(() =>
            {
                if (DalamudServices.ObjectTable.Length <= objectIndex)
                {
                    return;
                }

                if (DalamudServices.ObjectTable[objectIndex] is not IPlayerCharacter pc)
                {
                    return;
                }

                _ = DataReader.ApplyParseData(new ClearParseResult(pc.Name.TextValue, (ushort)(pc.HomeWorld.ValueNullable?.RowId ?? 0)), ParseSource.IPC);
            });
        }
        catch(Exception e)
        {
            DalamudServices.PluginLog.Error(e, "Error in clear IPC");
        }
    }

    // Functions
    public (uint, uint) VersionDetour()
    {
        return (MajorVersion, MinorVersion);
    }

    public bool EnabledDetour()
    {
        return Enabled;
    }

    public string GetPlayerDataDetour()
    {
        if (lastData.IsNullOrWhitespace())
        {
            RefreshLastData();
        }

        return lastData;
    }

    // Notifications
    private void NotifyReady()
    {
        try
        {
            Ready?.SendMessage();
        }
        catch (Exception e)
        {
            DalamudServices.PluginLog.Error(e, "An error occurred when notifying ready.");
        }
    }

    private void NotifyDisposing()
    {
        try
        {
            Disposing?.SendMessage();
        }
        catch(Exception e) 
        {
            DalamudServices.PluginLog.Error(e, "An error occurred when disposing.");
        }
    }

    private void OnDataChanged()
    {
        try
        {
            PlayerDataChanged?.SendMessage(lastData);
        }
        catch { }
    }

    // Interface Functions
    public void NotifyDataChanged()
    {
        RefreshLastData();

        hasDataChange = true;
    }

    private void RefreshLastData()
    {
        lock (lastData)
        {
            lastData = DataWriter.WriteData();
        }
    }

    public void ClearCachedData()
    {
        lock (lastData)
        {
            lastData = "";
        }
    }

    public void Dispose()
    {
        Enabled = false;

        NotifyDisposing();

        // Actions
        SetPlayerData.UnregisterAction();
        ClearPlayerIPCData?.UnregisterAction();

        // Functions
        ApiVersion?.UnregisterFunc();
        EnabledFunction?.UnregisterFunc();
        GetPlayerData?.UnregisterFunc();
    }
}
