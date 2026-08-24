using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Pet;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Player;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers;

internal class ChatLogParser : IChatLogParser
{
    private readonly IChatDatabaseHandler            ChatDatabaseHandler;
    private readonly IChatLogPlayerParserElement     ChatLogPlayerParserElement;
    private readonly List<IChatLogPetParserElement>  ChatLogPetParsers = [];
 
    private NameType       _replaceNameType = NameType.Raw;
    private IPetSheetData? _replaceData     = null;
    
    public ChatLogParser(IChatDatabaseHandler chatDatabase, IPetServices petServices)
    {
        ChatDatabaseHandler        = chatDatabase;
        ChatLogPlayerParserElement = new PlayerChatLogParserElement(chatDatabase.PlayerDatabase, chatDatabase.ChatElementDatabase);
        
        ChatLogPetParsers.Add(new CastDealerPetChatLogParserElement(chatDatabase.PetDatabase, petServices));
        ChatLogPetParsers.Add(new CastDealerUserChatLogParserElement(chatDatabase.PetDatabase, petServices));
        ChatLogPetParsers.Add(new EmoteChatLogParserElement(chatDatabase.PetDatabase, petServices));
        ChatLogPetParsers.Add(new SystemChatLogParserElement(chatDatabase.PetDatabase, petServices));
    }
    
    public void OnChatLog(uint messageId, XivChatType xivChatType, uint logMessageId, ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity)
    {
        _replaceNameType = NameType.Raw;
        _replaceData     = null;
        
        ResetParsers();
        
        IChatPlayer? sourcePlayer = ChatLogPlayerParserElement.Parse(sourceEntity);
        IChatPlayer? targetPlayer = ChatLogPlayerParserElement.Parse(targetEntity);
        IChatPet?    sourcePet    = ParsePet(xivChatType, logMessageId, sourcePlayer, sourceEntity);
        IChatPet?    targetPet    = ParsePet(xivChatType, logMessageId, sourcePlayer, targetEntity);
        
        if (_replaceData == null)
        {
            return;
        }
        
        ChatDatabaseHandler.ChatElementDatabase.AddChatElement(_replaceNameType, _replaceData, messageId, logMessageId, xivChatType, sourcePlayer, targetPlayer, sourcePet, targetPet);
    }
    
    private IChatPet? ParsePet(XivChatType chatType, uint messageId, IChatPlayer? playerElement, ILogMessageEntity? logMessageEntity)
    {
        if (logMessageEntity == null)
        {
            return null;
        }
        
        IChatLogPetParserElement? activeParser = null;
        
        foreach (IChatLogPetParserElement petParser in ChatLogPetParsers)
        {
            if (!petParser.IsMyParser(chatType))
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
            _replaceData     = activeParser.UsedData;
            _replaceNameType = activeParser.ReplaceNameType;
        }
        
        return returner;
    }
    
    private void ResetParsers()
    {
        foreach (IChatLogPetParserElement petParser in ChatLogPetParsers)
        {
            petParser.Reset();
        }
    }
}