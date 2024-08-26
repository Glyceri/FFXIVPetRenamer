using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class MJIMinionNotebookContextMenu : MinionNoteBookContextMenu
{
    public override string? AddonName { get; } = "MJIMinionNoteBook";

    public MJIMinionNotebookContextMenu(IPetSheets petSheets, IPettableUserList userList, IWindowHandler windowHandler, IActionTooltipHook actionTooltipHook) : base(petSheets, userList, windowHandler, actionTooltipHook) { }
}
