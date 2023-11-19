using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.Ipc.FindAnythingIPCHelper;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class LogInOutHook : HookableElement
{ 
    internal override void OnInit()
    {
        PluginHandlers.ClientState.Login += OnLogin;
        PluginHandlers.ClientState.Logout += OnLogout;
        PluginHandlers.ClientState.EnterPvP += OnPVPEnter;
    }

    internal override void OnDispose()
    {
        PluginHandlers.ClientState.Login -= OnLogin;
        PluginHandlers.ClientState.Logout -= OnLogout; 
        PluginHandlers.ClientState.EnterPvP -= OnPVPEnter;
    }

    void OnPVPEnter() => PluginHandlers.ChatGui.PrintError("Pet Nicknames is disabled in PVP zones.");
    void OnLogin() => ResetWindows();

    void OnLogout()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        ResetWindows();
    }

    void ResetWindows()
    {
        PluginLink.WindowHandler.GetWindow<PetRenameWindow>().Reset();
        FindAnythingIPCProvider.Deregister();
    }
}
