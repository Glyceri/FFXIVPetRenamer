using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.ChatElement;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers;

internal class ChatMessageParser : IChatMessageParser
{
    private readonly List<IChatMessageParserElement> ChatMessageParsers = [];
    
    public ChatMessageParser(IPetServices petServices)
    {
        ChatMessageParsers.Add(new EmoteChatMessageParserElement(petServices));
    }
    
    public void OnChatMessage(uint messageId, XivChatType xivChatType)
    {
        IChatMessageParserElement? messageParser = null;
        
        foreach (IChatMessageParserElement chatMessageParser in ChatMessageParsers)
        {
            if (!chatMessageParser.IsMyMessage(xivChatType))
            {
                continue;
            }
            
            messageParser = chatMessageParser;
            
            break;
        }
        
        if (messageParser == null)
        {
            return;
        }
        
        messageParser.Parse(messageId, xivChatType);
    }
}