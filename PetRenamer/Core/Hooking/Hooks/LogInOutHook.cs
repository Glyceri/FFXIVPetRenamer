using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.Ipc.FindAnythingIPCHelper;
using PetRenamer.Core.Updatable.Updatables;
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

    void OnPVPEnter()
    {
        if (PluginLink.Configuration.disablePVPChatMessage) return;
        PluginHandlers.ChatGui.PrintError("Pet Nicknames is disabled in PVP zones excluding the Wolves'Den Pier.");

    }
    void OnLogin() => SoftResetPlugin();

    void OnLogout()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        SoftResetPlugin();
    }

    void SoftResetPlugin()
    {
        PluginLink.PettableUserHandler.SetLocalUser(null!);
        LocalUserSafetyUpdatable.instance.Reset();
        PluginLink.WindowHandler.GetWindow<PetRenameWindow>().Reset();
        FindAnythingIPCProvider.Deregister();
    }
}
