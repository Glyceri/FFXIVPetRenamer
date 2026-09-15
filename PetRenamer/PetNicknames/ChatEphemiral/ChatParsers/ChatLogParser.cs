using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatLogParsers;
using PetRenamer.PetNicknames.ChatEphemiral.ChatLogParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Pet;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Player;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers;

internal class ChatLogParser : IChatLogParser
{
    private readonly List<IChatLogChatParser>        ChatLogChatParsers = [];
    private readonly IChatLogPlayerParserElement     ChatLogPlayerParserElement;
    private readonly List<IChatLogPetParserElement>  ChatLogPetParsers = [];
    
    private NameType       _replaceNameType = NameType.Raw;
    private IPetSheetData? _replaceData     = null;
    
    private readonly IPetServices PetServices;
    
    public ChatLogParser(IPetServices petServices)
    {
        PetServices                = petServices;
        ChatLogPlayerParserElement = new PlayerChatLogParserElement(petServices);
        
        ChatLogPetParsers.Add(new CastDealerPetChatLogParserElement(petServices));
        ChatLogPetParsers.Add(new CastDealerUserChatLogParserElement(petServices));
        ChatLogPetParsers.Add(new EmoteChatLogParserElement(petServices));
        ChatLogPetParsers.Add(new SystemChatLogParserElement(petServices));
        ChatLogPetParsers.Add(new SystemChatNotebookLogParserElement(petServices));
        ChatLogPetParsers.Add(new SystemChatXBMNotebookLogParserElement(petServices));
        
        ChatLogChatParsers.Add(new BasicChatLogParser(ChatLogPlayerParserElement, ChatLogPetParsers));
        ChatLogChatParsers.Add(new MinionNoteBookChatLogParser(ChatLogPlayerParserElement, ChatLogPetParsers));
    }
    
    public void OnChatLog(uint messageId, XivChatType xivChatType, uint logMessageId, ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity)
    {
        _replaceNameType = NameType.Raw;
        _replaceData     = null;
        
        ResetParsers();
        
        IChatPlayer? sourcePlayer = null;
        IChatPlayer? targetPlayer = null;
        IChatPet?    sourcePet    = null;
        IChatPet?    targetPet    = null;
        
        foreach (IChatLogChatParser chatParser in ChatLogChatParsers)
        {
            chatParser.Parse(sourceEntity, targetEntity, xivChatType, logMessageId, out sourcePlayer, out targetPlayer, out sourcePet, out targetPet);
            
            if (!ParsesSucceeded(sourcePlayer, targetPlayer, sourcePet, targetPet))
            {
                continue;
            }
            
            _replaceData     = chatParser.ReplaceData;
            _replaceNameType = chatParser.ReplaceNameType;
            
            PetLogWrapper.Instance?.DevLogVerbose(chatParser.GetType().Name);
            
            break;
        }
        
        PetLogWrapper.Instance?.DevLog($"Message: {sourcePlayer?.ContentId}, {targetPlayer?.ContentId}, {sourcePet?.Pet}, {targetPet?.Pet}");
        
        if (_replaceData == null)
        {
            return;
        }
        
        PetServices.ChatDatabaseService.ChatElementDatabase.AddChatElement(_replaceNameType, _replaceData, messageId, logMessageId, xivChatType, sourcePlayer, targetPlayer, sourcePet, targetPet);
    }
    
    private bool ParsesSucceeded(IChatPlayer? sourcePlayer, IChatPlayer? targetPlayer, IChatPet? sourcePet, IChatPet? targetPet)
        => (sourcePlayer != null || targetPlayer != null || sourcePet != null || targetPet != null);
    
    private void ResetParsers()
    {
        foreach (IChatLogPetParserElement petParser in ChatLogPetParsers)
        {
            petParser.Reset();
        }
    }
}