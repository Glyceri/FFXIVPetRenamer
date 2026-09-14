using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatLogParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatLogParsers;

internal abstract class BaseChatLogParser : IChatLogChatParser
{
    public NameType       ReplaceNameType { get; private set; }
    public IPetSheetData? ReplaceData     { get; private set; }

    protected readonly IChatLogPlayerParserElement    ChatLogPlayerParserElement;
    protected readonly List<IChatLogPetParserElement> ChatLogPetParsers;
    
    protected BaseChatLogParser(IChatLogPlayerParserElement chatLogPlayerParserElement, List<IChatLogPetParserElement> chatLogPetParsers)
    {
        ChatLogPlayerParserElement = chatLogPlayerParserElement;
        ChatLogPetParsers          = chatLogPetParsers;
    }
    
    public void Parse(ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity, XivChatType chatType, uint messageId, out IChatPlayer? sourcePlayer, out IChatPlayer? targetPlayer, out IChatPet? sourcePet, out IChatPet? targetPet)
    {
        ReplaceNameType = NameType.Raw;
        ReplaceData     = null;
        
        OnParse(sourceEntity, targetEntity, chatType, messageId, out sourcePlayer, out targetPlayer, out sourcePet, out targetPet);
    }
    
    protected abstract void OnParse(ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity, XivChatType chatType, uint messageId, out IChatPlayer? sourcePlayer, out IChatPlayer? targetPlayer, out IChatPet? sourcePet, out IChatPet? targetPet);
    
    protected IChatPet? ParsePet(XivChatType chatType, uint messageId, IChatPlayer? playerElement)
    {
        IChatLogPetParserElement? activeParser = null;
        
        foreach (IChatLogPetParserElement petParser in ChatLogPetParsers)
        {
            bool isMyParser = petParser.IsMyParser(chatType, messageId);
            
            if (!isMyParser)
            {
                continue;
            }
            
            activeParser = petParser;
            
            break;
        }
        
        if (activeParser == null)
        {
            return null;
        }
        
        IChatPet? returner = activeParser.Parse(messageId, playerElement);
        
        if (returner == null)
        {
            return null;
        }
        
        if (activeParser.UsedData != null)
        {
            ReplaceData     = activeParser.UsedData;
            ReplaceNameType = activeParser.ReplaceNameType;
        }
        
        return returner;
    }
}