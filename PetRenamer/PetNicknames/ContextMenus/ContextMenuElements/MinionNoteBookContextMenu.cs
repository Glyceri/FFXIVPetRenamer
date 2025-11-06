using Dalamud.Game.Gui.ContextMenu;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.KTKWindowing;
using PetRenamer.PetNicknames.KTKWindowing.Addons;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class MinionNoteBookContextMenu : IContextMenuElement
{
    public virtual string? AddonName { get; } = "MinionNoteBook";

    private readonly IPettableUserList  UserList;
    private readonly KTKWindowHandler   KTKWindowHandler;
    private readonly IActionTooltipHook ActionTooltipHook;
    private readonly IPetSheets         PetSheets;

    public MinionNoteBookContextMenu(IPetSheets petSheets, IPettableUserList userList, KTKWindowHandler ktkWindowHandler, IActionTooltipHook actionTooltipHook)
    {
        PetSheets         = petSheets;
        UserList          = userList;
        KTKWindowHandler  = ktkWindowHandler;
        ActionTooltipHook = actionTooltipHook;
    }

    public Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        IPettableUser? localUser = UserList.LocalPlayer;

        if (localUser == null)
        {
            return null;
        }

        IPetSheetData? petData = PetSheets.GetPetFromAction(ActionTooltipHook.LastActionID, localUser, false);

        if (petData == null)
        {
            return null;
        }

        return (a) =>
        {
            KTKWindowHandler.GetAddon<PetRenameAddon>()?.SetPetSkeleton(petData.Model);
            KTKWindowHandler.Open<PetRenameAddon>();
        };
    }
}
