using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class EmoteService : IEmoteService
{
    private const    int             CleanUpAfter = 5;
    private readonly List<EmoteData> _emotes      = [];

    public EmoteData[] EmoteData
        => [.._emotes];
    
    public void RegisterEmote(IPettableUser source, uint emoteId, IPettablePet target)
    {
        _emotes.Insert(0, new EmoteData(source, emoteId, target));
        
        HandleCleanup();
    }

    public void ClearEmotes()
    {
        _emotes.Clear();
    }

    private void HandleCleanup()
    {
        if (_emotes.Count <= CleanUpAfter)
        {
            return;
        }
        
        _emotes.RemoveRange(CleanUpAfter, _emotes.Count - CleanUpAfter);
    }
}