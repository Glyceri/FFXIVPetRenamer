using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class PettableUserHandler : IUpdatable
{
    private readonly IPetServices PetServices;

    public PettableUserHandler(IPetServices petServices)
    {
        PetServices = petServices;
    }

    public bool Enabled 
        => true;
    
    public void OnUpdate(IFramework framework)
    {
        foreach (IPettableUser? user in PetServices.UserList)
        {
            user?.Update();
        }
    }
}
