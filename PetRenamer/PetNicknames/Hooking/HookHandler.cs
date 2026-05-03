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

    public IIslandHook        IslandHook        { get; private set; } = null!;
    public IPronounHook       PronounHook       { get; private set; } = null!;

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

        _Register();
    }

    private void _Register()
    {
        Register(new HoverHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        
        MapHook mapHook = new MapHook(DalamudServices, PetServices, PettableUserList, DirtyListener);
        Register(mapHook);
        
        PronounHook = new PronounHook(DalamudServices, PetServices, PettableUserList, DirtyListener);
        Register(PronounHook);
        
        Register(new TooltipHook(DalamudServices, PetServices, PettableUserList, DirtyListener, mapHook, PronounHook));
        Register(new ActionMenuHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new MinionNoteBookHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        
        Register(new TargetHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        
        IslandHook = new IslandHook(DalamudServices, PettableUserList, PetServices, DirtyListener);
        Register(IslandHook);
        
        PetServices.NameService.RegisterPronounHook(PronounHook);
        
        Register(new NamePlateHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new FlyTextHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        Register(new PartyHook(DalamudServices, PetServices, PettableUserList, DirtyListener));
        
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
    }
}
