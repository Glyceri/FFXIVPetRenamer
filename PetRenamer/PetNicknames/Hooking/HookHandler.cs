using PetRenamer.PetNicknames.Hooking.HookElements;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking;

internal class HookHandler : IDisposable
{
    private readonly DalamudServices        DalamudServices;
    private readonly IPetServices           PetServices;
    private readonly IPettableUserList      PettableUserList;
    private readonly IPettableDirtyListener DirtyListener;
    private readonly IPettableDatabase      Database;
    private readonly ILegacyDatabase        LegacyDatabase;
    private readonly ISharingDictionary     SharingDictionary;
    private readonly IPettableDirtyCaller   DirtyCaller;
    private readonly ITooltipHookHelper     TooltipHookHelper;

    public IMapTooltipHook    MapTooltipHook    { get; private set; } = null!;
    public IActionTooltipHook ActionTooltipHook { get; private set; } = null!;
    public IIslandHook        IslandHook        { get; private set; } = null!;

    private readonly List<IHookableElement> hookableElements = [];

    public HookHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList pettableUserList, IPettableDirtyListener dirtyListener, IPettableDatabase database, ILegacyDatabase legacyDatabase, ISharingDictionary sharingDictionary, IPettableDirtyCaller dirtyCaller)
    {
        DalamudServices     = dalamudServices;
        PetServices         = petServices;
        PettableUserList    = pettableUserList;
        DirtyListener       = dirtyListener;
        Database            = database;
        LegacyDatabase      = legacyDatabase;
        SharingDictionary   = sharingDictionary;
        DirtyCaller         = dirtyCaller;

        TooltipHookHelper   = new TooltipHookHelper(DalamudServices);

        _Register();
    }

    private void _Register()
    {
        Register(new ActionMenuHook(DalamudServices, PetServices, PettableUserList, DirtyListener));

        ActionTooltipHook = new ActionTooltipHook(DalamudServices, PetServices, PettableUserList, DirtyListener, TooltipHookHelper);
        Register(ActionTooltipHook);

        MapTooltipHook = new MapTooltipHook(DalamudServices, PetServices, PettableUserList, DirtyListener, TooltipHookHelper);
        Register(MapTooltipHook);

        IslandHook = new IslandHook(DalamudServices, PettableUserList, PetServices, DirtyListener);
        Register(IslandHook);

        Register(new MapHook(DalamudServices, PetServices, PettableUserList, MapTooltipHook, DirtyListener));
        Register(new NamePlateHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new TargetBarHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new FlyTextHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new PartyHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new UINavigationHook(DalamudServices, PetServices, PettableUserList, DirtyListener, DirtyCaller));
        Register(new TransientGuideHook(DalamudServices, PetServices, PettableUserList, DirtyListener));

        Register(new CharacterManagerHook(DalamudServices, PettableUserList, PetServices, DirtyListener, Database, LegacyDatabase, SharingDictionary, DirtyCaller, IslandHook));
    }

    private void Register(IHookableElement element)
    {
        hookableElements.Add(element);

        element?.Init();
    }

    public void Dispose()
    {
        foreach (IHookableElement hookableElement in hookableElements)
        {
            hookableElement.Dispose();
        }

        TooltipHookHelper.Dispose();
    }
}
