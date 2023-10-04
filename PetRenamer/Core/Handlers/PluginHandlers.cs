using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace PetRenamer.Core.Handlers;

internal class PluginHandlers
{
    [PluginService] internal static DalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; set; } = null!;
    [PluginService] internal static IFramework Framework { get; set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IGameGui GameGui { get; private set; } = null!;
    [PluginService] internal static ITargetManager TargetManager { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IFlyTextGui FlyTextGui { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; set; } = null!;
    [PluginService] internal static IPluginLog PluginLog { get; set; } = null!;
    [PluginService] internal static IGameInteropProvider Hooking { get; set; } = null!;
    [PluginService] internal static IAddonLifecycle AddonLifecycle { get; set; } = null!;   

    internal static void Start(ref DalamudPluginInterface plugin) => plugin.Create<PluginHandlers>(); 
}
