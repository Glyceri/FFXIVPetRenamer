using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace PetRenamer.Core.Handlers;

internal class PluginHandlers
{
    [PluginService] internal static DalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] internal static CommandManager CommandManager { get; set; } = null!;
    [PluginService] internal static Framework Framework { get; set; } = null!;
    [PluginService] internal static DataManager DataManager { get; set; } = null!;
    [PluginService] internal static SigScanner SigScanner { get; set; } = null!;
    [PluginService] internal static GameObjectManager GameObjectManager { get; set; }

    internal static void Start(DalamudPluginInterface plugin) => plugin.Create<PluginHandlers>(); 
}
