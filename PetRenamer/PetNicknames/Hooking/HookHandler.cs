using PetRenamer.PetNicknames.Hooking.HookElements;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.Interfaces;
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

    public IMapTooltipHook MapTooltipHook { get; private set; } = null!;
    public IActionTooltipHook ActionTooltipHook { get; private set; } = null!;

    public HookHandler(in DalamudServices dalamudServices, in IPetServices petServices, in IPettableUserList pettableUserList)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PettableUserList = pettableUserList;

        _Register();
    }

    void _Register()
    {
        Register(new ActionMenuHook(DalamudServices, PetServices, PettableUserList));

        ActionTooltipHook = new ActionTooltipHook(DalamudServices, PetServices, PettableUserList);
        Register(ActionTooltipHook);

        MapTooltipHook = new MapTooltipHook(DalamudServices, PetServices, PettableUserList);
        Register(MapTooltipHook);

        Register(new MapHook(DalamudServices, PetServices, PettableUserList, MapTooltipHook));
        Register(new NamePlateHook(DalamudServices, PetServices, PettableUserList));
        Register(new TargetBarHook(DalamudServices, PetServices, PettableUserList));
        Register(new FlyTextHook(DalamudServices, PetServices, PettableUserList));
        Register(new PartyHook(DalamudServices, PetServices, PettableUserList));
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
