using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Pet;

internal class CastDealerUserChatLogParserElement : IChatLogPetParserElement
{
    private readonly IPetServices PetServices;
    
    public CastDealerUserChatLogParserElement(IPetServices petServices)
    {
        PetServices = petServices;
    }

    public NameType ReplaceNameType 
        => NameType.Action;
    
    public IPetSheetData? UsedData 
        { get; private set; } = null;

    public void Reset() 
        => UsedData = null;
    
    public bool IsMyParser(XivChatType chatType, uint messageId)
    {
        if (chatType != XivChatType.Action)
        {
            return false;
        }
        
        if (PetServices.PetCastHelper.LastCastDealer is not IPettableUser)
        {
            return false;
        }
        
        return true;
    }

    public IChatPet? Parse(uint messageId, IChatPlayer? chatPlayer)
    {
        UsedData = null;
        
        if (chatPlayer == null)
        {
            return null;
        }
        
        if (PetServices.PetCastHelper.LastCastDealer is not IPettableUser user)
        {
            return null;
        }
        
        if (user.DataBaseEntry.ContentId != chatPlayer.ContentId)
        {
            return null;
        }
        
        PetServices.PetLog.LogFatal(PetServices.PetCastHelper.LastAction);
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromAction(PetServices.PetCastHelper.LastAction);
        
        if (petData == null)
        {
            return null;
        }
        
        PetServices.PetLog.LogFatal(petData.Singular);
        
        UsedData = PetServices.PetSheets.MakeSoft(user, petData);
        
        return PetServices.ChatDatabaseService.PetDatabase.MakeChatPet(UsedData.Model, user.DataBaseEntry.Name, user.DataBaseEntry.Homeworld);
    }
}