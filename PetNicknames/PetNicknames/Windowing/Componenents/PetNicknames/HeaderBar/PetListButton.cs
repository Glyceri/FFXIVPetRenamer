using Dalamud.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class PetListButton : WindowOpenerButton<PetListWindow>
{
    public PetListButton(in Configuration configuration, in WindowHandler windowHandler) : base(configuration, windowHandler)
    {
        NodeValue = FontAwesomeIcon.ListUl.ToIconString();
        Tooltip = Translator.GetLine("PetList.Title");
    }
}
