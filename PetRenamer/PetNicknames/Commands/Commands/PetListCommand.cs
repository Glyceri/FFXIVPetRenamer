using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetListCommand : Command
{
    public PetListCommand(DalamudServices dalamudServices, IWindowHandler windowHandler) 
        : base(dalamudServices, windowHandler) { }

    protected override string CommandCode 
        => "/petlist";
    
    protected override string[] Aliases
        => ["/plist"];
    
    protected override string Description 
        => Translator.GetLine("Command.Petlist");
    
    protected override bool ShowInHelp
        => true;

    protected override void OnCommand(string command, string args) 
        => WindowHandler.Open<PetListWindow>();
}
