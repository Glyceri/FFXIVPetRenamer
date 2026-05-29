using PetRenamer.PetNicknames.Hooking.HookElements;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking;

internal class HookHandler : IDisposable
{
    private readonly DalamudServices    DalamudServices;
    private readonly IPetServices       PetServices;
    private readonly IPettableDatabase  Database;
    private readonly ILegacyDatabase    LegacyDatabase;
    private readonly ISharingDictionary SharingDictionary;

    public IIslandHook  IslandHook        { get; private set; } = null!;
    public IPronounHook PronounHook       { get; private set; } = null!;

    private readonly List<IHookableElement> hookableElements = [];

    public HookHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableDatabase database, ILegacyDatabase legacyDatabase, ISharingDictionary sharingDictionary)
    {
        DalamudServices   = dalamudServices;
        PetServices       = petServices;
        Database          = database;
        LegacyDatabase    = legacyDatabase;
        SharingDictionary = sharingDictionary;

        _Register();
    }

    private void _Register()
    {
        Register(new MirageHook(DalamudServices, PetServices));
        Register(new HoverHook(DalamudServices, PetServices));
        
        MapHook mapHook = new MapHook(DalamudServices, PetServices);
        Register(mapHook);
        
        PronounHook = new PronounHook(DalamudServices, PetServices);
        Register(PronounHook);
        
        Register(new TooltipHook(DalamudServices, PetServices, mapHook, PronounHook));
        Register(new ActionMenuHook(DalamudServices, PetServices));
        Register(new MinionNoteBookHook(DalamudServices, PetServices));
        
        Register(new TargetHook(DalamudServices, PetServices));
        
        IslandHook = new IslandHook(DalamudServices, PetServices);
        Register(IslandHook);
        
        PetServices.NameService.RegisterPronounHook(PronounHook);
        
        Register(new CastHook(DalamudServices, PetServices));
        Register(new NamePlateHook(DalamudServices, PetServices));
        Register(new PartyHook(DalamudServices, PetServices));
        
        Register(new CharacterManagerHook(DalamudServices, PetServices, Database, LegacyDatabase, SharingDictionary, IslandHook));
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
