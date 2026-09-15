using Dalamud.Game.Text;
using Dalamud.Utility;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.ChatElement;

internal class EmoteChatMessageParserElement : IChatMessageParserElement
{
    private readonly IPetServices PetServices;
    
    public EmoteChatMessageParserElement(IPetServices petServices)
    {
        PetServices = petServices;
    }
    
    public bool IsMyMessage(XivChatType type)
    {
        return (type == XivChatType.StandardEmote);
    }

    public void Parse(uint messageId, XivChatType type)
    {
        IEphemeralChatElement? chatElement = PetServices.ChatDatabaseService.ChatElementDatabase.GetChatElement((int)messageId);
        
        if (chatElement == null)
        {
            return;
        }
        
        PetServices.ChatDatabaseService.ChatElementDatabase.RemoveElement(chatElement);
        
        if (chatElement.TargetPet == null)
        {
            return;
        }
        
        IPetSheetData? data = PetServices.PetSheets.GetPet(chatElement.TargetPet.Pet);
        
        if (data == null)
        {
            return;
        }
        
        string? replaceString = PetServices.NameService.GetName(NameType.Pronoun, data);
        
        if (replaceString.IsNullOrWhitespace())
        {
            return;
        }
        
        chatElement.SetReplaceString(replaceString);
        
        PetServices.ChatDatabaseService.ChatElementDatabase.AddChatElement(chatElement);
    }
}