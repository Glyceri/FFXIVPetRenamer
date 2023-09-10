using Dalamud.ContextMenu;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.ContextMenu.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows;
using PetRenamer.Windows.PetWindows;
using TargetObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using DBGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;
using PetRenamer.Core.PettableUserSystem;
using Dalamud.Plugin;
using Dalamud.Logging;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.ContextMenu.ContextMenuElements;

[ContextMenu]
internal unsafe class PetContextMenu : ContextMenuElement
{
    internal override void OnOpenMenu(GameObjectContextMenuOpenArgs args)
    {
        if (args.ParentAddonName != null) return;
        if (PluginHandlers.ClientState.LocalPlayer == null) return;
        if (!PluginLink.Configuration.useContextMenus) return;
        if (args.ObjectId == 0xE000000) return;
        DBGameObject target = PluginHandlers.TargetManager.Target!;
        if (PluginHandlers.TargetManager.SoftTarget != null)
            target = PluginHandlers.TargetManager.SoftTarget;
        if (target == null) return;
        TargetObjectKind targetObjectKind = target.ObjectKind;
        if (targetObjectKind != TargetObjectKind.BattleNpc && targetObjectKind != TargetObjectKind.Companion) return;
        uint ownerID = target.OwnerId;
        if(targetObjectKind == TargetObjectKind.Companion)
            ownerID = GameObjectManager.GetGameObjectByIndex(target.ObjectIndex)->GetObjectID().ObjectID;
        if (ownerID != PluginHandlers.ClientState.LocalPlayer.ObjectId) return;

        string name = args.Text?.ToString() ?? string.Empty;
        // This is commented out, because so far I havent seen a reason that it needs to be active, but this is and end all fix (its just costly)
        // If there are still exceptions out there, or its laggy, or you know... w/e I'll turn it on
        // if (!SheetUtils.instance.PetExistsInANY(name)) return;

        args.AddCustomItem(new GameObjectContextMenuItem("Give Nickname", (a) => 
        {
            if(targetObjectKind == TargetObjectKind.Companion) PetWindow.SetPetMode(PetMode.Normal);
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
