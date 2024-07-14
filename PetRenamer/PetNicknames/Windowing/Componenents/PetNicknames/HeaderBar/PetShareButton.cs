using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;
using PetRenamer.PetNicknames.Windowing.Windows.PetShareWindow;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class PetShareButton : WindowOpenerButton<PetSharingWindow>
{
    public PetShareButton(in Configuration configuration, in WindowHandler windowHandler) : base(configuration, windowHandler)
    {
        Tooltip = "Sharing";
        NodeValue = FontAwesomeIcon.AddressBook.ToIconString();
    }
}
