using Dalamud.Game.Text;
using Dalamud.Utility;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing;

internal class ChatElementDatabase : IChatElementDatabase
{
    private readonly IPetServices PetServices;
    
    public List<IEphemeralChatElement> Elements { get; } = [];
    
    public ChatElementDatabase(IPetServices petServices)
    {
        PetServices = petServices;
    }
    
    public IEphemeralChatElement? GetChatElement(int id)
    {
        int chatsLength = Elements.Count;
        
        for (int i = 0; i < chatsLength; i++)
        {
            IEphemeralChatElement currentElement = Elements[i];
            
            if (currentElement.MessageId != id)
            {
                continue;
            }
            
            return currentElement;
        }
        
        return null;
    }

    public void RemoveElement(IEphemeralChatElement element)
    {
        Elements.Remove(element);
    }

    public void AddChatElement(IEphemeralChatElement chatElement)
    {
        RemoveElement(chatElement);
        
        Elements.Add(chatElement);
    }

    public void AddChatElement(NameType replaceNameType, IPetSheetData? replaceData, uint messageId, uint logMessageId, XivChatType chatType, IChatPlayer? sourcePlayer, IChatPlayer? targetPlayer, IChatPet? sourcePet, IChatPet? targetPet)
    {
        if (replaceData == null)
        {
            return;
        }
        
        string? replaceString = string.Empty;
            
        if (replaceNameType != NameType.Pronoun)
        {
            replaceString = PetServices.NameService.GetName(replaceNameType, replaceData);
                
            if (replaceString.IsNullOrWhitespace())
            {
                return;
            }
        }
        
        Elements.Add(new ChatElement(messageId, chatType, logMessageId, replaceString, sourcePlayer, targetPlayer, sourcePet, targetPet));
    }
}