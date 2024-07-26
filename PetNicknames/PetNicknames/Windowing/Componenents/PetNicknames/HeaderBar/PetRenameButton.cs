using Dalamud.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class PetRenameButton : WindowOpenerButton<PetRenameWindow>
{
    public PetRenameButton(in WindowHandler windowHandler, in IPetWindow petWindow) : base(windowHandler, petWindow)
    {
        NodeValue = FontAwesomeIcon.PenSquare.ToIconString();
        Tooltip = Translator.GetLine("ContextMenu.Rename");
    }
}
