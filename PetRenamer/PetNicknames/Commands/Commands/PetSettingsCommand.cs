using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetSettingsCommand : Command
{
    public PetSettingsCommand(DalamudServices dalamudServices, IWindowHandler windowHandler) 
        : base(dalamudServices, windowHandler) { }

    protected override string CommandCode  
        => "/petsettings";
    
    protected override string[] Aliases
        => ["/psettings", "/petconfig", "/pconfig"];
    
    protected override string Description 
        => Translator.GetLine("Command.PetSettings");
    
    protected override bool ShowInHelp 
        => true;

    protected override void OnCommand(string command, string args) 
        => WindowHandler.Open<PetConfigWindow>();
}
