using Dalamud.Game;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Lumina.Excel.Sheets.Action;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class SheetsWrapper : IPetSheets
{
    private readonly DalamudServices         DalamudServices;
    private readonly IStringHelper           StringHelper;

    private readonly List<IPetSheetData>     PetSheetCache = [];
    private readonly string[]                NameToClass;

    private readonly ExcelSheet<Companion>   PetSheet;
    private readonly ExcelSheet<Pet>         BattlePetSheet;
    private readonly ExcelSheet<World>       WorldSheet;
    private readonly ExcelSheet<ClassJob>    ClassJobSheet;
    private readonly ExcelSheet<Action>      ActionSheet;
    private readonly ExcelSheet<TextCommand> TextCommandSheet;
    private readonly ExcelSheet<BNpcName>    BNpcNameSheet;

    public SheetsWrapper(DalamudServices dalamudServices, IStringHelper helper)
    {
        DalamudServices  = dalamudServices;
        StringHelper     = helper;

        PetSheet         = dalamudServices.DataManager.GetExcelSheet<Companion>();
        WorldSheet       = dalamudServices.DataManager.GetExcelSheet<World>();
        ClassJobSheet    = dalamudServices.DataManager.GetExcelSheet<ClassJob>();
        BattlePetSheet   = dalamudServices.DataManager.GetExcelSheet<Pet>();
        ActionSheet      = dalamudServices.DataManager.GetExcelSheet<Action>();
        TextCommandSheet = dalamudServices.DataManager.GetExcelSheet<TextCommand>();
        BNpcNameSheet    = dalamudServices.DataManager.GetExcelSheet<BNpcName>();

        SetupSheetDataCache();

        NameToClass = DalamudServices.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => JapaneseNames,
            ClientLanguage.English  => EnglishNames,
            ClientLanguage.German   => GermanNames,
            ClientLanguage.French   => FrenchNames,
            _                       => EnglishNames,
        };
    }

    private void SetupCopmanionSheet()
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
            IPetSheetData? petSheetData = PetSheetData.CreatePetSheetData(this, StringHelper, DalamudServices, pet);
            
            if (petSheetData == null)
            {
                continue;
            }
            
            PetSheetCache.Add(petSheetData);
        }
    }

    private void SetupSheetDataCache()
    {
        SetupCopmanionSheet();

        SetupBattlePetSheet();
    }

    public TextCommand? GetCommand(uint id) 
        => TextCommandSheet.GetRow(id);

    public Action? GetAction(uint actionId) 
        => ActionSheet.GetRow(actionId);

    public BNpcName? GetBNpcName(uint bnpcId) 
        => BNpcNameSheet.GetRow(bnpcId);

    public string? GetClassName(int id)
    {
        foreach (ClassJob cls in ClassJobSheet)
        {
            if (cls.RowId != id)
            {
                continue;
            }

            return cls.Name.ExtractText();
        }

        return null;
    }

    public string GetWorldName(ushort worldId) 
        => WorldSheet.GetRow(worldId).InternalName.ExtractText();

    public IPetSheetData? GetPet(PetSkeleton skeletonId)
    {
        for (int i = 0; i < PetSheetCache.Count; i++)
        {
            if (PetSheetCache[i].Model != skeletonId)
            {
                continue;
            }

            return PetSheetCache[i];
        }

        return null;
    }

    public PetSkeleton ToSoftSkeleton(PetSkeleton skeletonId, PetSkeleton[] softSkeletons)
    {
        bool canBeSoft = MutatableIds.Contains(skeletonId);

        if (!canBeSoft)
        {
            return skeletonId;
        }

        int index = -1;

        for (int i = 0; i < PluginConstants.BaseSkeletons.Length; i++)
        {
            if (PluginConstants.BaseSkeletons[i] != skeletonId)
            {
                continue;
            }
            
            index = i;

            break;
        }

        if (index >= 0 && index < softSkeletons.Length)
        {
            return softSkeletons[index];
        }

        return skeletonId;
    }

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

    public int? NameToSoftSkeletonIndex(string name)
    {
        name = name.Trim();

        if (name.IsNullOrWhitespace())
        {
            return null;
        }

        for (int i = 0; i < NameToClass.Length; i++)
        {
            string nameToClassUnmodified = NameToClass[i];
            string converted             = StringHelper.CleanupActionName(nameToClassUnmodified);

            if (!nameToClassUnmodified.InvariantEquals(name) && !converted.InvariantEquals(name))
            {
                continue;
            }
            
            return i;
        }

        return null;
    }

    public int? CastToSoftIndex(uint castId)
    {
        if (castId == 0)
        {
            return null;
        }

        for (int i = 0; i < CastToIndex.Length; i++)
        {
            uint cast = CastToIndex[i];

            if (cast != castId)
            {
                continue;
            }

            return i;
        }

        return null;
    }

    private List<IPetSheetData> GetListFromLine(string line)
    {
        List<IPetSheetData> list = [];

        if (line == string.Empty)
        {
            return list;
        }

        foreach (IPetSheetData pet in PetSheetCache)
        {
            if (pet.BaseSingular.InvariantEquals(line))
            {
                list.Add(pet);

                return list;
            }

            bool hasValidValue = false;

            hasValidValue |= (line.InvariantContains(pet.BaseSingular) && !pet.BaseSingular.IsNullOrWhitespace());
            hasValidValue |= (line.InvariantContains(pet.ActionName)   && !pet.ActionName.IsNullOrWhitespace());

            if (!hasValidValue)
            {
                continue;
            }

            list.Add(pet);
        }

        if (list.Count == 0)
        {
            return list;
        }

        list.Sort((i1, i2) => string.Compare(i1.LongestIdentifier(), i2.LongestIdentifier(), StringComparison.Ordinal));

        list.Reverse();

        return list;
    }

    public IPetSheetData? GetPetFromString(string baseString, IPettableUser user, bool soft)
    {
        List<IPetSheetData> data = GetListFromLine(baseString);

        if (data.Count == 0)
        {
            return null;
        }

        IPetSheetData normalPetData = data[0];

        if (normalPetData.Model.SkeletonType != SkeletonType.BattlePet)
        {
            return normalPetData;
        }

        if (!soft)
        {
            return normalPetData;
        }

        return MakeSoft(user, normalPetData);
    }

    public IPetSheetData? GetPetFromIcon(uint iconId)
    {
        if (iconId == 0 || iconId == uint.MaxValue)
        {
            return null;
        }

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

    public IPetSheetData? GetPetFromAction(uint actionId, IPettableUser user, bool isSoft = true)
    {
        if (actionId == 0 || actionId == uint.MaxValue)
        {
            return null;
        }

        IPetSheetData? activePet = null;

        int sheetCount = PetSheetCache.Count;

        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = PetSheetCache[i];

            if (!pet.IsAction(actionId))
            {
                continue;
            }

            activePet = pet;

            break;
        }

        if (activePet == null)
        {
            return null;
        }

        if (!isSoft)
        {
            return activePet;
        }
        
        int? softIndex = CastToSoftIndex(actionId);
        
        if (softIndex == null)
        {
            return activePet;
        }
        
        return GetFromSoftIndex(user, activePet, softIndex.Value);
    }

    public IPetSheetData MakeSoft(IPettableUser user, IPetSheetData oldData)
    {
        if (oldData.Model.SkeletonType != SkeletonType.BattlePet)
        {
            return oldData;
        }

        int? softIndex = NameToSoftSkeletonIndex(oldData.BasePlural);
        
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

        return new PetSheetData(softPetData.Model, softPetData.LegacyModelId, softPetData.Icon, softPetData.Pronoun, oldData.BaseSingular, oldData.BasePlural, oldData.ActionName, oldData.ActionId, DalamudServices);
    }

    public bool IsValidBattlePet(PetSkeleton skeleton) 
        => IPetSheets.PetIdToAction.ContainsKey(skeleton);

    public IPetSheetData[] GetLegacyPets(int legacyModelId)
        => PetSheetCache.Where(data => data.LegacyModelId == legacyModelId).ToArray();

    [Obsolete]
    public PetSkeleton[] GetObsoleteIDsFromClass(int classJob)
    {
        if (battlePetToClass.TryGetValue(classJob, out PetSkeleton[]? id))
        {
            return id;
        }

        return [];  
    }

    public List<IPetSheetData> GetMissingPets(List<PetSkeleton> battlePetSkeletons)
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

    private static readonly PetSkeleton[] MutatableIds =
    [
        PluginConstants.Eos                 , // Eos
        PluginConstants.Selene              , // Selene
        PluginConstants.EmeraldCarbuncle    , // Emerald Carbuncle
        PluginConstants.RubyCarbuncle       , // Ruby Carbuncle
        PluginConstants.Carbuncle           , // Carbuncle
        PluginConstants.TopazCarbuncle      , // Topaz Carbuncle
        PluginConstants.IfritEgi            , // Ifrit-Egi
        PluginConstants.TitanEgi            , // Titan-Egi
        PluginConstants.GarudaEgi             // Garuda-Egi
    ];

    // Sheets wrapper explains why the order is like this... it's crucial it stays like this.
    // Soft Mapping is the most hardcoded thing in this plogon :c
    // 0 --> Karfunkel
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos
    private static readonly uint[] CastToIndex =
    [
        25798,   // Summon Carbuncle
        25807,   // Summon Garuda
        25806,   // Summon Titan
        25805,   // Summon Ifrit
        17215,   // Summon Eos
    ];

    // Hardcoded now
    // This command hasn't changed in a while, and if it does well... I'll have to rework the system anyways
    // 0 --> Carbuncle
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos
    private static readonly string[] EnglishNames  = ["Carbuncle", "Garuda-Egi", "Titan-Egi", "Ifrit-Egi", "Eos"];
    private static readonly string[] GermanNames   = ["Karfunkel", "Garuda-Egi", "Titan-Egi", "Ifrit-Egi", "Eos"];
    private static readonly string[] FrenchNames   = ["Carbuncle", "Garuda-Egi", "Titan-Egi", "Ifrit-Egi", "Eos"];
    private static readonly string[] JapaneseNames = ["カーバンクル", "ガルーダ・エギ", "タイタン・エギ", "イフリート・エギ", "フェアリー・エオス"];

    [Obsolete("Classes have been obsolete since 1.4")]
    private static readonly Dictionary<int, PetSkeleton[]> battlePetToClass = new Dictionary<int, PetSkeleton[]>()
    {
        {
            PluginConstants.LegacySummonerClassID, 
            [
                PluginConstants.Carbuncle,
                PluginConstants.RubyCarbuncle,
                PluginConstants.TopazCarbuncle,
                PluginConstants.EmeraldCarbuncle,
                PluginConstants.IfritEgi,
                PluginConstants.TitanEgi,
                PluginConstants.GarudaEgi,
                PluginConstants.IffritII,
                PluginConstants.GarudaII,
                PluginConstants.TitanII,
                PluginConstants.Phoenix,
                PluginConstants.Bahamut,
                PluginConstants.SolarBahamut
            ]
        },
        {
            PluginConstants.LegacyScholarClassID, 
            [
                PluginConstants.Eos,
                PluginConstants.Selene,
                PluginConstants.Seraph
            ]
        },
        {
            PluginConstants.LegacyMachinistClassID,
            [
                PluginConstants.RookAutoTurret,
                PluginConstants.AutomatonQueen,
            ]
        },
        {
            PluginConstants.LegacyDarkKnightClassID,
            [
                PluginConstants.LivingShadow
            ]
        }
    };
}
