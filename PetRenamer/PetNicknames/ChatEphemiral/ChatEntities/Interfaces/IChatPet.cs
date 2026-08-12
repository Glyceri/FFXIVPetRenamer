using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;

internal interface IChatPet : IChatEntity
{    
    PetSkeleton Pet   { get; }
    IChatPlayer Owner { get; }
}