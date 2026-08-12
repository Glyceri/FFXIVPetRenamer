using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatPetDatabase : IChatDatabase<IChatPet>
{
    IChatPet? FindChatPet(PetSkeleton petSkeleton, IChatPlayer owner);
    IChatPet  MakeChatPet(PetSkeleton petSkeleton, IChatPlayer owner);
    IChatPet  MakeChatPet(PetSkeleton petSkeleton, string ownerName, ushort ownerHomeworld);
}