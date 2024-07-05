using Lumina.Excel.GeneratedSheets;
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
    IPetSheetData? GetPetFromActionName(string actionName);
    IPetSheetData? GetPetFromString(string text, ref IPettableUser user, bool IsSoft);
    List<IPetSheetData> GetListFromLine(string line);
    int? NameToSoftSkeletonIndex(string name);
    int? CastToSoftIndex(uint castId);
    bool IsValidBattlePet(int skeleton);


    [System.Obsolete] int[] GetObsoleteIDsFromClass(int classJob);
}
