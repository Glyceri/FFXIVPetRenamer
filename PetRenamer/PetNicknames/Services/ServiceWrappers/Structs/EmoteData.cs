using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal readonly struct EmoteData
{
    public readonly IPettableUser Source;
    public readonly uint          EmoteId;
    public readonly IPettablePet  Target;
    
    public EmoteData(IPettableUser source, uint emoteId, IPettablePet target)
    {
        Source  = source;
        EmoteId = emoteId;
        Target  = target;
    }
}