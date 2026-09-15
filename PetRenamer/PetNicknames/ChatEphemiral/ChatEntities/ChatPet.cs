using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;

internal class ChatPet : IChatPet
{
    public PetSkeleton Pet   { get; }
    public IChatPlayer Owner { get; }
    
    public ChatPet(PetSkeleton pet, IChatPlayer owner)
    {
        Pet   = pet;
        Owner = owner;
    }
    
    public uint LastUsedAt
    {
        get;
        set => Owner.LastUsedAt = field = value;
    }
}