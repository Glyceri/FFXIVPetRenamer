using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.PetConfigWindow;
using PetRenamer.PetNicknames.Windowing.Windows.PetShareWindow;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetsettingsCommand : Command
{
    public PetsettingsCommand(in DalamudServices dalamudServices, in IWindowHandler windowHandler) : base(dalamudServices, windowHandler) { }

    public override string CommandCode { get; } = "/petsettings";
    public override string Description { get; } = Translator.GetLine("Command.PetSettings");
    public override bool ShowInHelp { get; } = true;

    public override void OnCommand(string command, string args)
    {
        WindowHandler.Open<PetSharingWindow>();
    }
}
