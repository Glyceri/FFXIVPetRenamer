using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.Chat.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Chat.Base;

internal abstract class RestrictedChatElement : IChatElement
{
    private readonly HashSet<int> ChatTypes = [];

    internal void RegisterChat(int chatType) 
        => ChatTypes.Add(chatType);
    
    internal void RegisterChat(XivChatType chatType) 
        => ChatTypes.Add((int)chatType);
    
    internal void RegisterChat(params int[] chats)
    {
        for (int i = 0; i < chats.Length; i++)
        {
            ChatTypes.Add(chats[i]);
        }
    }
    
    internal void RegisterChat(params XivChatType[] chats)
    {
        for (int i = 0; i < chats.Length; i++)
        {
            ChatTypes.Add((int)chats[i]);
        }
    }

    public void OnChatMessage(IHandleableChatMessage chatMessage)
    {
        if (!ChatTypes.Contains((int)chatMessage.LogKind))
        {
            return;
        }
        
        OnRestrictedChatMessage(chatMessage);
    }

    protected abstract void OnRestrictedChatMessage(IHandleableChatMessage chatMessage);
}
