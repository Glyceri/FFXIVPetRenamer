using PetRenamer.PetNicknames.ContextMenus.ContextMenuElements.Abstract;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class QuickPanelContextMenu : PetActionContextMenu
{
    public QuickPanelContextMenu(IPettableUserList userList, IWindowHandler windowHandler, IActionTooltipHook actionTooltipHook)
        :base(userList, windowHandler, actionTooltipHook) { }

    public override string? AddonName
        => "QuickPanel";
}
