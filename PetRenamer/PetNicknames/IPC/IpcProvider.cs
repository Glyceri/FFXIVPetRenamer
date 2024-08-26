using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System;

namespace PetRenamer;

internal class IpcProvider : IIpcProvider
{
    const string ApiNamespace = "PetRenamer.";
    const uint MajorVersion = 3;
    const uint MinorVersion = 1;

    bool ready = false;
    string lastData = "[unprepared]";

    readonly DalamudServices DalamudServices;
    readonly IDataWriter DataWriter;
    readonly IDataParser DataReader;


    // Notifications
    readonly ICallGateProvider<object> Ready;
    readonly ICallGateProvider<object> Disposing;
    readonly ICallGateProvider<string, object> PlayerDataChanged;

    // Functions
    readonly ICallGateProvider<bool> Enabled;
    readonly ICallGateProvider<(uint, uint)>? ApiVersion;
    readonly ICallGateProvider<string> GetPlayerData;

    // Actions
    readonly ICallGateProvider<string, object> SetPlayerData;
    readonly ICallGateProvider<ushort, object> ClearPlayerIPCData;


    /* ------------------------ READ ME ------------------------
     * 
     * {ApiNamespace} = "PetRenamer."
     * 
     * Pet Nicknames data is tied to the PLAYER not to a pet. 
     * (I don't even want to list the billion reasons as to why that is the ONLY way to make this plugin work.)
     * 
     * Notifications:
     *      - Ready:
     *          This triggers when the plugin enables. When subscribed, receiving this message means the plugin is active.
     *          
     *      - Disposing:
     *          This triggers when the plugin disables. When subscribed, receiving this message means the plugin is inactive.
     *          
     *      - PlayerDataChanged (string):
     *          This triggers when the local player data has changed. When subscribed, you receive a string with all the data of this player.
     *
     * Functions:
     *      - Enabled <bool>:
     *          Call this function to see if the plugin is enabled. If it errors out or you receive a false value it means the plugins IPC is not ready.
     *          
     *      - ApiVersion <(uint, uint)>:
     *          Call this function to receive back the current IPC API version. (<uint> Majour Version, <uint> Minor Version).
     *          For this release it should be (3, 0).
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
     *      - ClearPlayerIPCData <ushort>:
     *          Call this action to clear the IPC data of the given ObjectIndex.
     *          
     * ----------------------END READ ME -----------------------
     */

    public IpcProvider(DalamudServices dalamudServices, IDalamudPluginInterface petNicknamesPlugin, IDataParser dataReader, IDataWriter dataWriter)
    {
        DalamudServices = dalamudServices;
        DataReader = dataReader;
        DataWriter = dataWriter;

        // Notifiers
        Ready                   = petNicknamesPlugin.GetIpcProvider<object>                                 ($"{ApiNamespace}Ready");
        Disposing               = petNicknamesPlugin.GetIpcProvider<object>                                 ($"{ApiNamespace}Disposing");
        PlayerDataChanged       = petNicknamesPlugin.GetIpcProvider<string, object>                         ($"{ApiNamespace}PlayerDataChanged");

        // Functions
        ApiVersion              = petNicknamesPlugin.GetIpcProvider<(uint, uint)>                           ($"{ApiNamespace}ApiVersion");
        Enabled                 = petNicknamesPlugin.GetIpcProvider<bool>                                   ($"{ApiNamespace}Enabled");
        GetPlayerData           = petNicknamesPlugin.GetIpcProvider<string>                                 ($"{ApiNamespace}GetPlayerData");

        // Actions
        SetPlayerData           = petNicknamesPlugin.GetIpcProvider<string, object>                         ($"{ApiNamespace}SetPlayerData");
        ClearPlayerIPCData      = petNicknamesPlugin.GetIpcProvider<ushort, object>                         ($"{ApiNamespace}ClearPlayerData");
    }

    public void Prepare()
    {
        if (ready) return;

        RegsterActions();
        RegisterFunctions();

        ready = true;
        NotifyReady();

        NotifyDataChanged();
    }

    void RegsterActions()
    {
        SetPlayerData!.RegisterAction(SetPlayerDataDetour);
        ClearPlayerIPCData!.RegisterAction(ClearIPCDataDetour);
    }

    void RegisterFunctions()
    {
        ApiVersion!.RegisterFunc(VersionDetour);
        Enabled!.RegisterFunc(EnabledDetour);
        GetPlayerData!.RegisterFunc(GetPlayerDataDetour);
    }

    // Actions
    public void SetPlayerDataDetour(string data)
    {
        try
        {
            DalamudServices.Framework.Run(() =>
            {
                IDataParseResult result = DataReader.ParseData(data);
                DataReader.ApplyParseData(result, true);
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
            DalamudServices.Framework.Run(() =>
            {
                if (DalamudServices.ObjectTable.Length <= objectIndex) return;
                if (DalamudServices.ObjectTable[objectIndex] is not IPlayerCharacter pc) return;

                DataReader.ApplyParseData(new ClearParseResult(pc.Name.TextValue, (ushort)pc.HomeWorld.Id), true);
            });
        }
        catch(Exception e)
        {
            DalamudServices.PluginLog.Error(e, "Error in clear IPC");
        }
    }

    // Functions
    public (uint, uint) VersionDetour() => (MajorVersion, MinorVersion);
    public bool EnabledDetour() => ready;
    public string GetPlayerDataDetour() => lastData;

    // Notifications
    void NotifyReady()
    {
        try
        {
            Ready?.SendMessage();
        }
        catch { }
    }

    void NotifyDisposing()
    {
        try
        {
            Disposing?.SendMessage();
        }
        catch { }
    }

    void OnDataChanged()
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
        lastData = DataWriter.WriteData();
        OnDataChanged();
    }

    public void ClearCachedData()
    {
        lastData = "";
    }

    public void Dispose()
    {
        NotifyDisposing();

        // Actions
        SetPlayerData.UnregisterAction();
        ClearPlayerIPCData?.UnregisterAction();

        // Functions
        ApiVersion?.UnregisterFunc();
        Enabled?.UnregisterFunc();
        GetPlayerData?.UnregisterFunc();
    }
}
