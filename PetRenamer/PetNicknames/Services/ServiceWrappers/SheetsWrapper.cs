using Lumina.Excel;
using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Lumina.Excel.Sheets.Action;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class SheetsWrapper : IPetSheets
{
    private readonly DalamudServices         DalamudServices;

    private readonly List<IPetSheetData>     PetSheetCache = [];

    private readonly ExcelSheet<Companion>   PetSheet;
    private readonly ExcelSheet<Pet>         BattlePetSheet;
    private readonly ExcelSheet<World>       WorldSheet;
    private readonly ExcelSheet<Action>      ActionSheet;
    private readonly ExcelSheet<BNpcName>    BNpcNameSheet;
    private readonly ExcelSheet<PetMirage>   PetMirageSheet;

    public SheetsWrapper(DalamudServices dalamudServices)
    {
        DalamudServices  = dalamudServices;

        PetSheet         = dalamudServices.DataManager.GetExcelSheet<Companion>();
        WorldSheet       = dalamudServices.DataManager.GetExcelSheet<World>();
        BattlePetSheet   = dalamudServices.DataManager.GetExcelSheet<Pet>();
        ActionSheet      = dalamudServices.DataManager.GetExcelSheet<Action>();
        BNpcNameSheet    = dalamudServices.DataManager.GetExcelSheet<BNpcName>();
        PetMirageSheet   = dalamudServices.DataManager.GetExcelSheet<PetMirage>();

        SetupSheetDataCache();
    }

    private void SetupSheetDataCache()
    {
        SetupCompanionSheet();
        SetupBattlePetSheet();
    }
    
    private void SetupCompanionSheet()
    {
        foreach (Companion companion in PetSheet)
        {
            IPetSheetData? petSheetData = PetSheetData.CreatePetSheetData(DalamudServices, companion);
            
            if (petSheetData == null)
            {
                continue;
            }
            
            PetSheetCache.Add(petSheetData);
        }
    }

    private void SetupBattlePetSheet()
    {
        foreach (Pet pet in BattlePetSheet)
        {
            IPetSheetData? petSheetData = PetSheetData.CreatePetSheetData(this, DalamudServices, pet);
            
            if (petSheetData == null)
            {
                continue;
            }
            
            PetSheetCache.Add(petSheetData);
        }
    }
    
    public Action? GetAction(uint actionId) 
        => ActionSheet.GetRow(actionId);

    public BNpcName? GetBNpcName(uint bnpcId) 
        => BNpcNameSheet.GetRow(bnpcId);

    public PetMirage? GetPetMirage(uint petMirageId)
        => petMirageId != 0 ? PetMirageSheet.GetRow(petMirageId) : null;

    public string GetWorldName(ushort worldId) 
        => WorldSheet.GetRow(worldId).InternalName.ExtractText();

    public IPetSheetData? GetPet(PetSkeleton skeletonId)
        => PetSheetCache.FirstOrDefault(t => t.Model == skeletonId);

    public IPetSheetData[] AllPets
        => PetSheetCache.ToArray();
    
    public IPetSheetData? GetPetFromName(string name)
    {
        int sheetCount = PetSheetCache.Count;

        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = PetSheetCache[i];

            if (!pet.IsPet(name))
            {
                continue;
            }

            return pet;
        }

        return null;
    }

    private int? ToSoftIndex(PetSkeleton petSkeleton)
    {
        int? index = null;
        
        for (int i = 0; i < PluginConstants.BaseSkeletons.Length; i++)
        {
            if (PluginConstants.BaseSkeletons[i] != petSkeleton)
            {
                continue;
            }
            
            index = i;
            
            break;
        }
        
        return index;
    }
    
    public int? CastToSoftIndex(uint castId)
    {
        if (castId == 0)
        {
            return null;
        }

        PetSkeleton? foundSkeleton = null;
        
        foreach (KeyValuePair<PetSkeleton, uint> keyValuePair in PluginConstants.PetIdToAction)
        {
            if (keyValuePair.Value != castId)
            {
                continue;
            }
             
            foundSkeleton = keyValuePair.Key;
            
            break;
        }
        
        if (foundSkeleton == null)
        {
            return null;
        }
        
        return ToSoftIndex(foundSkeleton.Value);
    }

    public IPetSheetData? GetPetFromIcon(uint iconId)
    {
        int sheetCount = PetSheetCache.Count;

        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = PetSheetCache[i];

            if (pet.Icon != iconId)
            {
                continue;
            }

            return pet;
        }

        return null;
    }

    public IPetSheetData? GetPetFromAction(uint actionId)
    {
        int sheetCount = PetSheetCache.Count;

        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = PetSheetCache[i];

            if (!pet.IsAction(actionId))
            {
                continue;
            }

            return pet;
        }

        return null;
    }

    public IPetSheetData MakeSoft(IPettableUser user, IPetSheetData oldData)
    {
        if (oldData.Model.SkeletonType != SkeletonType.BattlePet)
        {
            return oldData;
        }

        int? softIndex = ToSoftIndex(oldData.Model);
        
        if (softIndex == null)
        {
            return oldData;
        }

        return GetFromSoftIndex(user, oldData, softIndex.Value);
    }

    private IPetSheetData GetFromSoftIndex(IPettableUser user, IPetSheetData oldData, int softIndex)
    {
        PetSkeleton? softSkeleton = user.DataBaseEntry.GetSoftSkeleton(softIndex);

        if (softSkeleton == null)
        {
            return oldData;
        }

        IPetSheetData? softPetData = GetPet(softSkeleton.Value);

        if (softPetData == null)
        {
            return oldData;
        }

        return new PetSheetData(softPetData.Model, softPetData.LegacyModelId, softPetData.Icon, softPetData.Pronoun, oldData.Singular, oldData.ActionName, oldData.ActionId, DalamudServices);
    }

    public IPetSheetData[] GetLegacyPets(int legacyModelId)
        => PetSheetCache.Where(data => data.LegacyModelId == legacyModelId).ToArray();

    [Obsolete]
    public PetSkeleton[] GetObsoleteIDsFromClass(int classJob)
    {
        if (PluginConstants.BattlePetToClass.TryGetValue(classJob, out PetSkeleton[]? id))
        {
            return id;
        }

        return [];  
    }

    public List<IPetSheetData> GetMissingBattlePets(List<PetSkeleton> battlePetSkeletons)
    {
        List<IPetSheetData> sheetData = [];

        foreach(IPetSheetData data in PetSheetCache)
        {
            PetSkeleton model = data.Model;

            if (model.SkeletonType != SkeletonType.BattlePet)
            {
                continue;
            }

            if (battlePetSkeletons.Contains(model))
            {
                continue;
            }

            sheetData.Add(data);
        }

        return sheetData;
    }
}
