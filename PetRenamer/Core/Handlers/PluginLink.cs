using Dalamud.Plugin;
using PetRenamer.Commands;
using PetRenamer.Core.Hooking;
using PetRenamer.Core.Legacy;
using PetRenamer.Core.Updatable;
using PetRenamer.Utilization;
using PetRenamer.Windows.Handler;

namespace PetRenamer.Core.Handlers;

internal class PluginLink
{
    internal static Configuration Configuration { get; set; } = null!;
    internal static UtilsHandler Utils { get; set; } = null!;
    internal static PetRenamerPlugin PetRenamerPlugin { get; set; } = null!;
    internal static WindowsHandler WindowHandler { get; set; } = null!;
    internal static DalamudPluginInterface DalamudPlugin { get; set; } = null!;
    internal static CommandHandler CommandHandler { get; set; } = null!;
    internal static UpdatableHandler UpdatableHandler { get; set; } = null!;
    internal static LegacyCompatibilityHandler LegacyCompatibilityHandler { get; set; } = null!;
    internal static QuitHandler QuitHandler { get; set; } = null!;
    internal static IpcStorage IpcStorage { get; set; } = null!;
    internal static HookHandler HookHandler { get; set; } = null!;

    internal static void Start(ref DalamudPluginInterface dalamud, ref PetRenamerPlugin petPlugin)
    {
        DalamudPlugin = dalamud;
        PetRenamerPlugin = petPlugin;
        IpcStorage = new IpcStorage();
        Utils = new UtilsHandler();
        Configuration = PluginHandlers.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize();
        WindowHandler = new WindowsHandler();
        CommandHandler = new CommandHandler();
        UpdatableHandler = new UpdatableHandler();
        WindowHandler.Initialize();
        LegacyCompatibilityHandler = new LegacyCompatibilityHandler();
        HookHandler = new HookHandler();
        QuitHandler = new QuitHandler();
    }
}
