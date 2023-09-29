using System.Collections.Generic;

namespace PetRenamer.Core.Debug;

public static class DebugStorage
{
    // SETTINGS
    static int maxPetChatCount = 100;
    static int removePetChatCount = 10;



    static int petChatCount = 0;
    public static readonly List<PetChatMessage> petChatMessages = new List<PetChatMessage>();
    public static void InsertPetChatMessage(PetChatMessage message)
    {
        petChatMessages.Add(message);
        petChatCount++;
        if (petChatCount > maxPetChatCount)
        {
            petChatMessages.RemoveRange(0, removePetChatCount);
            petChatCount = petChatMessages.Count;
        }
    }

    public static void Dispose()
    {
        petChatMessages.Clear();
    }
}
