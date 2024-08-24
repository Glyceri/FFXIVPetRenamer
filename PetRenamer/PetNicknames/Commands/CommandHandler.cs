using PetRenamer.PetNicknames.Commands.Commands;
using PetRenamer.PetNicknames.Commands.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Commands;

internal class CommandHandler : ICommandHandler
{
    readonly DalamudServices DalamudServices;
    readonly IWindowHandler WindowHandler;
    readonly Configuration Configuration;

    readonly List<ICommand> Commands = new List<ICommand>();

    public CommandHandler(DalamudServices dalamudServices, Configuration configuration, IWindowHandler windowHandler)
    {
        DalamudServices = dalamudServices;
        Configuration = configuration;
        WindowHandler = windowHandler;

        RegisterCommands();
    }

    void RegisterCommands()
    {
        RegisterCommand(new PetnameCommand      (DalamudServices, WindowHandler));
        RegisterCommand(new PetsettingsCommand  (DalamudServices, WindowHandler));
        RegisterCommand(new PetsharingCommand   (DalamudServices, WindowHandler));
        RegisterCommand(new PetlistCommand      (DalamudServices, WindowHandler));
        RegisterCommand(new PetDevCommand       (DalamudServices, Configuration, WindowHandler));
    }

    void RegisterCommand(ICommand command)
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
