using Dalamud.Plugin;
using PetRenamer.Core.Ipc.FindAnythingIPCHelper;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using PetRenamer.Windows;

namespace PetRenamer.Core.Handlers;

internal class StartHandler
{
    public void Start(ref DalamudPluginInterface dalamudPluginInterface, PetRenamerPlugin plugin)
    {
        PluginHandlers.Start(ref dalamudPluginInterface);
        PluginLink.Start(ref dalamudPluginInterface, ref plugin);

        IpcProvider.Init(ref dalamudPluginInterface);
        IpcProvider.NotifyReady();

        PenumbraIPCProvider.Init(ref dalamudPluginInterface);
        FindAnythingIPCProvider.Init(ref dalamudPluginInterface);

        PetWindow.petMode = PetMode.Normal;
    }
}
