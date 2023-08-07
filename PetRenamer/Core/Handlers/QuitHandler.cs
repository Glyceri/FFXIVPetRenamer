﻿namespace PetRenamer.Core.Handlers;

internal class QuitHandler
{
    public void Quit() 
    {
        PluginLink.WindowHandler.Dispose();
        PluginLink.CommandHandler.Dispose();
        PluginLink.UpdatableHandler.Dispose();
        PluginLink.HookHandler.Dispose();
        PluginLink.LegacyCompatibilityHandler.Dispose();

        IpcProvider.NotifyDisposing();
        IpcProvider.DeInit();
    }
}

