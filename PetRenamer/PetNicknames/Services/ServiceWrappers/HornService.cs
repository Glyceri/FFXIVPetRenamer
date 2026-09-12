using FFXIVClientStructs.FFXIV.Client.Game;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal unsafe class HornService : IHornService
{
    private readonly IPetSheets Sheets;
    
    public HornService(IPetSheets sheets)
    {
        Sheets = sheets;
    }
    
    public XBMPet? GetPetForSlot(byte slot)
    {
        // There are only 3 pets.
        if (slot >= 3)
        {
            return null;
        }
        
        ActionManager* actionManager = ActionManager.Instance();
        
        if (actionManager == null)
        {
            return null;
        }
        
        byte pet = actionManager->BeastmasterPets[slot];
        
        if (pet == 0)
        {
            return null;
        }
        
        return Sheets.GetSheetXBMPet(pet);
    }
}