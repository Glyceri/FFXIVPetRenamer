using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatLogParsers;

internal class MinionNoteBookChatLogParser : BaseChatLogParser
{
    public MinionNoteBookChatLogParser(IChatLogPlayerParserElement chatLogPlayerParserElement, List<IChatLogPetParserElement> chatLogPetParsers)
        : base (chatLogPlayerParserElement, chatLogPetParsers) 
        { }
    
    protected override void OnParse(ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity, XivChatType chatType, uint messageId, out IChatPlayer? sourcePlayer, out IChatPlayer? targetPlayer, out IChatPet? sourcePet, out IChatPet? targetPet)
    {
        sourcePet       = null;
        targetPet       = null;
        sourcePlayer    = null;
        targetPlayer    = null;
        
        if (chatType != XivChatType.SystemMessage)
        {
            return;
        }
        
        IChatPlayer? chatPlayer = ChatLogPlayerParserElement.MakeFromLocalPlayer();
        
        if (chatPlayer == null)
        {
            return;
        }
        
        targetPet = ParsePet(chatType, messageId, chatPlayer);
        
        if (targetPet == null)
        {
            return;
        }
        
        sourcePlayer = chatPlayer;
    }
}