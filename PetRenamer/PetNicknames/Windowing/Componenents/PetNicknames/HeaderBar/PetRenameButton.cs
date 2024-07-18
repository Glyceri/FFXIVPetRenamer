using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;
using PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class PetRenameButton : WindowOpenerButton<PetRenameWindow>
{
    public PetRenameButton(in Configuration configuration, in WindowHandler windowHandler) : base(configuration, windowHandler)
    {
        NodeValue = FontAwesomeIcon.PenSquare.ToIconString();
        Tooltip = "Rename Pet";
    }
}
