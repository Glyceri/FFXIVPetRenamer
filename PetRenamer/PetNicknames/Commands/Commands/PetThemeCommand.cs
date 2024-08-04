using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.ColourEditorWindow;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetThemeCommand : Command
{
    public PetThemeCommand(in DalamudServices dalamudServices, in IWindowHandler windowHandler) : base(dalamudServices, windowHandler) { }

    public override string CommandCode { get; } = "/pettheme";
    public override string Description { get; } = Translator.GetLine("Command.PetTheme");
    public override bool ShowInHelp { get; } = true;

    public override void OnCommand(string command, string args)
    {
        WindowHandler.Open<ColourEditorWindow>();
    }
}
