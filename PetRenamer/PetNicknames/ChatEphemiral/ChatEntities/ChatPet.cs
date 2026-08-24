using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;

internal class ChatPet : IChatPet
{
    public PetSkeleton Pet          { get; private set; }
    public IChatPlayer Owner        { get; private set; }
    public uint        LastUsedAt   { get; set; }
    
    public ChatPet(PetSkeleton pet, IChatPlayer owner)
    {
        Pet   = pet;
        Owner = owner;
    }
}