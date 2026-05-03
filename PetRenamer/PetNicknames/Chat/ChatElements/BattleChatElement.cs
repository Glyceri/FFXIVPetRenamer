using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class BattleChatElement : RestrictedChatElement
{
    private readonly IPettableUserList  UserList;
    private readonly IPetServices       PetServices;

    public BattleChatElement(IPetServices petServices, IPettableUserList userList)
    {
        UserList    = userList;
        PetServices = petServices;
        
        RegisterChat(XivChatType.Action, XivChatType.SystemMessage, XivChatType.SystemError);
    }

    protected override void OnRestrictedChatMessage(IHandleableChatMessage chatMessage)
    {
        if (!PetServices.Configuration.showInBattleChat)
        {
            return;
        }
        
        IPettableUser? user = UserList.GetUser(PetServices.PetCastHelper.LastCastDealer);
        
        if (user == null)
        {
            return;
        }
        
        if (!user.IsActive)
        {
            return;
        }

        IPetSheetData? petData;
        IPettablePet?  battlePet = UserList.GetPet(PetServices.PetCastHelper.LastCastDealer);

        if (battlePet == null)
        {
            petData = PetServices.PetSheets.GetPetFromAction((uint)PetServices.PetCastHelper.LastCastID, in user);
        }
        else
        {
            petData = battlePet.PetData;
        }
        
        if (petData == null)
        {
            return;
        }

        string? customName = user.GetCustomName(petData);
        
        if (customName == null)
        {
            return;
        }

        SeString message = chatMessage.Message;
        
        PetServices.StringHelper.ReplaceSeString(ref message, customName, petData);
        
        chatMessage.Message = message;
    }
}
