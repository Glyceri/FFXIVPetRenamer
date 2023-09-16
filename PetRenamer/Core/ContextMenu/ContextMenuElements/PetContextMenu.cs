using Dalamud.ContextMenu;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.ContextMenu.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows;
using PetRenamer.Windows.PetWindows;
using TargetObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using DBGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;
using PetRenamer.Core.PettableUserSystem;
using Dalamud.Logging;
using PetRenamer.Utilization.UtilsModule;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Hooking.Hooks;

namespace PetRenamer.Core.ContextMenu.ContextMenuElements;

[ContextMenu]
internal unsafe class PetContextMenu : ContextMenuElement
{
    internal override void OnOpenMenu(GameObjectContextMenuOpenArgs args)
    {
        if (args.ParentAddonName != null && args.ParentAddonName != "MinionNoteBook") return;
        if (PluginHandlers.ClientState.LocalPlayer == null) return;
        if (args.ObjectId == 0xE000000) return;
        if (args.ParentAddonName != "MinionNoteBook") HandleNonNotebook(args);
        else HandleNotebook(args);
    }

    void HandleNotebook(GameObjectContextMenuOpenArgs args)
    {
        string petname = TooltipHook.latestOutcome;
        foreach (Companion c in SheetUtils.instance.petSheet)
        {
            if (c.Singular.ToString() == string.Empty) continue;
            if (petname.ToLower().Normalize() == string.Empty) continue;
            if (c.Singular.ToString().ToLower().Normalize().Trim() != petname.ToLower().Normalize().Trim()) continue;
            args.AddCustomItem(new GameObjectContextMenuItem("Give Nickname", (a) => PluginLink.WindowHandler.GetWindow<PetRenameWindow>()?.OpenForId(c.Model.Value!.Model, true)));
            break;
        }
    }

    void HandleNonNotebook(GameObjectContextMenuOpenArgs args)
    {
        DBGameObject target = PluginHandlers.TargetManager.Target!;
        if (PluginHandlers.TargetManager.SoftTarget != null)
            target = PluginHandlers.TargetManager.SoftTarget;
        if (target == null) return;
        TargetObjectKind targetObjectKind = target.ObjectKind;
        if (targetObjectKind != TargetObjectKind.BattleNpc && targetObjectKind != TargetObjectKind.Companion) return;
        uint ownerID = target.OwnerId;
        if (targetObjectKind == TargetObjectKind.Companion)
            ownerID = GameObjectManager.GetGameObjectByIndex(target.ObjectIndex)->GetObjectID().ObjectID;
        if (ownerID != PluginHandlers.ClientState.LocalPlayer!.ObjectId) return;

        string name = args.Text?.ToString() ?? string.Empty;
        // This is commented out, because so far I havent seen a reason that it needs to be active, but this is and end all fix (its just costly)
        // If there are still exceptions out there, or its laggy, or you know... w/e I'll turn it on
        // if (!SheetUtils.instance.PetExistsInANY(name)) return;

        if (targetObjectKind == TargetObjectKind.Companion && !PluginLink.Configuration.useContextMenuOnMinions) return;
        if (targetObjectKind == TargetObjectKind.BattleNpc && !PluginLink.Configuration.useContextMenuOnBattlePets) return;

        args.AddCustomItem(new GameObjectContextMenuItem("Give Nickname", (a) =>
        {
            if (targetObjectKind == TargetObjectKind.Companion) PetWindow.SetPetMode(PetMode.Normal);
            else PetWindow.SetPetMode(PetMode.BattlePet);

            PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
            if (user == null) return;
            PetRenameWindow petWindow = PluginLink.WindowHandler.GetWindow<PetRenameWindow>();
            if (targetObjectKind == TargetObjectKind.Companion) petWindow?.OpenForId(user.CompanionID, true);
            else petWindow?.OpenForBattleID(user.BattlePetID, true);
        }
        ));
    }
}
