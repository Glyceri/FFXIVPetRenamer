using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text;
using PetRenamer.Core.ContextMenu.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Hooks;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Core.ContextMenu.ContextMenuElements;

//[ContextMenu]
internal unsafe class PetContextMenu : ContextMenuElement
{
    internal override void OnOpenMenu(IMenuOpenedArgs args)
    {
        if (args.AddonName != null && args.AddonName != "MinionNoteBook") return;
        if (PluginHandlers.ClientState.LocalPlayer == null) return;
        if (args.AgentPtr == 0xE000000) return;
        if (args.AddonName != "MinionNoteBook") HandleNonNotebook(args);
        else HandleNotebook(args);
    }

    void HandleNotebook(IMenuOpenedArgs args)
    {
        if (!PluginLink.Configuration.useContextMenuOnMinions) return;
        string petname = TooltipHook.latestOutcome;
        foreach (PNCompanion c in SheetUtils.instance.petSheet)
        {
            if (c.Singular == string.Empty) continue;
            if (petname.ToLower().Normalize() == string.Empty) continue;
            if (c.Singular.ToLower().Normalize().Trim() != petname.ToLower().Normalize().Trim()) continue;
            args.AddMenuItem(new MenuItem()
            {
                Name = "Rename",
                Prefix = SeIconChar.BoxedLetterP,
                PrefixColor = 0,
                OnClicked = (a) => PluginLink.WindowHandler.GetWindow<PetRenameWindow>()?.OpenForId((int)c.Model, true)
            }); 
            break;
        }
    }

    void HandleNonNotebook(IMenuOpenedArgs args)
    {
        nint address = PluginHandlers.TargetManager.Target?.Address ?? nint.Zero;
        PettableUser targetUser = PluginLink.PettableUserHandler.GetUser(address);
        if (targetUser == null) return;
        if (!targetUser.LocalUser) return;

        PetBase pet = PluginLink.PettableUserHandler.GetPet(targetUser, address);
        if (pet == null) return;
        if (pet.ID > -1 && !PluginLink.Configuration.useContextMenuOnMinions) return;
        if (pet.ID < -1 && !PluginLink.Configuration.useContextMenuOnBattlePets) return;

        args.AddMenuItem(new MenuItem()
        {
            Name = "Rename",
            Prefix = SeIconChar.BoxedLetterP,
            PrefixColor = 0,
            OnClicked = (a) => PluginLink.WindowHandler.GetWindow<PetRenameWindow>()?.OpenForId(pet.ID, true)
        });
    }
}
