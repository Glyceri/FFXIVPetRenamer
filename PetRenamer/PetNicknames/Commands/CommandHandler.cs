using PetRenamer.PetNicknames.Commands.Commands;
using PetRenamer.PetNicknames.Commands.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Commands;

internal class CommandHandler : ICommandHandler
{
    private readonly DalamudServices    DalamudServices;
    private readonly IWindowHandler     WindowHandler;
    private readonly IPetServices       PetServices;

    private readonly List<ICommand>     Commands = [];

    public CommandHandler(DalamudServices dalamudServices, IPetServices petServices, IWindowHandler windowHandler)
    {
        DalamudServices = dalamudServices;
        WindowHandler   = windowHandler;
        PetServices     = petServices;

        RegisterCommands();
    }

    private void RegisterCommands()
    {
        RegisterCommand(new PetNameCommand      (DalamudServices, WindowHandler, PetServices));
        RegisterCommand(new PetSettingsCommand  (DalamudServices, WindowHandler));
        RegisterCommand(new PetSharingCommand   (DalamudServices, WindowHandler));
        RegisterCommand(new PetListCommand      (DalamudServices, WindowHandler));
        RegisterCommand(new PetDevCommand       (DalamudServices, WindowHandler));
    }

    private void RegisterCommand(ICommand command)
    {
        Commands.Add(command);
    }

    public void Dispose()
    {
        foreach(ICommand command in Commands)
        {
            command?.Dispose();
        }   

        Commands.Clear();
    }
}
