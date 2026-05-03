using PetRenamer.PetNicknames.ContextMenus.ContextMenuElements.Abstract;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class QuickPanelContextMenu : PetActionContextMenu
{
    public QuickPanelContextMenu(IPetServices petServices, IPettableUserList userList, IWindowHandler windowHandler)
        : base(petServices, userList, windowHandler) { }

    public override string? AddonName
        => "QuickPanel";
}
