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
    List<IPetSheetData> GetMissingBattlePets(List<PetSkeleton> battlePetSkeletons);
    IPetSheetData?      GetPetFromName(string name);
    IPetSheetData?      GetPetFromIcon(uint iconId);
    IPetSheetData?      GetPetFromAction(uint actionId);
    IPetSheetData       MakeSoft(IPettableUser user, IPetSheetData oldData);
    int?                CastToSoftIndex(uint castId);
    PetMirage?          GetPetMirage(uint petMirageId);
    LogMessage?         GetLogMessage(uint logMessageId);

    /// <summary>
    /// CALL FOR DEBUG PURPOSES ONLY, IF YOU EVER USE THIS IN THE ACTUAL PLUGIN, YOU ARE DOING IT WRONG!
    /// </summary>
    /// <returns>All Registered Pets</returns>
    IPetSheetData[]     AllPets { get; }
    
    [Obsolete] PetSkeleton[] GetObsoleteIDsFromClass(int classJob);
}
