using Dalamud.Game.Text;
using PetRenamer.PetNicknames.Chat.Interfaces;
using Dalamud.Game.Chat;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class EmoteChatElement : IChatElement
{
    private readonly IPetServices PetServices;

    public EmoteChatElement(IPetServices petServices) 
    {
        PetServices = petServices;
    }
    
    public void OnChatMessage(IHandleableChatMessage chatMessage)
    {
        if (chatMessage.LogKind != XivChatType.StandardEmote)
        {
            return;
        }
        
        IPettableUser?   senderUser = PetServices.UserList.GetUser(chatMessage.Sender.TextValue);
        IPettableEntity? target     = PetServices.TargetManager.GetLeadingTarget(senderUser);
        
        if (target is not IPettablePet pet) 
        {
            return;
        }

        PetServices.StringHelper.ReplaceChat(PetServices.Configuration.ShowOnEmotesColour, chatMessage, pet, NameType.Pronoun);
    }
}

