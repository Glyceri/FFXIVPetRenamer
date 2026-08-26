using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
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
    private readonly DalamudServices        DalamudServices;
    private readonly IPetServices           PetServices;
    private readonly IPettableDatabase      Database;
    private readonly ILegacyDatabase        LegacyDatabase;
    private readonly ISharingDictionary     SharingDictionary;
    private readonly IEphemaralChatHandler  ChatHandler;
    
    public IPronounHook PronounHook { get; private set; } = null!;

    private readonly List<IHookableElement> hookableElements = [];

    public HookHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableDatabase database, ILegacyDatabase legacyDatabase, ISharingDictionary sharingDictionary, IEphemaralChatHandler chatHandler)
    {
        DalamudServices   = dalamudServices;
        PetServices       = petServices;
        Database          = database;
        LegacyDatabase    = legacyDatabase;
        SharingDictionary = sharingDictionary;
        ChatHandler       = chatHandler;

        _Register();
        _Initialize();
    }

    public void Dispose()
    {
        foreach (IHookableElement hookableElement in hookableElements)
        {
            hookableElement.Dispose();
        }
    }
    
    private void _Register()
    {
        Register(new TextElementHook(DalamudServices, PetServices));
        
        Register(new MirageHook(DalamudServices, PetServices));
        Register(new HoverHook(DalamudServices, PetServices));
        
        Register(new MapHook(DalamudServices, PetServices));
        
        PronounHook = new PronounHook(DalamudServices, PetServices);
        Register(PronounHook);
        
        Register(new ChatHook(DalamudServices, PetServices, ChatHandler));
        Register(new TooltipHook(DalamudServices, PetServices, PronounHook));
        Register(new ActionMenuHook(DalamudServices, PetServices));
        Register(new MinionNoteBookHook(DalamudServices, PetServices));
        Register(new TargetHook(DalamudServices, PetServices));
        Register(new IslandHook(DalamudServices, PetServices, Database));
        Register(new CastHook(DalamudServices, PetServices));
        Register(new NamePlateHook(DalamudServices, PetServices));
        Register(new PartyHook(DalamudServices, PetServices));
        Register(new CharacterManagerHook(DalamudServices, PetServices, Database, LegacyDatabase, SharingDictionary));
    }

    private void Register(IHookableElement element)
    {
        hookableElements.Add(element);
    }
    
    private void _Initialize()
    {
        foreach (IHookableElement hookableElement in hookableElements)
        {
            hookableElement.Init();
        }
    }
}
