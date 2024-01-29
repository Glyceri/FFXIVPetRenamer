using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System.Collections.Generic;

namespace PetRenamer.Core.Chat;

internal abstract class RestrictedChatElement : ChatElement
{
    readonly HashSet<int> ChatTypes = new HashSet<int>();

    internal void RegisterChat(int chatType) => ChatTypes.Add(chatType);
    internal void RegisterChat(XivChatType chatType) => ChatTypes.Add((int)chatType);
    internal void RegisterChat(params int[] chats)
    {
        for(int i = 0; i < chats.Length; i++)
            ChatTypes.Add(chats[i]);
    }
    internal void RegisterChat(params XivChatType[] chats)
    {
        for (int i = 0; i < chats.Length; i++)
            ChatTypes.Add((int)chats[i]);
    }

    internal sealed override void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!ChatTypes.Contains((int)type)) return;
        OnRestrictedChatMessage(type, senderId, ref sender, ref message, ref isHandled);
    }

    internal abstract void OnRestrictedChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled);
}
