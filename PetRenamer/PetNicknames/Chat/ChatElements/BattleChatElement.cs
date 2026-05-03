using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class BattleChatElement : RestrictedChatElement
{
    private readonly IPetServices PetServices;

    public BattleChatElement(IPetServices petServices)
    {
        PetServices = petServices;
        
        RegisterChat(XivChatType.Action);
    }
    
    private void HandleAsPet(IHandleableChatMessage chatMessage, IPettablePet pet)
    {
        if (!pet.IsActive)
        {
            return;
        }
        
        PetServices.StringHelper.ReplaceChat(chatMessage, pet, NameType.Pronoun);
    }
    
    private void HandleAsUser(IHandleableChatMessage chatMessage, IPettableUser user)
    {
        if (!user.IsActive)
        {
            return;
        }
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromAction((uint)PetServices.PetCastHelper.LastCastId, in user);
        
        if (petData == null)
        {
            return;
        }
        
        PetServices.StringHelper.ReplaceChat(chatMessage, petData, NameType.Action, user);
    }

    protected override void OnRestrictedChatMessage(IHandleableChatMessage chatMessage)
    {
        if (!PetServices.Configuration.showInBattleChat)
        {
            return;
        }

        if (PetServices.PetCastHelper.LastCastDealer is IPettablePet pet)
        {
            HandleAsPet(chatMessage, pet);
        }
        
        if (PetServices.PetCastHelper.LastCastDealer is IPettableUser user)
        {
            HandleAsUser(chatMessage,  user);
        }
    }
}
