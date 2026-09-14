using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Diagnostics.CodeAnalysis;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal unsafe class HornService : IHornService
{
    private readonly IPetSheets Sheets;
    private readonly byte[]     _lastHornPets = new byte[IHornService.AmountOfSlots]; 
    
    public HornService(IPetSheets sheets)
    {
        Sheets = sheets;
    }
    
    public bool HasChangedForSlot(byte slot)
    {
        if (slot >= IHornService.AmountOfSlots)
        {
            return false;
        }
                
        ActionManager* actionManager = ActionManager.Instance();
        
        if (actionManager == null)
        {
            return false;
        }
        
        byte pet = actionManager->BeastmasterPets[slot];
        
        if (_lastHornPets[slot] == pet)
        {
            return false;
        }
        
        return true;
    }
    
    public bool TryGetPetForSlot(byte slot, [NotNullWhen(true)] out XBMPet? xbmPet)
    {
        xbmPet = null;
        
        if (slot >= IHornService.AmountOfSlots)
        {
            return false;
        }
        
        ActionManager* actionManager = ActionManager.Instance();
        
        if (actionManager == null)
        {
            return false;
        }
        
        byte pet = actionManager->BeastmasterPets[slot];
        
        if (pet == 0)
        {
            return false;
        }
        
        xbmPet = Sheets.GetSheetXBMPet(pet);
        
        return (xbmPet != null);
    }

    public bool Enabled
        => true;
    
    public void OnUpdate(IFramework framework)
    {
        ActionManager* actionManager = ActionManager.Instance();
        
        if (actionManager == null)
        {
            return;
        }
        
        for (byte i = 0; i < IHornService.AmountOfSlots; i++)
        {
            _lastHornPets[i] = actionManager->BeastmasterPets[i];
        }
    }
}