using Dalamud.ContextMenu;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Commands;
using PetRenamer.Core.Chat;
using PetRenamer.Core.ContextMenu;
using PetRenamer.Core.Hooking;
using PetRenamer.Core.Legacy;
using PetRenamer.Core.Networking;
using PetRenamer.Core.PettableUserSystem;
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
    internal static ContextMenuHandler ContextMenuHandler { get; set; } = null!;
    internal static DalamudContextMenu DalamudContextMenu { get; private set; } = null!;
    internal static ChatHandler ChatHandler { get; private set; } = null!;
    internal static PettableUserHandler PettableUserHandler { get; private set; } = null!;
    internal static NetworkingHandler NetworkingHandler { get; private set; } = null!;
    unsafe internal static CharacterManager* CharacterManager => FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager.Instance();

    internal static void Start(ref DalamudPluginInterface dalamud, ref PetRenamerPlugin petPlugin)
    {
        DalamudPlugin = dalamud;
        PetRenamerPlugin = petPlugin;
        IpcStorage = new IpcStorage();
        Utils = new UtilsHandler();
        Configuration = PluginHandlers.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize();
        NetworkingHandler = new NetworkingHandler();
        PettableUserHandler = new PettableUserHandler();
        PettableUserHandler.Initialize();
        WindowHandler = new WindowsHandler();
        CommandHandler = new CommandHandler();
        UpdatableHandler = new UpdatableHandler();
        WindowHandler.Initialize();
        LegacyCompatibilityHandler = new LegacyCompatibilityHandler();
        DalamudContextMenu = new DalamudContextMenu();
        ContextMenuHandler = new ContextMenuHandler();
        HookHandler = new HookHandler();
        ChatHandler = new ChatHandler();
        QuitHandler = new QuitHandler();
        IpcStorage?.LateInitialize();
    }
}
