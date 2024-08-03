using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;
using PetRenamer.PetNicknames.Windowing.Windows.PetConfigWindow;
using PetRenamer.PetNicknames.Windowing;
using PetRenamer;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;

internal class PetConfigButton : WindowOpenerButton<PetConfigWindow>
{
    public PetConfigButton(in WindowHandler windowHandler, in IPetWindow petWindow) : base(windowHandler, petWindow)
    {
        Tooltip = Translator.GetLine("Config.Title");
        NodeValue = FontAwesomeIcon.Cogs.ToIconString();
    }
}

