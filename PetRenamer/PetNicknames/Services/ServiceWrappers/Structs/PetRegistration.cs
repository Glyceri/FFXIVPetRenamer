using Lumina.Excel.Sheets;
using Lumina.Extensions;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Linq;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal readonly struct PetRegistration
{
    private readonly uint               _pet;
    private readonly uint               _bnpcName;
    private readonly uint               _action;
    private readonly SkeletonType       _skeletonType;
    private readonly LegacySkeletonType _legacySkeletonType;
    private readonly PetSkeleton        _petSkeleton;
    
    public PetRegistration(uint pet, uint modelChara, uint bnpcName, uint action, SkeletonType skeletonType, LegacySkeletonType legacySkeletonType)
    {
        _pet                = pet;
        _bnpcName           = bnpcName;
        _action             = action;
        _skeletonType       = skeletonType;
        _legacySkeletonType = legacySkeletonType;
        _petSkeleton        = new PetSkeleton(modelChara, _skeletonType);
    }
    
    public readonly PetSkeleton PetSkeleton
        => _petSkeleton;
    
    public readonly SkeletonType SkeletonType
        => _skeletonType;
    
    public readonly LegacySkeletonType LegacySkeletonType
        => _legacySkeletonType;
    
    public readonly Pet? GetBattlePet(IPetSheets sheets)
        => _skeletonType == SkeletonType.BattlePet ? sheets.GetSheetPet(_pet) : null;
    
    public readonly XBMPet? GetBeastMasterPet(IPetSheets sheets)
        => _skeletonType == SkeletonType.BeastMaster ? sheets.GetSheetXBMPet(_pet) : null;
    
    public readonly BNpcName? GetBNPCName(IPetSheets sheets)
        => sheets.GetBNpcName(_bnpcName);
    
    public readonly Action? GetAction(IPetSheets sheets)
        => sheets.GetAction(_action);
    
    public static PetRegistration? GetRegistrationFromPet(uint pet)
        => PluginConstants.PetRegistrations.FirstOrNull(x => x._pet == pet);
    
    public static PetRegistration? GetRegistrationFromAction(uint action)
        => PluginConstants.PetRegistrations.FirstOrNull(x => x._action == action);
    
    public static PetRegistration? GetRegistrationFromBNPCName(uint bnpcName)
        => PluginConstants.PetRegistrations.FirstOrNull(x => x._bnpcName == bnpcName);
    
    public static PetRegistration[] GetRegistrationsFromClass(LegacySkeletonType classJob)
        => PluginConstants.PetRegistrations.Where(x => x._legacySkeletonType == classJob).ToArray();
}