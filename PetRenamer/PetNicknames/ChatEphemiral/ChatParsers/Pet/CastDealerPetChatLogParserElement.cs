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

internal class CastDealerPetChatLogParserElement : IChatLogPetParserElement
{    
    private readonly IChatPetDatabase PetDatabase;
    private readonly IPetServices     PetServices;
    
    public CastDealerPetChatLogParserElement(IChatPetDatabase petDatabase, IPetServices petServices)
    {
        PetServices = petServices;
        PetDatabase = petDatabase;
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
        
        return (PetServices.PetCastHelper.LastCastDealer is IPettablePet);
    }

    public IChatPet? Parse(uint messageId, IChatPlayer? chatPlayer)
    {
        UsedData = null;
        
        if (PetServices.PetCastHelper.LastCastDealer is not IPettablePet pet)
        {
            return null;
        }
        
        if (pet.Owner == null)
        {
            return null;
        }
        
        UsedData = pet.PetData;
        
        return PetDatabase.MakeChatPet(pet.SkeletonId, pet.Owner.DataBaseEntry.Name, pet.Owner.DataBaseEntry.Homeworld);
    }
}