using PetRenamer.PetNicknames.ContextMenus.ContextMenuElements.Abstract;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class MinionNoteBookContextMenu : PetActionContextMenu
{
    public MinionNoteBookContextMenu(IPetServices petServices, IPettableUserList userList, IWindowHandler windowHandler)
        : base(petServices, userList, windowHandler) { }

    public override string? AddonName
        => "MinionNoteBook";
}
