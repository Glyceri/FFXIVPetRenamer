using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.KTKWindowing;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetsharingCommand : Command
{
    public PetsharingCommand(DalamudServices dalamudServices, IWindowHandler windowHandler, KTKWindowHandler ktkWindowHandler) 
        : base(dalamudServices, windowHandler, ktkWindowHandler) { }

    public override string CommandCode
        => "/petsharing";

    public override string Description
        => Translator.GetLine("Command.PetSharing");

    public override bool ShowInHelp 
        => true;

    public override void OnCommand(string command, string args)
    {
        WindowHandler.Open<PetListWindow>();
    }
}
