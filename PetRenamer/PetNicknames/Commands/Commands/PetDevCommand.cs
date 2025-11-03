using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.KTKWindowing;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal class PetDevCommand : Command
{
    private readonly Configuration Configuration;

    public PetDevCommand(DalamudServices dalamudServices, IWindowHandler windowHandler, KTKWindowHandler ktkWindowHandler, Configuration configuration) 
        : base(dalamudServices, windowHandler, ktkWindowHandler) 
    { 
        Configuration = configuration;
    }

    public override string CommandCode 
        => "/petdev";

    public override string Description 
        => "Opens the Pet Dev Window";

    public override bool ShowInHelp 
        => false;

    public override void OnCommand(string command, string args)
    {
        if (!Configuration.debugModeActive)
        {
            return;
        }

        WindowHandler.Open<PetDevWindow>();
    }
}
