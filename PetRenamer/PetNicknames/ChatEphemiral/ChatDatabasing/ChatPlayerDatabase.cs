using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing;

internal class ChatPlayerDatabase : ChatEntityDatabase, IChatPlayerDatabase
{
    // TODO: Implement overflow protec for player and pet like you did for chatElement
    
    public List<IChatPlayer> Elements { get; } = [];
    
    private readonly IPettableDatabase Database;
    
    public ChatPlayerDatabase(IPettableDatabase database)
    {
        Database = database;
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
        IPettableDatabaseEntry? entry        = Database.GetEntry(playerName, homeworld, false);
            
        if (entry != null)
        {
            foundElement ??= new ChatPlayer(entry.ContentId, entry.Name, entry.Homeworld);
            
            foundElement.MakeStrong(entry.ContentId);
        }
            
        foundElement ??= new ChatPlayer(playerName, homeworld);
        
        foundElement.UpdatePlayerData(playerName, homeworld);
        
        // TODO: realistically should be hashset
        Elements.Remove(foundElement);
        Elements.Add(foundElement);
        
        return foundElement;
    }
}