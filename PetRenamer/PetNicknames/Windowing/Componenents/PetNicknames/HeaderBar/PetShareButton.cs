using Dalamud.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class PetShareButton : WindowOpenerButton<PetListWindow>
{
    public PetShareButton(in WindowHandler windowHandler, in IPetWindow petWindow) : base(windowHandler, in petWindow)
    {
        Tooltip = Translator.GetLine("PetList.Sharing");
        NodeValue = FontAwesomeIcon.FileExport.ToIconString();
    }
}
