﻿using Dalamud.Plugin;
using PetRenamer.Core.Ipc.FindAnythingIPCHelper;
using PetRenamer.Windows;

namespace PetRenamer.Core.Handlers;

internal static class StartHandler
{
    internal static void Start(ref IDalamudPluginInterface dalamudPluginInterface, PetRenamerPlugin plugin)
    {
        PluginHandlers.Start(ref dalamudPluginInterface);

        IpcProvider.EarlyInit();

        PluginLink.Start(ref dalamudPluginInterface, ref plugin);

        IpcProvider.Init(ref dalamudPluginInterface);

        FindAnythingIPCProvider.Init(ref dalamudPluginInterface);

        PetWindow.petMode = PetMode.Normal;
        // For some reason update can call instantly upon subscribing, so we have to start it late.
        // This doesn't happen when you automatically reload a plugin upon loading btw, only when you manually enable...
        PluginLink.UpdatableHandler.Initialize();
        IpcProvider.NotifyReady();
    }
}
