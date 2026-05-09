using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetDevCommand : Command
{
    public PetDevCommand(DalamudServices dalamudServices, IWindowHandler windowHandler) 
        : base(dalamudServices, windowHandler) { }

    public override string CommandCode 
        => "/petdev";
    
    public override string Description 
        => "Opens the Pet Dev Window";
    
    public override bool ShowInHelp 
        => false;

    public override void OnCommand(string command, string args)
        => WindowHandler.Open<PetDevWindow>();
}
