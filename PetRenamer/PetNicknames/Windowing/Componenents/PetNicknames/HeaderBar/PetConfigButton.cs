using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;
using PetRenamer.PetNicknames.Windowing.Windows.PetConfigWindow;
using PetRenamer.PetNicknames.Windowing;
using PetRenamer;
using PetRenamer.PetNicknames.TranslatorSystem;

internal class PetConfigButton : WindowOpenerButton<PetConfigWindow>
{
    public PetConfigButton(in Configuration configuration, in WindowHandler windowHandler) : base(configuration, windowHandler)
    {
        Tooltip = Translator.GetLine("Config.Title");
        NodeValue = FontAwesomeIcon.Cogs.ToIconString();
    }
}

