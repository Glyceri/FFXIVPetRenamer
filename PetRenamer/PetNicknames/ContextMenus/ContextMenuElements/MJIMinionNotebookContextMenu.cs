using PetRenamer.PetNicknames.ContextMenus.ContextMenuElements.Abstract;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class MJIMinionNotebookContextMenu : PetActionContextMenu
{
    public MJIMinionNotebookContextMenu(IPetServices petServices, IWindowHandler windowHandler)
        : base(petServices, windowHandler) { }

    public override string AddonName
        => "MJIMinionNoteBook";
}
