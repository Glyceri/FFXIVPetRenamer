using Lumina.Excel.GeneratedSheets2;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetSheets
{
    TextCommand? GetCommand(uint id);
    Action? GetAction(uint actionID);
    string? GetClassName(int id);
    string? GetWorldName(ushort worldID);
    IPetSheetData? GetPet(int skeletonID);
    List<IPetSheetData> GetLegacyPets(int legacyModelID);
    int ToSoftSkeleton(int skeletonID, int[] softSkeletons);
    IPetSheetData? GetPetFromName(string name);
    IPetSheetData? GetPetFromAction(uint actionID, in IPettableUser user, bool IsSoft = true);
    IPetSheetData? GetPetFromString(string text, in IPettableUser user, bool IsSoft = false);
    IPetSheetData? MakeSoft(in IPettableUser user, in IPetSheetData oldData);
    List<IPetSheetData> GetListFromLine(string line);
    int? NameToSoftSkeletonIndex(string name);
    int? CastToSoftIndex(uint castId);
    bool IsValidBattlePet(int skeleton);


    [System.Obsolete] int[] GetObsoleteIDsFromClass(int classJob);
}
