using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetnameCommand : Command
{
    public PetnameCommand(in DalamudServices dalamudServices, in IWindowHandler windowHandler) : base(dalamudServices, windowHandler) { }

    public override string CommandCode { get; } = "/petname";
    public override string Description { get; } = Translator.GetLine("Command.Petname");
    public override bool ShowInHelp { get; } = true;

    public override void OnCommand(string command, string args)
    {
        WindowHandler.Open<PetRenameWindow>();
    }
}
