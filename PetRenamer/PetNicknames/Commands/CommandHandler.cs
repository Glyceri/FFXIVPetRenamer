using PetRenamer.PetNicknames.Commands.Commands;
using PetRenamer.PetNicknames.Commands.Interfaces;
using PetRenamer.PetNicknames.KTKWindowing;
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
    private readonly KTKWindowHandler   KTKWindowHandler;

    private readonly List<ICommand> Commands = new List<ICommand>();

    public CommandHandler(DalamudServices dalamudServices, IWindowHandler windowHandler, KTKWindowHandler ktkWindowHandler, IPetServices petServices, IPettableUserList userList)
    {
        DalamudServices  = dalamudServices;
        WindowHandler    = windowHandler;
        PetServices      = petServices;
        UserList         = userList;
        KTKWindowHandler = ktkWindowHandler;

        RegisterCommands();
    }

    private void RegisterCommands()
    {
        RegisterCommand(new PetnameCommand      (DalamudServices, WindowHandler, KTKWindowHandler, PetServices, UserList));
        RegisterCommand(new PetsettingsCommand  (DalamudServices, WindowHandler, KTKWindowHandler));
        RegisterCommand(new PetsharingCommand   (DalamudServices, WindowHandler, KTKWindowHandler));
        RegisterCommand(new PetlistCommand      (DalamudServices, WindowHandler, KTKWindowHandler));
        RegisterCommand(new PetDevCommand       (DalamudServices, WindowHandler, KTKWindowHandler, PetServices.Configuration));
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
