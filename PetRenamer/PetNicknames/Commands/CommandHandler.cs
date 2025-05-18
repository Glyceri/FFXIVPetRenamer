using PetRenamer.PetNicknames.Commands.Commands;
using PetRenamer.PetNicknames.Commands.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
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
    private readonly IPettableUserList  UserList;
    private readonly IPettableDatabase  Database;

    private readonly List<ICommand> Commands = new List<ICommand>();

    public CommandHandler(DalamudServices dalamudServices, IWindowHandler windowHandler, IPetServices petServices, IPettableUserList userList, IPettableDatabase database)
    {
        DalamudServices = dalamudServices;
        WindowHandler   = windowHandler;
        PetServices     = petServices;
        UserList        = userList;
        Database        = database;

        RegisterCommands();
    }

    private void RegisterCommands()
    {
        RegisterCommand(new PetnameCommand      (DalamudServices, WindowHandler, PetServices, UserList, Database));
        RegisterCommand(new PetsettingsCommand  (DalamudServices, WindowHandler));
        RegisterCommand(new PetsharingCommand   (DalamudServices, WindowHandler));
        RegisterCommand(new PetlistCommand      (DalamudServices, WindowHandler));
        RegisterCommand(new PetDevCommand       (DalamudServices, PetServices.Configuration, WindowHandler));
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
