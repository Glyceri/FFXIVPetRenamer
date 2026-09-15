using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Linq;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Pet;

internal class SystemChatXBMNotebookLogParserElement : IChatLogPetParserElement
{
    private static readonly uint[] ValidMessageIds =
    [
        11481, // Your <sheet(Pet,<sheet(XBMPet,lnum1,0)>,0)> is assigned to the <switch(lnum2,first battlehorn,second battlehorn,third battlehorn)>.
    ];

    private readonly IPetServices PetServices;
    
    public SystemChatXBMNotebookLogParserElement(IPetServices petServices)
    {
        PetServices = petServices;
    }
    
    public NameType ReplaceNameType
        { get; private set; } = NameType.Raw;
    
    public IPetSheetData? UsedData 
        { get; private set; } = null;
    
    public void Reset() 
        => UsedData = null;

    public bool IsMyParser(XivChatType chatType, uint messageId)
    {
        if (!ValidMessageIds.Contains(messageId))
        {
            return false;
        }
        
        return chatType == XivChatType.SystemMessage;
    }

    public IChatPet? Parse(uint messageId, IChatPlayer? chatPlayer)
    {
        UsedData        = null;
        ReplaceNameType = NameType.Raw;
        
        if (chatPlayer == null)
        { 
            return null;
        }
        
        if (PetServices.UserList.LocalPlayer == null)
        {
            return null;
        }
        
        for (byte i = 0; i < IHornService.AmountOfSlots; i++)
        {
            if (!PetServices.HornService.TryGetPetForSlot(i, out XBMPet? pet))
            {
                continue;
            }
            
            bool hasChanged = PetServices.HornService.HasChangedForSlot(i);
            
            if (!hasChanged)
            {
                continue;
            }
            
            IPetSheetData? petData = PetServices.PetSheets.GetPetFromIcon(pet.Value.Icon);
            
            if (petData == null)
            {
                continue;
            }
            
            UsedData = petData;
            
            return PetServices.ChatDatabaseService.PetDatabase.MakeChatPet(petData.Model, chatPlayer);
        }
        
        return null;
    }
}