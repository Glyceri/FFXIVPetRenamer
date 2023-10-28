﻿using PetRenamer.Core.Debug;
using PetRenamer.Core.Ipc.FindAnythingIPCHelper;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;

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

        DebugStorage.Dispose();

        IpcProvider.NotifyDisposing();
        IpcProvider.DeInit();

        FindAnythingIPCProvider.DeInit();

        PenumbraIPCProvider.DeInit();
    }
}

