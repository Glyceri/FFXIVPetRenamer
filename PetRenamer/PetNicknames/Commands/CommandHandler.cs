using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.Commands;

internal class CommandHandler : IDisposable
{
    readonly DalamudServices DalamudServices;
    readonly IPetServices PetServices;
    readonly IPettableUserList UserList;

    public CommandHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        UserList = userList;

        _RegisterCommands();
    }

    void _RegisterCommands()
    {

    }

    void RegisterCommand()
    {

    }

    public void Dispose()
    {
        
    }
}
