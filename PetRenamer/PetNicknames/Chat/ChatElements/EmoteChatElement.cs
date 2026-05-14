using Dalamud.Game.Text;
using PetRenamer.PetNicknames.Chat.Interfaces;
using Dalamud.Game.Chat;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class EmoteChatElement : IChatElement
{
    private readonly IPettableUserList UserList;
    private readonly IPetServices      PetServices;

    public EmoteChatElement(IPetServices petServices, IPettableUserList userList) 
    {
        UserList    = userList;
        PetServices = petServices;
    }
    
    public void OnChatMessage(IHandleableChatMessage chatMessage)
    {
        if (chatMessage.LogKind != XivChatType.StandardEmote)
        {
            return;
        }
        
        IPettableUser? senderUser = UserList.GetUser(chatMessage.Sender.TextValue);

        if (senderUser == null)
        {
            PetServices.PetLog.LogVerbose($"Sender User is NULL [{chatMessage.Sender.TextValue}].");

            return;
        }

        IPettableEntity? target = senderUser.TargetManager?.GetLeadingTarget();

        if (target == null)
        {
            PetServices.PetLog.LogVerbose($"Target is NULL [{chatMessage.Sender.TextValue}].");

            return;
        }

        if (target is not IPettablePet pet)
        {
            PetServices.PetLog.LogVerbose($"Target is NOT IPettablePet [{target.GetType().Name}].");

            return;
        }

        PetServices.StringHelper.ReplaceChat(PetServices.Configuration.ShowOnEmotesColour, chatMessage, pet, NameType.Pronoun);
    }
}

