using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using System.Xml.Linq;

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
    public static void NotifyReady()
    {
        try
        {
            Ready?.SendMessage();
        }
        catch { }
    }
    public static void NotifyDisposing()
    {
        try
        {
            Disposing?.SendMessage();
        }
        catch { }
    }
    public static void NotifySetPetNickname(nint pet, string name)
    {
        try
        {
            if (name == string.Empty || name == null!) ClearPetNickname?.SendMessage(pet);
            else SetPetNicknameNint?.SendMessage(pet, name);
        }
        catch { }
    }
    public static void NotifyClearPetNickname(nint pet)
    {
        try
        {
            ClearPetNickname?.SendMessage(pet);
        }
        catch { }
    }

    // Actions
    public static void OnClearPetNickname(nint pet) => PluginLink.IpcStorage.Register((pet, string.Empty));
    public static void OnSetPetNickname(nint pet, string nickname) => PluginLink.IpcStorage.Register((pet, nickname));

    // Functions
    public static(uint, uint) VersionFunction() => (MajorVersion, MinorVersion);
    public static string GetPetNicknameFromNintCallback(nint pet) => IpcUtils.instance.GetNickname(pet);
    public static bool EnabledDetour() => true;


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
