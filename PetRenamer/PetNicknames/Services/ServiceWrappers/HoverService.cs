using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class HoverService : IHoverService
{
    public NameType CurrentNameType 
        { get; private set; } = NameType.Raw;

    public IPetSheetData? CurrentlyHoveredPet 
        { get; private set; } = null;
    
    public void SetHoveredPet(IPetSheetData? currentlyHoveredPet) 
        => CurrentlyHoveredPet = currentlyHoveredPet;
    
    public void SetCurrentNameType(NameType nameType) 
        => CurrentNameType = nameType;
}