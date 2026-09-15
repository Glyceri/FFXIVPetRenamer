using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing;

internal class ChatPetDatabase : IChatPetDatabase
{
    public List<IChatPet> Elements { get; } = [];

    private readonly IPetServices PetServices;
    
    public ChatPetDatabase(IPetServices petServices)
    {
        PetServices = petServices;
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
        
        if (Elements.Count > IChatPetDatabase.MAX_CHAT_ELEMENTS)
        {
            CleanUp();
        }
        
        return foundElement;
    }
    
    public IChatPet MakeChatPet(PetSkeleton petSkeleton, string ownerName, ushort ownerHomeworld)
        => MakeChatPet(petSkeleton, PetServices.ChatDatabaseService.PlayerDatabase.MakeChatPlayer(ownerName, ownerHomeworld));
    
    public void CleanUp()
    {
        if (Elements.Count <= 0)
        {
            return;
        }
        
        // Sorts based on last used
        Elements.Sort((pet1, pet2) => pet2.LastUsedAt.CompareTo(pet1.LastUsedAt));
         
        // Remove the last so many messages
        Elements.RemoveRange(Elements.Count - (int)IChatPetDatabase.CLEANUP_COUNT - 1, (int)IChatPetDatabase.CLEANUP_COUNT);
    }
}