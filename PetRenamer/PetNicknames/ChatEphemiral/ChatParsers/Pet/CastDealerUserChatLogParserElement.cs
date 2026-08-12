using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Pet;

internal class CastDealerUserChatLogParserElement : IChatLogPetParserElement
{
    private readonly IChatPetDatabase PetDatabase;
    private readonly IPetServices     PetServices;
    
    public CastDealerUserChatLogParserElement(IChatPetDatabase petDatabase, IPetServices petServices)
    {
        PetDatabase = petDatabase;
        PetServices = petServices;
    }

    public NameType ReplaceNameType 
        => NameType.Action;
    
    public IPetSheetData? UsedData 
        { get; private set; } = null;

    public bool IsMyParser(XivChatType chatType)
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
        
        if (PetServices.PetCastHelper.LastCastDealer is not IPettableUser user)
        {
            return null;
        }
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromAction((uint)PetServices.PetCastHelper.LastCastId);
        
        if (petData == null)
        {
            return null;
        }
        
        UsedData = PetServices.PetSheets.MakeSoft(user, petData);
        
        return PetDatabase.MakeChatPet(UsedData.Model, user.DataBaseEntry.Name, user.DataBaseEntry.Homeworld);
    }
}