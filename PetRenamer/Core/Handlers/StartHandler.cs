using Dalamud.Plugin;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.Handlers;

internal class StartHandler
{
    public void Start(ref DalamudPluginInterface dalamudPluginInterface, PetRenamerPlugin plugin)
    {
        PluginHandlers.Start(ref dalamudPluginInterface);
        PluginLink.Start(ref dalamudPluginInterface, ref plugin);

        IpcProvider.Init(ref dalamudPluginInterface);
        IpcProvider.NotifyReady();
    }
}
