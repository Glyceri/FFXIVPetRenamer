using PetRenamer.Core.Ipc.FindAnythingIPCHelper;
using PetRenamer.Core.Networking.NetworkingElements;

namespace PetRenamer.Core.Handlers;

internal static class QuitHandler
{
    internal static void Quit() 
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
        PluginLink.ToolbarAnimator?.Dispose();  

        IpcProvider.NotifyDisposing();
        IpcProvider.DeInit();

        FindAnythingIPCProvider.DeInit();
        HttpRequestQueue.Dispose();
    }
}

