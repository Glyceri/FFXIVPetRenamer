using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.KTKWindowing;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetlistCommand : Command
{
    public PetlistCommand(DalamudServices dalamudServices, IWindowHandler windowHandler, KTKWindowHandler ktkWindowHandler) 
        : base(dalamudServices, windowHandler, ktkWindowHandler) { }

    public override string CommandCode 
        => "/petlist";

    public override string Description 
        => Translator.GetLine("Command.Petlist");

    public override bool ShowInHelp  
        => true;

    public override void OnCommand(string command, string args)
    {
        WindowHandler.Open<PetListWindow>();
    }
}
