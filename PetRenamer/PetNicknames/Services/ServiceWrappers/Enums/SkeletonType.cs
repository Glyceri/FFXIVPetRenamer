using PetRenamer.PetNicknames.Services.ServiceWrappers.Attributes;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

internal enum SkeletonType : byte
{
    Invalid     = 0,
    None        = 0,
    [SkeletonTypeSymbol('+')]
    Minion      = 1,
    LovmPet     = 1,
    [SkeletonTypeSymbol('-')]
    BattlePet   = 2,
    [SkeletonTypeSymbol('#')]
    BeastMaster = 3,    // Future proofing c:
    Chocobo     = 4,
    COUNT       = 5,
}
