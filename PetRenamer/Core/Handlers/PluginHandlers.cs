using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace PetRenamer.Core.Handlers;

internal class PluginHandlers
{
    [PluginService] internal static DalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] internal static CommandManager CommandManager { get; set; } = null!;
    [PluginService] internal static Framework Framework { get; set; } = null!;
    [PluginService] internal static DataManager DataManager { get; set; } = null!;
    [PluginService] internal static SigScanner SigScanner { get; set; } = null!;
    [PluginService] internal static ClientState ClientState { get; private set; } = null!;
    [PluginService] internal static GameGui GameGui { get; private set; } = null!;
    [PluginService] internal static TargetManager TargetManager { get; private set; } = null!;

    internal static void Start(ref DalamudPluginInterface plugin) => plugin.Create<PluginHandlers>(); 
}
