using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;
using PetRenamer.PetNicknames.Windowing.Windows.PetShareWindow;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;

internal class KofiButton : WindowOpenerButton<KofiWindow>
{
    public KofiButton(in Configuration configuration, in WindowHandler windowHandler) : base(configuration, windowHandler)
    {
        NodeValue = FontAwesomeIcon.Coffee.ToIconString();
        Tooltip = "Ko-fi";
    }
}
