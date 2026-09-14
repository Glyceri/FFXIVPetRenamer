using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatLogParsers;

internal class BasicChatLogParser : BaseChatLogParser
{
    public BasicChatLogParser(IChatLogPlayerParserElement chatLogPlayerParserElement, List<IChatLogPetParserElement> chatLogPetParsers)
        : base (chatLogPlayerParserElement, chatLogPetParsers)
        { }
    
    protected override void OnParse(ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity, XivChatType chatType, uint messageId, out IChatPlayer? sourcePlayer, out IChatPlayer? targetPlayer, out IChatPet? sourcePet, out IChatPet? targetPet)
    {
        sourcePet = null;
        targetPet = null;
        
        sourcePlayer = ChatLogPlayerParserElement.Parse(sourceEntity);
        targetPlayer = ChatLogPlayerParserElement.Parse(targetEntity);
        
        if (sourceEntity != null)
        {
            sourcePet = ParsePet(chatType, messageId, sourcePlayer);
        }
        
        if (targetEntity != null)
        {
            targetPet = ParsePet(chatType, messageId, sourcePlayer);
        }
    }
}