using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.Chat.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Chat.Base;

internal abstract class RestrictedChatElement : IChatElement
{
    private readonly HashSet<int> ChatTypes = [];

    protected void RegisterChat(XivChatType chatType) 
        => ChatTypes.Add((int)chatType);
    
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
