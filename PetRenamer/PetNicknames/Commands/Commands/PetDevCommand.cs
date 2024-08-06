using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetDevCommand : Command
{
    readonly Configuration Configuration;

    public PetDevCommand(in DalamudServices dalamudServices, in Configuration configuration, in IWindowHandler windowHandler) : base(dalamudServices, windowHandler) 
    { 
        Configuration = configuration;
    }

    public override string CommandCode { get; } = "/petdev";
    public override string Description { get; } = "Opens the Pet Dev Window";
    public override bool ShowInHelp { get; } = false;

    public override void OnCommand(string command, string args)
    {
        if (Configuration.debugModeActive)
        {
            WindowHandler.Open<PetDevWindow>();
        }
    }
}
