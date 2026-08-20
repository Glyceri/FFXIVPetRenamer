using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetDevCommand : Command
{
    public PetDevCommand(DalamudServices dalamudServices, IWindowHandler windowHandler) 
        : base(dalamudServices, windowHandler) { }

    protected override string CommandCode 
        => "/petdev";

    protected override string[] Aliases
        => ["/pdev"];

    protected override string Description 
        => "Opens the Pet Dev Window";
    
    protected override bool ShowInHelp 
        => false;

    protected override void OnCommand(string command, string args)
        => WindowHandler.Open<PetDevWindow>();
}
