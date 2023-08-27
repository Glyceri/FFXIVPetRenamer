using Dalamud.ContextMenu;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.ContextMenu.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Updatable.Updatables;
using PetRenamer.Windows;
using PetRenamer.Windows.PetWindows;
using TargetObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using DBGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;
using PetRenamer.Core.PettableUserSystem;

namespace PetRenamer.Core.ContextMenu.ContextMenuElements;

[ContextMenu]
internal unsafe class PetContextMenu : ContextMenuElement
{
    internal override void OnOpenMenu(GameObjectContextMenuOpenArgs args)
    {
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
