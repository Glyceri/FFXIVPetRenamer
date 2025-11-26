using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetSheets
{
    TextCommand? GetCommand(uint id);
    Action? GetAction(uint actionID);
    BNpcName? GetBNPCName(uint bnpcID);
    string? GetClassName(int id);
    string? GetWorldName(ushort worldID);
    IPetSheetData? GetPet(PetSkeleton skeletonID);
    List<IPetSheetData> GetLegacyPets(int legacyModelID);
    List<IPetSheetData> GetMissingPets(List<PetSkeleton> battlePetSkeletons);
    PetSkeleton ToSoftSkeleton(PetSkeleton skeletonID, PetSkeleton[] softSkeletons);
    IPetSheetData? GetPetFromName(string name);
    IPetSheetData? GetPetFromIcon(uint iconID);
    IPetSheetData? GetPetFromAction(uint actionID, in IPettableUser user, bool IsSoft = true);
    IPetSheetData? GetPetFromString(string text, in IPettableUser user, bool IsSoft = false);
    IPetSheetData? MakeSoft(in IPettableUser user, in IPetSheetData oldData);
    int? NameToSoftSkeletonIndex(string name);
    int? CastToSoftIndex(uint castId);
    bool IsValidBattlePet(PetSkeleton skeleton);


    [System.Obsolete] PetSkeleton[] GetObsoleteIDsFromClass(int classJob);
}
