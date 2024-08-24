using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetlistCommand : Command
{
    public PetlistCommand(DalamudServices dalamudServices, IWindowHandler windowHandler) : base(dalamudServices, windowHandler) { }

    public override string CommandCode { get; } = "/petlist";
    public override string Description { get; } = Translator.GetLine("Command.Petlist");
    public override bool ShowInHelp { get; } = true;

    public override void OnCommand(string command, string args)
    {
        WindowHandler.Open<PetListWindow>();
    }
}
