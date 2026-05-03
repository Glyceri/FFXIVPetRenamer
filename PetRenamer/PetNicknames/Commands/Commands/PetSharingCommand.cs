using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetSharingCommand : Command
{
    public PetSharingCommand(DalamudServices dalamudServices, IWindowHandler windowHandler) 
        : base(dalamudServices, windowHandler) { }

    public override string CommandCode  
        => "/petsharing";
    
    public override string Description  
        => Translator.GetLine("Command.PetSharing");
    
    public override bool ShowInHelp 
        => true;

    public override void OnCommand(string command, string args) 
        => WindowHandler.Open<PetListWindow>();
}
