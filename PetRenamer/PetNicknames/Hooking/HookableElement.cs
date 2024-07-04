using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking;

internal abstract class HookableElement : IHookableElement
{
    public DalamudServices DalamudServices { get; private set; }
    public IPettableUserList UserList { get; private set; }
    public IPetServices PetServices { get; private set; }


    public HookableElement(DalamudServices services, IPettableUserList userList, IPetServices petServices)
    {
        DalamudServices = services;
        UserList = userList;
        PetServices = petServices;
        DalamudServices.Hooking.InitializeFromAttributes(this);
    }

    public abstract void Init();
    public abstract void Dispose();
}
