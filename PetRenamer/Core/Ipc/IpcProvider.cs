using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Logging;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;

namespace PetRenamer;

public static class IpcProvider
{
    static bool ready = false;

    const uint MajorVersion = 2;
    const uint MinorVersion = 0;

    static ICallGateProvider<(uint, uint)>? ApiVersion;
    static ICallGateProvider<object>? Ready;
    static ICallGateProvider<object>? Disposing;
    static ICallGateProvider<bool>? Enabled;
    static ICallGateProvider<nint, string>? GetPetNicknameNint;

    static ICallGateProvider<string>? GetLocalPlayerDataAll;
    static ICallGateProvider<IPlayerCharacter, string, object>? SetPlayerDataAll;
    static ICallGateProvider<string, object>? OnPlayerDataChangedAll;
    static ICallGateProvider<IPlayerCharacter, string, object>? SetPlayerDataSingle;
    static ICallGateProvider<string, object>? OnPlayerDataChangedSingle;
    static ICallGateProvider<IPlayerCharacter, object>? ClearPlayerDataAll;

    internal static Dictionary<uint, string> PetNicknameDict { get; private set; } = new Dictionary<uint, string>();

    // ------------------------ READ ME ------------------------
    // {PluginConstants.apiNamespace} = "PetRenamer."
    // When a player does not exist, Pet Nicknames automatically cleans up ALL ipc data connected to that player.
    // When a player comes into existance on your client, or the plugin has boardcasted Ready, you should call 'GetLocalPlayerDataAll'
    // For syncing with another player send this whole list to 'SetPlayerDataAll'
    // When a player changes any of their own pet nicknames, an event with that single namechange will broadcast on 'OnPlayerDataChangedSingle'
    // For syncing send this data to 'SetPlayerDataSingle'
    // (Yes, you can change nicknames of pets that are not currently visible)
    // (No, names dont only show up on target bars and nameplates. They also show up on castbars, in flyout text and in your battle log. For this reason alone it is impossible to soley sync on nint)
    // Sometimes a player can import a list of all their old nicknames, this has to do with manual sharing.
    // The same rules as single sharing apply here, only the channels 'OnPlayerDataChangedAll' and 'SetPlayerDataAll' are now used respectively.
    // If you want to completely remove all IPC data from a player call 'ClearPlayerDataAll'
    // This will clear all IPC data send to that player and act as if no data got send ever (clearing data is non-recoverable, you will have to completely resend data to 'SetPlayerDataAll')
    // You can NOT! set data for your Local Player. You can only set data for other players, this is by design.

    internal static void EarlyInit()
    {
        RegisterDictionaries();
    }

    internal static void Init(ref IDalamudPluginInterface dalamudPluginInterface)
    {
        RegisterIPCProfiders(ref dalamudPluginInterface);
        RegisterActions();
        RegisterFunctions();
    }

    static void RegisterIPCProfiders(ref IDalamudPluginInterface dalamudPluginInterface)
    {
        // Notifiers
        Ready                       = dalamudPluginInterface.GetIpcProvider<object>                             ($"{PluginConstants.apiNamespace}Ready");
        Disposing                   = dalamudPluginInterface.GetIpcProvider<object>                             ($"{PluginConstants.apiNamespace}Disposing");
        OnPlayerDataChangedSingle   = dalamudPluginInterface.GetIpcProvider<string, object>                     ($"{PluginConstants.apiNamespace}OnPlayerDataChangedSingle");
        OnPlayerDataChangedAll      = dalamudPluginInterface.GetIpcProvider<string, object>                     ($"{PluginConstants.apiNamespace}OnPlayerDataChangedAll");

        // Functions
        GetPetNicknameNint          = dalamudPluginInterface.GetIpcProvider<nint, string>                       ($"{PluginConstants.apiNamespace}GetPetNicknameNint");
        ApiVersion                  = dalamudPluginInterface.GetIpcProvider<(uint, uint)>                       ($"{PluginConstants.apiNamespace}ApiVersion");
        Enabled                     = dalamudPluginInterface.GetIpcProvider<bool>                               ($"{PluginConstants.apiNamespace}Enabled");
        GetLocalPlayerDataAll       = dalamudPluginInterface.GetIpcProvider<string>                             ($"{PluginConstants.apiNamespace}GetLocalPlayerDataAll");

        // Actions
        SetPlayerDataAll            = dalamudPluginInterface.GetIpcProvider<IPlayerCharacter, string, object>    ($"{PluginConstants.apiNamespace}SetPlayerDataAll");
        SetPlayerDataSingle         = dalamudPluginInterface.GetIpcProvider<IPlayerCharacter, string, object>    ($"{PluginConstants.apiNamespace}SetPlayerDataSingle");
        ClearPlayerDataAll          = dalamudPluginInterface.GetIpcProvider<IPlayerCharacter, object>            ($"{PluginConstants.apiNamespace}ClearPlayerDataAll");
    }

    static void RegisterActions()
    {
        SetPlayerDataAll!.RegisterAction (SetPlayerDataAllDetour);
        SetPlayerDataSingle!.RegisterAction (SetPlayerDataSingleDetour);
        ClearPlayerDataAll!.RegisterAction (ClearPlayerDataAllDetour);
    }

    static void RegisterFunctions()
    {
        ApiVersion!.RegisterFunc(VersionDetour);
        Enabled!.RegisterFunc(EnabledDetour);
        GetPetNicknameNint!.RegisterFunc(GetPetNicknameFromNintDetour);
        GetLocalPlayerDataAll!.RegisterFunc(GetLocalPlayerDataAllDetour);
    }

    static void RegisterDictionaries()
    {
        PetNicknameDict = PluginHandlers.PluginInterface.GetOrCreateData($"{PluginConstants.apiNamespace}GameObjectRenameDict", () => new Dictionary<uint, string>());
    }

    // Notifiers
    public static void NotifyReady()
    {
        try
        {
            ready = true;
            Ready?.SendMessage();
        }
        catch(Exception e) { PetLog.Log($"Error on Ready IPC Notify: {e}"); }
    }
    public static void NotifyDisposing()
    {
        try
        {
            Disposing?.SendMessage();
        }
        catch (Exception e) { PetLog.Log($"Error on Disposing IPC Notify: {e}"); }
    }
    public static void NotifyPlayerDataChangedAll(string data)
    {
        try
        {
            OnPlayerDataChangedAll?.SendMessage(data);
        }
        catch (Exception e) { PetLog.Log($"Error on PlayerDataChangedAllDetour IPC Notify: {e}"); }
    }
    public static void NotifyPlayerDataChangedSingle(string data)
    {
        try
        {
            OnPlayerDataChangedSingle?.SendMessage(data);
        }
        catch (Exception e) { PetLog.Log($"Error on PlayerDataChangedSingleDetour IPC Notify: {e}"); }
    }

    // Actions
    public static void SetPlayerDataAllDetour(IPlayerCharacter character, string data) => PluginLink.IpcStorage.Register((character, data));
    public static void SetPlayerDataSingleDetour(IPlayerCharacter character, string data) => PluginLink.IpcStorage.Register((character, data));
    public static void ClearPlayerDataAllDetour(IPlayerCharacter character) => PluginLink.IpcStorage.Register((character, PluginConstants.IpcClear));

    // Functions
    public static(uint, uint) VersionDetour() => (MajorVersion, MinorVersion);
    public static string GetPetNicknameFromNintDetour(nint pet) => IpcUtils.instance.GetNickname(pet);
    public static bool EnabledDetour() => ready;
    public static string GetLocalPlayerDataAllDetour() => IpcUtils.instance.GetAllLocalPlayerData();

    internal static void DeInit()
    {
        PetNicknameDict.Clear();
        PluginLink.DalamudPlugin.RelinquishData($"{PluginConstants.apiNamespace}GameObjectRenameDict");
        ApiVersion?.UnregisterFunc();
        Enabled?.UnregisterFunc();
        GetPetNicknameNint?.UnregisterFunc();
        GetLocalPlayerDataAll?.UnregisterFunc();
        ClearPlayerDataAll?.UnregisterAction();
        SetPlayerDataAll?.UnregisterAction();
        SetPlayerDataSingle?.UnregisterAction();
        Ready = null;
        Disposing = null;
    }
}
