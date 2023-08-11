using Dalamud.ContextMenu;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.ContextMenu.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Updatable.Updatables;
using PetRenamer.Windows;
using PetRenamer.Windows.PetWindows;
using TargetObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;

namespace PetRenamer.Core.ContextMenu.ContextMenuElements;

[ContextMenu]
internal unsafe class PetContextMenu : ContextMenuElement
{
    internal override void OnOpenMenu(GameObjectContextMenuOpenArgs args)
    {
        if (PluginHandlers.ClientState.LocalPlayer == null) return;
        if (!PluginLink.Configuration.useContextMenus) return;
        if (args.ObjectId == 0xE000000) return;
        if (PluginHandlers.TargetManager.Target == null) return;
        TargetObjectKind targetObjectKind = PluginHandlers.TargetManager.Target.ObjectKind;
        if (targetObjectKind != TargetObjectKind.BattleNpc && targetObjectKind != TargetObjectKind.Companion) return;
        uint ownerID = PluginHandlers.TargetManager.Target.OwnerId;
        if(targetObjectKind == TargetObjectKind.Companion)
            ownerID = GameObjectManager.GetGameObjectByIndex(PluginHandlers.TargetManager.Target.ObjectIndex)->GetObjectID().ObjectID;
        if (ownerID != PluginHandlers.ClientState.LocalPlayer.ObjectId) return;

        args.AddCustomItem(new GameObjectContextMenuItem("[PetRenamer] Give Nickname", (a) => 
        {
            if(targetObjectKind == TargetObjectKind.Companion) PetWindow.SetPetMode(PetMode.Normal);
            else PetWindow.SetPetMode(PetMode.BattlePet);

            foreach(FoundPlayerCharacter chara in PluginLink.IpcStorage.characters)
            {
                if (chara.ownName != PluginHandlers.ClientState.LocalPlayer.Name.ToString() || chara.ownHomeWorld != PluginHandlers.ClientState.LocalPlayer.HomeWorld.Id) continue;
                if (targetObjectKind == TargetObjectKind.Companion) PluginLink.WindowHandler.GetWindow<MainWindow>().OpenForId(chara.GetCompanionID());
                else PluginLink.WindowHandler.GetWindow<MainWindow>().OpenForId(chara.GetBattlePetID());
                break;
            }
        }
        ));
    }
}
