using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing;

internal class ChatPlayerDatabase : IChatPlayerDatabase
{
    public List<IChatPlayer> Elements { get; } = [];
    
    private readonly IPetServices PetServices;
    
    public ChatPlayerDatabase(IPetServices petServices)
    {
        PetServices = petServices;
    }
    
    public IChatPlayer? FindChatPlayer(string playerName, ushort homeworld)
    {
        foreach (IChatPlayer player in Elements)
        {
            if (homeworld != player.Homeworld)
            {
                continue;
            }
            
            if (!string.Equals(playerName, player.PlayerName, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }
            
            return player;
        }
        
        return null;
    }

    public IChatPlayer MakeChatPlayer(string playerName, ushort homeworld)
    {
        IChatPlayer?            foundElement = FindChatPlayer(playerName, homeworld);
        IPettableDatabaseEntry? entry        = PetServices.Database.GetEntry(playerName, homeworld, false);
            
        if (entry != null)
        {
            foundElement ??= new ChatPlayer(entry.ContentId, entry.Name, entry.Homeworld);
            
            foundElement.MakeStrong(entry.ContentId);
        }
            
        foundElement ??= new ChatPlayer(playerName, homeworld);
        
        foundElement.UpdatePlayerData(playerName, homeworld);
        
        Elements.Remove(foundElement);
        Elements.Add(foundElement);
        
        if (Elements.Count > IChatPlayerDatabase.MAX_CHAT_ELEMENTS)
        {
            CleanUp();
        }
        
        return foundElement;
    }
    
    public void CleanUp()
    {
        // How did this shit even trigger.
        if (Elements.Count <= 0)
        {
            return;
        }
        
        // Remove all elements with a 0 entry.
        for (int i = Elements.Count - 1; i >= 0; i--)
        {
            if (Elements[i].LastUsedAt != 0)
            {
                continue;
            }
            
            Elements.RemoveAt(i);
        }
        
        // Remove all players that don't have a chat backing their existance.
        for (int i = Elements.Count - 1; i >= 0; i--)
        {
            IChatPlayer chatPlayer = Elements[i];
            
            int length = PetServices.ChatDatabaseService.ChatElementDatabase.Elements.Count;
            
            bool hasMessage = false;
            
            for (int f = length - 1; f >= 0; f--)
            {
                IEphemeralChatElement chatElement = PetServices.ChatDatabaseService.ChatElementDatabase.Elements[f];
                
                if (chatElement.MessageId != chatPlayer.LastUsedAt)
                {
                    continue;
                }
                
                hasMessage = true;
                
                break;
            }
            
            if (hasMessage)
            {
                continue;
            }
            
            Elements.RemoveAt(i);
        }
        
        // Remove all players that don't have a pet backing their existance.
        for (int i = Elements.Count - 1; i >= 0; i--)
        {
            IChatPlayer chatPlayer = Elements[i];
            
            int length = PetServices.ChatDatabaseService.PetDatabase.Elements.Count;
            
            bool hasPet = false;
            
            for (int f = length - 1; f >= 0; f--)
            {
                IChatPet chatPet = PetServices.ChatDatabaseService.PetDatabase.Elements[f];
                
                if (chatPet.Owner.ContentId != chatPlayer.ContentId)
                {
                    continue;
                }
                
                hasPet = true;
                
                break;
            }
            
            if (hasPet)
            {
                continue;
            }
            
            Elements.RemoveAt(i);
        }
        
        // Sorts based on last used
        Elements.Sort((player1, player2) => player2.LastUsedAt.CompareTo(player1.LastUsedAt));
        
        // Remove the last so many messages
        Elements.RemoveRange(Elements.Count - (int)IChatPlayerDatabase.CLEANUP_COUNT - 1, (int)IChatPlayerDatabase.CLEANUP_COUNT);
    }
}