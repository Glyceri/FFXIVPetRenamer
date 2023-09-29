using Dalamud.ContextMenu;
using PetRenamer.Core.ContextMenu.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Hooking.Hooks;
using PetRenamer.Core.PettableUserSystem.Pet;

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
            args.AddCustomItem(new GameObjectContextMenuItem("Give Nickname", (a) => PluginLink.WindowHandler.GetWindow<NewPetRenameWindow>()?.OpenForId((int)c.Model.Value!.RowId, true)));
            break;
        }
    }

    void HandleNonNotebook(GameObjectContextMenuOpenArgs args)
    {
        nint address = PluginHandlers.TargetManager.Target?.Address ?? nint.Zero;
        PettableUser targetUser = PluginLink.PettableUserHandler.GetUser(address);
        if (targetUser == null) return;
        if (!targetUser.LocalUser) return;

        PetBase pet = PluginLink.PettableUserHandler.GetPet(targetUser, address);
        if (pet == null) return;

        args.AddCustomItem(new GameObjectContextMenuItem("Give Nickname", (a) =>
        {
            NewPetRenameWindow petWindow = PluginLink.WindowHandler.GetWindow<NewPetRenameWindow>();
            if (pet.ID > -1) petWindow.OpenForId(pet.ID, true);
            else if (pet.ID < -1) petWindow.OpenForBattleID(pet.ID, true);
        }
        ));
    }
}
