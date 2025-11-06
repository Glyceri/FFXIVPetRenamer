using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.KTKWindowing;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class MJIMinionNotebookContextMenu : MinionNoteBookContextMenu
{
    public override string? AddonName { get; } = "MJIMinionNoteBook";

    public MJIMinionNotebookContextMenu(IPetSheets petSheets, IPettableUserList userList, KTKWindowHandler ktkWindowHandler, IActionTooltipHook actionTooltipHook) 
        : base(petSheets, userList, ktkWindowHandler, actionTooltipHook) { }
}
