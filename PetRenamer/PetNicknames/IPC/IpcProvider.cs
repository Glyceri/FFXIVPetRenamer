﻿using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;

namespace PetRenamer;

internal class IpcProvider : IIpcProvider
{
    const string ApiNamespace = "PetRenamer.";
    const uint MajorVersion = 3;
    const uint MinorVersion = 0;

    bool ready = false;
    string lastData = string.Empty;

    readonly IPetServices PetServices;
    readonly IDalamudPluginInterface PetNicknamesPlugin;
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
    readonly ICallGateProvider<IPlayerCharacter, string, object> SetPlayerData;
    readonly ICallGateProvider<IPlayerCharacter, object> ClearPlayerIPCData;

    // Data Sharing
    readonly Dictionary<uint, string> PetNicknameDict = new Dictionary<uint, string>();


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
     *      - SetPlayerData <IPlayerCharacter, string>:
     *          If you have an IPlayerCharacter object and their respective data in the form of a string. Calling this action will overwrite their data with the new data.
     *          (You can never set the data of the current active local player.)
     *          
     *      - ClearPlayerIPCData <IPlayerCharacter>:
     *          Call this action to clear the IPC data of the given IPlayerCharacter.
     *          
     * ----------------------END READ ME -----------------------
     */

    public IpcProvider(in IPetServices petServices, in IDalamudPluginInterface petNicknamesPlugin, in IDataParser dataReader, in IDataWriter dataWriter)
    {
        PetServices = petServices;
        PetNicknamesPlugin = petNicknamesPlugin;
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
        SetPlayerData           = petNicknamesPlugin.GetIpcProvider<IPlayerCharacter, string, object>       ($"{ApiNamespace}SetPlayerData");
        ClearPlayerIPCData      = petNicknamesPlugin.GetIpcProvider<IPlayerCharacter, object>               ($"{ApiNamespace}ClearPlayerData");

        // Data sharing
        PetNicknameDict         = petNicknamesPlugin.GetOrCreateData($"{ApiNamespace}GameObjectRenameDict", () => new Dictionary<uint, string>());
    }

    public void Prepare()
    {
        if (ready) return;

        RegsterActions();
        RegisterFunctions();

        ready = true;
        NotifyReady();
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
    public void SetPlayerDataDetour(IPlayerCharacter character, string data)
    {
            PetServices.PetLog.Log("SET ALL DATA!" + data);
            IDataParseResult result = DataReader.ParseData(data);
            DataReader.ApplyParseData(character, result, true);
    }

    public unsafe void ClearIPCDataDetour(IPlayerCharacter character)
    {
        PetServices.PetLog.Log("Clear!");
        BattleChara* battleChara = (BattleChara*)character.Address;
        if (battleChara == null) return;
        DataReader.ApplyParseData(character, new ClearParseResult(battleChara->ContentId), true);
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
        PetNicknameDict.Clear();
        PetNicknamesPlugin.RelinquishData($"{ApiNamespace}GameObjectRenameDict");

        // Actions
        SetPlayerData.UnregisterAction();
        ClearPlayerIPCData?.UnregisterAction();

        // Functions
        ApiVersion?.UnregisterFunc();
        Enabled?.UnregisterFunc();
        GetPlayerData?.UnregisterFunc();
    }
}
