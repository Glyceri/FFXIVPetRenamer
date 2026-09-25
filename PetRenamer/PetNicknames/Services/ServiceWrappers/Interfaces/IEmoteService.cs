using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IEmoteService
{
    EmoteData[] EmoteData { get; }
    
    void RegisterEmote(IPettableUser source, uint emoteId, IPettablePet target);
    void ClearEmotes();
}