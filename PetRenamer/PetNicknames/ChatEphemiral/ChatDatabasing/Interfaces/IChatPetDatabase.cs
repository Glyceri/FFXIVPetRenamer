using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatPetDatabase : IChatDatabase<IChatPet>
{
    const uint MAX_CHAT_ELEMENTS = 4096;
    const uint CLEANUP_COUNT     = 512;
    
    IChatPet? FindChatPet(PetSkeleton petSkeleton, IChatPlayer owner);
    IChatPet  MakeChatPet(PetSkeleton petSkeleton, IChatPlayer owner);
    IChatPet  MakeChatPet(PetSkeleton petSkeleton, string ownerName, ushort ownerHomeworld);
}