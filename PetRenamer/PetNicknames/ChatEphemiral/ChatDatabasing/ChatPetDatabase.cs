using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing;

internal class ChatPetDatabase : IChatPetDatabase
{
    public List<IChatPet> Elements { get; } = [];
    
    private readonly IChatPlayerDatabase PlayerDatabase;
    
    public ChatPetDatabase(IChatPlayerDatabase playerDatabase)
    {
        PlayerDatabase = playerDatabase;
    }
    
    public IChatPet? FindChatPet(PetSkeleton petSkeleton, IChatPlayer owner)
    {
        foreach (IChatPet pet in Elements)
        {
            if (petSkeleton != pet.Pet)
            {
                continue;
            }
            
            if (owner.Homeworld != pet.Owner.Homeworld)
            {
                continue;
            }
            
            if (!string.Equals(owner.PlayerName, pet.Owner.PlayerName, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }
            
            return pet;           
        }
        
        return null;
    }

    public IChatPet MakeChatPet(PetSkeleton petSkeleton, IChatPlayer owner)
    {
        IChatPet? foundElement = FindChatPet(petSkeleton, owner);
        
        foundElement ??= new ChatPet(petSkeleton, owner);
        
        Elements.Remove(foundElement);
        Elements.Add(foundElement);
        
        return foundElement;
    }
    
    public IChatPet MakeChatPet(PetSkeleton petSkeleton, string ownerName, ushort ownerHomeworld)
        => MakeChatPet(petSkeleton, PlayerDatabase.MakeChatPlayer(ownerName, ownerHomeworld));
}