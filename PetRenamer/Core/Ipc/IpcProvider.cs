using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using PetRenamer.Core;
using PetRenamer.Utilization.UtilsModule;
using System;

namespace PetRenamer;

public static class IpcProvider
{
    const uint MajorVersion = 2;
    const uint MinorVersion = 0;

    static ICallGateProvider<(uint, uint)>? ApiVersion;
    static ICallGateProvider<object>? Ready;
    static ICallGateProvider<object>? Disposing;
    static ICallGateProvider<bool>? Enabled;
    static ICallGateProvider<nint, string>? GetPetNicknameNint;
    static ICallGateProvider<nint, string, object>? SetPetNicknameNint;
    static ICallGateProvider<nint, object>? ClearPetNickname;
    static ICallGateProvider<nint, object>? DoClearPetNickname;
    static ICallGateProvider<nint, string, object>? DoSetPetNickname;

    internal static void Init(ref DalamudPluginInterface dalamudPluginInterface)
    {
        RegisterIPCProfiders(ref dalamudPluginInterface);
        RegisterActions();
        RegisterFunctions();
    }

    static void RegisterIPCProfiders(ref DalamudPluginInterface dalamudPluginInterface)
    {
        // Notifiers
        Ready               = dalamudPluginInterface.GetIpcProvider<object>                 ($"{PluginConstants.apiNamespace}Ready");
        Disposing           = dalamudPluginInterface.GetIpcProvider<object>                 ($"{PluginConstants.apiNamespace}Disposing");
        SetPetNicknameNint  = dalamudPluginInterface.GetIpcProvider<nint, string, object>   ($"{PluginConstants.apiNamespace}OnSetPetNicknameNint");
        ClearPetNickname    = dalamudPluginInterface.GetIpcProvider<nint, object>           ($"{PluginConstants.apiNamespace}OnClearPetNicknameNint");

        // Actions
        DoClearPetNickname  = dalamudPluginInterface.GetIpcProvider<nint, object>           ($"{PluginConstants.apiNamespace}ClearPetNicknameNint");
        DoSetPetNickname    = dalamudPluginInterface.GetIpcProvider<nint, string, object>   ($"{PluginConstants.apiNamespace}SetPetNicknameNint");

        // Functions
        GetPetNicknameNint  = dalamudPluginInterface.GetIpcProvider<nint, string>           ($"{PluginConstants.apiNamespace}GetPetNicknameNint");
        ApiVersion          = dalamudPluginInterface.GetIpcProvider<(uint, uint)>           ($"{PluginConstants.apiNamespace}ApiVersion");
        Enabled             = dalamudPluginInterface.GetIpcProvider<bool>                   ($"{PluginConstants.apiNamespace}Enabled");
    }

    static void RegisterActions()
    {
        DoClearPetNickname!.RegisterAction(OnClearPetNickname);
        DoSetPetNickname!.RegisterAction(OnSetPetNickname);
    }

    static void RegisterFunctions()
    {
        ApiVersion!.RegisterFunc(VersionFunction);
        Enabled!.RegisterFunc(EnabledDetour);
        GetPetNicknameNint!.RegisterFunc(GetPetNicknameFromNintCallback);
    }

    // Notifiers
    public static void NotifyReady() => Ready?.SendMessage();
    public static void NotifyDisposing() => Disposing?.SendMessage();
    public static void NotifySetPetNickname(nint pet, string name) => SetPetNicknameNint?.SendMessage(pet, name);
    public static void NotifyClearPetNickname(nint pet) => ClearPetNickname?.SendMessage(pet);

    // Actions
    static void OnClearPetNickname(nint pet) => throw new NotImplementedException();
    static void OnSetPetNickname(nint pet, string nickname) => throw new NotImplementedException();

    // Functions
    static (uint, uint) VersionFunction() => (MajorVersion, MinorVersion);
    static string GetPetNicknameFromNintCallback(nint pet) => IpcUtils.instance.GetNickname(pet);
    static bool EnabledDetour() => true;


    internal static void DeInit()
    {
        ApiVersion?.UnregisterFunc();
        Enabled?.UnregisterFunc();
        GetPetNicknameNint?.UnregisterFunc();
        DoClearPetNickname?.UnregisterAction();
        DoSetPetNickname?.UnregisterAction();
        Ready = null;
        Disposing = null;
    }
}
