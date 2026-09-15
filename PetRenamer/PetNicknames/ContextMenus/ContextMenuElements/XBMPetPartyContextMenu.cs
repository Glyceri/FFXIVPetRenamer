using PetRenamer.PetNicknames.ContextMenus.ContextMenuElements.Abstract;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class XBMPetPartyContextMenu : PetActionContextMenu
{
    public XBMPetPartyContextMenu(IPetServices petServices, IWindowHandler windowHandler)
        : base(petServices, windowHandler) { }

    public override string AddonName
        => "XBMPetParty";
}