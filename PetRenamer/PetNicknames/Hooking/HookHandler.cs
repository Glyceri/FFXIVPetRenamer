using PetRenamer.PetNicknames.Hooking.HookElements;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking;
internal class HookHandler : IDisposable
{
    IPetLog PetLog { get; init; }
    DalamudServices DalamudServices { get; init; }
    IPetServices PetServices { get; init; }
    IPettableUserList PettableUserList { get; init; }
    IPettableDatabase PettableDatabase { get; init; }

    public HookHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList pettableUserList, IPettableDatabase pettableDatabase)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PettableUserList = pettableUserList;
        PettableDatabase = pettableDatabase;
        PetLog = PetServices.PetLog;

        _Register();
    }

    void _Register()
    {
        Register(new ActionMenuHook(DalamudServices, PetServices, PettableUserList));
        Register(new NamePlateHook(DalamudServices, PetServices, PettableUserList));
        Register(new TargetBarHook(DalamudServices, PetServices, PettableUserList));
        Register(new FlyTextHook(DalamudServices, PetServices, PettableUserList));
    }

    List<IHookableElement> hookableElements = new List<IHookableElement>();

    void Register(IHookableElement element)
    {
        hookableElements.Add(element);
        element?.Init();
    }

    public void Dispose()
    {
        foreach(IHookableElement hookableElement in hookableElements)
            hookableElement.Dispose();
    }
}
