using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IHoverService
{
    NameType       CurrentNameType     { get; }
    IPetSheetData? CurrentlyHoveredPet { get; }
    
    void SetCurrentNameType(NameType nameType);
    void SetHoveredPet(IPetSheetData? currentlyHoveredPet);
}