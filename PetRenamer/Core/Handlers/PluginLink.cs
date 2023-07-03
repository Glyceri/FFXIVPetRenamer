using Dalamud.Plugin;
using PetRenamer.Commands;
using PetRenamer.Utilization;
using PetRenamer.Windows.Handler;

namespace PetRenamer.Core.Handlers
{
    internal class PluginLink
    {
        internal static Configuration Configuration { get; set; } = null!;
        internal static Utils Utils { get; set; } = null!;
        internal static PetRenamerPlugin PetRenamerPlugin { get; set; } = null!;
        internal static WindowsHandler WindowHandler { get; set; } = null!;
        internal static DalamudPluginInterface DalamudPlugin { get; set; } = null!;
        internal static CommandHandler CommandHandler { get; set; } = null!;

        internal static void Start(DalamudPluginInterface dalamud, PetRenamerPlugin petPlugin)
        {
            DalamudPlugin = dalamud;
            PetRenamerPlugin = petPlugin;
            Utils = new Utils();
            Configuration = PluginHandlers.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize();
            WindowHandler = new WindowsHandler();
            CommandHandler = new CommandHandler();
        }
    }
}
