using Dalamud.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class PetShareButton : WindowOpenerButton<PetListWindow>
{
    public PetShareButton(in Configuration configuration, in WindowHandler windowHandler) : base(configuration, windowHandler)
    {
        Tooltip = Translator.GetLine("PetList.Sharing");
        NodeValue = FontAwesomeIcon.FileExport.ToIconString();
    }
}
