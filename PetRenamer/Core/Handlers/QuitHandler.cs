using PetRenamer.Core.Ipc.FindAnythingIPCHelper;

namespace PetRenamer.Core.Handlers;

internal class QuitHandler
{
    public void Quit() 
    {
        PluginLink.WindowHandler?.Dispose();
        PluginLink.CommandHandler?.Dispose();
        PluginLink.UpdatableHandler?.Dispose();
        PluginLink.HookHandler?.Dispose();
        PluginLink.LegacyCompatibilityHandler?.Dispose();
        PluginLink.ContextMenuHandler?.Dispose();
        PluginLink.ChatHandler?.Dispose();
        PluginLink.IpcStorage?.Dispose();
        PluginLink.PettableUserHandler?.Dispose();
        PluginLink.NetworkingHandler?.Dispose();

        IpcProvider.NotifyDisposing();
        IpcProvider.DeInit();

        FindAnythingIPCProvider.DeInit();
    }
}

