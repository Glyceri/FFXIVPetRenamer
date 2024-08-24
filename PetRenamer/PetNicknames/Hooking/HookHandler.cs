using PetRenamer.PetNicknames.Hooking.HookElements;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking;
internal class HookHandler : IDisposable
{
    readonly DalamudServices DalamudServices;
    readonly IPetServices PetServices;
    readonly IPettableUserList PettableUserList;
    readonly IPettableDirtyListener DirtyListener;

    public IMapTooltipHook MapTooltipHook { get; private set; } = null!;
    public IActionTooltipHook ActionTooltipHook { get; private set; } = null!;
    public IIslandHook IslandHook { get; private set; } = null!;

    public HookHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList pettableUserList, IPettableDirtyListener dirtyListener)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PettableUserList = pettableUserList;
        DirtyListener = dirtyListener;

        _Register();
    }

    void _Register()
    {
        Register(new ActionMenuHook(DalamudServices, PetServices, PettableUserList, DirtyListener));

        ActionTooltipHook = new ActionTooltipHook(DalamudServices, PetServices, PettableUserList, DirtyListener);
        Register(ActionTooltipHook);

        MapTooltipHook = new MapTooltipHook(DalamudServices, PetServices, PettableUserList, DirtyListener);
        Register(MapTooltipHook);

        IslandHook = new IslandHook(DalamudServices, PettableUserList, PetServices, DirtyListener);
        Register(IslandHook);

        Register(new MapHook(DalamudServices, PetServices, PettableUserList, MapTooltipHook, DirtyListener));
        Register(new NamePlateHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new TargetBarHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new FlyTextHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new PartyHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
    }

    readonly List<IHookableElement> hookableElements = new List<IHookableElement>();

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
