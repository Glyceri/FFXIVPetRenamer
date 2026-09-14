using PetRenamer.PetNicknames.Update.Interfaces;
using System.Diagnostics.CodeAnalysis;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IHornService : IUpdatable
{
    const byte AmountOfSlots = 3;
    
    bool TryGetPetForSlot(byte slot, [NotNullWhen(true)] out XBMPet? xbmPet);
    bool HasChangedForSlot(byte slot);
}