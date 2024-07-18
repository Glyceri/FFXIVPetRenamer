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

    readonly List<ICommand> Commands = new List<ICommand>();

    public CommandHandler(in DalamudServices dalamudServices, in IWindowHandler windowHandler)
    {
        DalamudServices = dalamudServices;
        WindowHandler = windowHandler;

        _RegisterCommands();
    }

    void _RegisterCommands()
    {
        RegisterCommand(new PetnameCommand(in DalamudServices, in WindowHandler));
        RegisterCommand(new PetsettingsCommand(in DalamudServices, in WindowHandler));
        RegisterCommand(new PetsharingCommand(in DalamudServices, in WindowHandler));
        RegisterCommand(new PetlistCommand(in DalamudServices, in WindowHandler));

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
