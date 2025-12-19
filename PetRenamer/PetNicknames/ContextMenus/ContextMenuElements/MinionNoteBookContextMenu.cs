using PetRenamer.PetNicknames.ContextMenus.ContextMenuElements.Abstract;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class MinionNoteBookContextMenu : PetActionContextMenu
{
    public MinionNoteBookContextMenu(IPettableUserList userList, IWindowHandler windowHandler, IActionTooltipHook actionTooltipHook)
        : base(userList, windowHandler, actionTooltipHook) { }

    public override string? AddonName
        => "MinionNoteBook";
}
