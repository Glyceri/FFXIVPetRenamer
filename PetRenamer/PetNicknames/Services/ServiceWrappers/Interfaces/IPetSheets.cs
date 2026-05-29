using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;
using Action = Lumina.Excel.Sheets.Action;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetSheets
{
    Action?             GetAction(uint actionId);
    BNpcName?           GetBNpcName(uint bNpcId);
    string?             GetWorldName(ushort worldId);
    IPetSheetData?      GetPet(PetSkeleton skeletonId);
    IPetSheetData[]     GetLegacyPets(int legacyModelId);
    List<IPetSheetData> GetMissingPets(List<PetSkeleton> battlePetSkeletons);
    IPetSheetData?      GetPetFromName(string name);
    IPetSheetData?      GetPetFromIcon(uint iconId);
    IPetSheetData?      GetPetFromAction(uint actionId, IPettableUser user, bool isSoft = true);
    IPetSheetData       MakeSoft(IPettableUser user, IPetSheetData oldData);
    int?                CastToSoftIndex(uint castId);
    PetMirage?          GetPetMirage(uint petMirageId);

    [Obsolete] PetSkeleton[] GetObsoleteIDsFromClass(int classJob);
}
