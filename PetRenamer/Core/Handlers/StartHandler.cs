using Dalamud.Plugin;
using HtmlAgilityPack;
using PetRenamer.Core.Ipc.FindAnythingIPCHelper;
using PetRenamer.Core.Ipc.MappyIPC;
using PetRenamer.Logging;
using PetRenamer.Windows;
using System.Net.Http;
using System.Threading.Tasks;

namespace PetRenamer.Core.Handlers;

internal static class StartHandler
{
    internal static void Start(ref DalamudPluginInterface dalamudPluginInterface, PetRenamerPlugin plugin)
    {
        PluginHandlers.Start(ref dalamudPluginInterface);
        PluginLink.Start(ref dalamudPluginInterface, ref plugin);

        IpcProvider.Init(ref dalamudPluginInterface);

        FindAnythingIPCProvider.Init(ref dalamudPluginInterface);
        IPCMappy.Init(ref dalamudPluginInterface);

        PetWindow.petMode = PetMode.Normal;
        // For some reason update can call instantly upon subscribing, so we have to start it late.
        // This doesn't happen when you automatically reload a plugin upon loading btw, only when you manually enable...
        PluginLink.UpdatableHandler.ReleaseUpdate();
        IpcProvider.NotifyReady();
    }
}
