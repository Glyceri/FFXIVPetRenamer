using Dalamud.Game;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class SheetsWrapper : IPetSheets
{
    private readonly DalamudServices DalamudServices;
    private readonly IStringHelper   StringHelper;

    private readonly List<IPetSheetData>      petSheetCache = [];
    private readonly string[]                 nameToClass   = [];

    private readonly ExcelSheet<Companion>?   petSheet;
    private readonly ExcelSheet<Pet>?         battlePetSheet;
    private readonly ExcelSheet<World>?       worlds;
    private readonly ExcelSheet<ClassJob>?    classJob;
    private readonly ExcelSheet<Action>?      actions;
    private readonly ExcelSheet<TextCommand>? textCommands;
    private readonly ExcelSheet<BNpcName>?    bnpcNames;

    public SheetsWrapper(ref DalamudServices dalamudServices, IStringHelper helper)
    {
        DalamudServices = dalamudServices;
        StringHelper    = helper;

        petSheet        = dalamudServices.DataManager.GetExcelSheet<Companion>();
        worlds          = dalamudServices.DataManager.GetExcelSheet<World>();
        classJob        = dalamudServices.DataManager.GetExcelSheet<ClassJob>();
        battlePetSheet  = dalamudServices.DataManager.GetExcelSheet<Pet>();
        actions         = dalamudServices.DataManager.GetExcelSheet<Action>();
        textCommands    = dalamudServices.DataManager.GetExcelSheet<TextCommand>();
        bnpcNames       = dalamudServices.DataManager.GetExcelSheet<BNpcName>();

        SetupSheetDataCache();

        nameToClass = DalamudServices.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => japaneseNames,
            ClientLanguage.English  => englishNames,
            ClientLanguage.German   => germanNames,
            ClientLanguage.French   => frenchNames,
            _ => englishNames,
        };
    }

    private void SetupCopmanionSheet()
    {
        if (petSheet == null)
        {
            return;
        }

        foreach (Companion companion in petSheet)
        {
            if (!companion.Model.IsValid)
            {
                continue;
            }

            ModelChara? model = companion.Model.ValueNullable;

            if (model == null)
            {
                continue;
            }

            uint companionIndex = companion.RowId;
            int  modelID        = (int)model.Value.RowId;

            PetSkeleton petSkeleton = new PetSkeleton((uint)modelID, SkeletonType.Minion);

            int  legacyModelID  = model.Value.Model;

            if (legacyModelID == 0)
            {
                continue;
            }

            string singular = companion.Singular.ExtractText();

            if (singular.IsNullOrWhitespace())
            {
                continue;
            }

            singular = singular.ToTitleCase();

            string plural        = companion.Plural.ExtractText();
            uint   icon          = companion.Icon;

            sbyte  pronoun       = companion.Pronoun;

            uint   raceID        = companion.MinionRace.ValueNullable?.RowId ?? 0;
            string raceName      = companion.MinionRace.ValueNullable?.Name.ExtractText() ?? string.Empty;
            string behaviourName = companion.Behavior.ValueNullable?.Name.ExtractText() ?? string.Empty;

            petSheetCache.Add(new PetSheetData(petSkeleton, legacyModelID, icon, raceName, raceID, behaviourName, pronoun, singular, plural, singular, companionIndex, in DalamudServices));
        }
    }

    private void SetupBattlePetSheet()
    {
        if (battlePetSheet == null)
        {
            return;
        }

        foreach (Pet pet in battlePetSheet)
        {
            uint sheetSkeleton = pet.RowId;

            if (!battlePetRemap.TryGetValue(sheetSkeleton, out PetSkeleton skeleton))
            {
                continue;
            }

            if (!petIDToAction.TryGetValue(skeleton, out uint actionID))
            {
                continue;
            }

            if (!battlePetToBNpcName.TryGetValue(skeleton, out uint bnpcnameId))
            {
                continue;
            }

            Action? petAction = GetAction(actionID);

            if (petAction == null)
            {
                continue;
            }

            ushort    petIcon  = petAction.Value.Icon;

            BNpcName? bnpcName = GetBNPCName(bnpcnameId);

            if (bnpcName == null)
            {
                continue;
            }

            string name              = bnpcName.Value.Singular.ExtractText().ToTitleCase();
            string actionName        = petAction.Value.Name.ExtractText();
            uint   actionRowID       = petAction.Value.RowId;

            string cleanedActionName = StringHelper.CleanupActionName(StringHelper.CleanupString(actionName));

            petSheetCache.Add(new PetSheetData(skeleton, -1, petIcon, bnpcName.Value.Pronoun, name, cleanedActionName, actionName, actionRowID, in DalamudServices));
        }
    }

    private void SetupSheetDataCache()
    {
        SetupCopmanionSheet();

        SetupBattlePetSheet();
    }

    public TextCommand? GetCommand(uint id) 
        => textCommands?.GetRow(id);

    public Action? GetAction(uint actionID) 
        => actions?.GetRow(actionID);

    public BNpcName? GetBNPCName(uint bnpcID) 
        => bnpcNames?.GetRow(bnpcID);

    public string? GetClassName(int id)
    {
        if (classJob == null)
        {
            return null;
        }

        foreach (ClassJob cls in classJob)
        {
            if (cls.RowId != id)
            {
                continue;
            }

            return cls.Name.ExtractText();
        }

        return null;
    }

    public string? GetWorldName(ushort worldID)
    {
        if (worlds == null)
        {
            return null;
        }

        World? world = worlds.GetRow(worldID);

        if (world == null)
        {
            return null;
        }

        return world.Value.InternalName.ExtractText();
    }

    public IPetSheetData? GetPet(PetSkeleton skeletonID)
    {
        for (int i = 0; i < petSheetCache.Count; i++)
        {
            if (petSheetCache[i].Model != skeletonID)
            {
                continue;
            }

            return petSheetCache[i];
        }

        return null;
    }

    public PetSkeleton ToSoftSkeleton(PetSkeleton skeletonID, PetSkeleton[] softSkeletons)
    {
        bool canBeSoft = mutatableID.Contains(skeletonID);

        if (!canBeSoft)
        {
            return skeletonID;
        }

        int index = -1;

        for (int i = 0; i < PluginConstants.BaseSkeletons.Length; i++)
        {
            if (PluginConstants.BaseSkeletons[i] != skeletonID)
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

        return skeletonID;
    }

    public IPetSheetData? GetPetFromName(string name)
    {
        int sheetCount = petSheetCache.Count;

        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = petSheetCache[i];

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

        if (name == string.Empty || name == null)
        {
            return null;
        }

        for (int i = 0; i < nameToClass.Length; i++)
        {
            string nameToClassUnmodified = nameToClass[i];
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

        for (int i = 0; i < castToIndex.Count; i++)
        {
            uint cast = castToIndex[i];

            if (cast != castId)
            {
                continue;
            }

            return i;
        }

        return null;
    }

    public List<IPetSheetData> GetListFromLine(string line)
    {
        List<IPetSheetData> list = [];

        if (line == string.Empty)
        {
            return list;
        }

        foreach (IPetSheetData pet in petSheetCache)
        {
            if (pet.BaseSingular.InvariantEquals(line))
            {
                list.Add(pet);

                return list;
            }

            bool hasValidValue = false;

            hasValidValue |= (line.InvariantContains(pet.BaseSingular) && !pet.BaseSingular.IsNullOrWhitespace());
            hasValidValue |= (line.InvariantContains(pet.BasePlural)   && !pet.BasePlural.IsNullOrWhitespace());
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

        list.Sort((i1, i2) =>
        {
            return i1.LongestIdentifier().CompareTo(i2.LongestIdentifier());
        });

        list.Reverse();

        return list;
    }

    public IPetSheetData? GetPetFromString(string baseString, in IPettableUser user, bool soft)
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

        return MakeSoft(in user, in normalPetData);
    }

    public IPetSheetData? GetPetFromIcon(uint iconID)
    {
        if (iconID == 0 || iconID == uint.MaxValue)
        {
            return null;
        }

        int sheetCount = petSheetCache.Count;

        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = petSheetCache[i];

            if (pet.Icon != iconID)
            {
                continue;
            }

            return pet;
        }

        return null;
    }

    public IPetSheetData? GetPetFromAction(uint actionID, in IPettableUser user, bool IsSoft)
    {
        if (actionID == 0 || actionID == uint.MaxValue)
        {
            return null;
        }

        IPetSheetData? activePet = null;

        int sheetCount = petSheetCache.Count;

        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = petSheetCache[i];

            if (!pet.IsAction(actionID))
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

        if (!IsSoft)
        {
            return activePet;
        }

        return MakeSoft(in user, in activePet);
    }

    public IPetSheetData? MakeSoft(in IPettableUser user, in IPetSheetData oldData)
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

        return GetFromSoftIndex(in user, in oldData, softIndex.Value);
    }

    public IPetSheetData? GetFromSoftIndex(in IPettableUser user, in IPetSheetData oldData, int softIndex)
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

        return new PetSheetData(softPetData.Model, softPetData.Icon, softPetData.Pronoun, oldData.BaseSingular, oldData.BasePlural, in DalamudServices);
    }

    public bool IsValidBattlePet(PetSkeleton skeleton) 
        => petIDToAction.ContainsKey(skeleton);

    public List<IPetSheetData> GetLegacyPets(int legacyModelID)
    {
        List<IPetSheetData> legacyPets = [];

        for (int i = 0; i < petSheetCache.Count; i++)
        {
            IPetSheetData data = petSheetCache[i];

            if (data.LegacyModelID != legacyModelID)
            {
                continue;
            }

            legacyPets.Add(data);
        }

        return legacyPets;
    }

    [System.Obsolete]
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

        foreach(IPetSheetData data in petSheetCache)
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

    public readonly Dictionary<PetSkeleton, uint> battlePetToBNpcName = new Dictionary<PetSkeleton, uint>()
    {
        { PluginConstants.EmeraldCarbuncle      , 1401 },
        { PluginConstants.RubyCarbuncle         , 4149 },
        { PluginConstants.Carbuncle             , 10261 },
        { PluginConstants.TopazCarbuncle        , 1400 },
        { PluginConstants.IfritEgi              , 1402 },
        { PluginConstants.TitanEgi              , 1403 },
        { PluginConstants.GarudaEgi             , 1404 },
        { PluginConstants.Eos                   , 1398 },
        { PluginConstants.Selene                , 1399 },
        { PluginConstants.AutomatonQueen        , 8230 },
        { PluginConstants.Seraph                , 8227 },
        { PluginConstants.Phoenix               , 8228 },
        { PluginConstants.LivingShadow          , 8229 },
        { PluginConstants.IffritII              , 10262 },
        { PluginConstants.GarudaII              , 10263 },
        { PluginConstants.TitanII               , 10264 },
        { PluginConstants.Bahamut               , 6566 },
        { PluginConstants.RookAutoTurret        , 3666 },
        { PluginConstants.SolarBahamut          , 13159 },

    };

    public readonly Dictionary<uint, PetSkeleton> battlePetRemap = new Dictionary<uint, PetSkeleton>
    {
        { 6,    PluginConstants.Eos                     }, // EOS
        { 7,    PluginConstants.Selene                  }, // Selene

        { 1,    PluginConstants.EmeraldCarbuncle        }, // Emerald Carbuncle
        { 38,   PluginConstants.RubyCarbuncle           }, // Ruby Carbuncle
        { 2,    PluginConstants.TopazCarbuncle          }, // Topaz Carbuncle
        { 36,   PluginConstants.Carbuncle               }, // Carbuncle

        { 27,   PluginConstants.IfritEgi                }, // Ifrit-Egi
        { 28,   PluginConstants.TitanEgi                }, // Titan-Egi
        { 29,   PluginConstants.GarudaEgi               }, // Garuda-Egi 

        { 8,    PluginConstants.RookAutoTurret          }, // Rook Autoturret MCHN
        { 21,   PluginConstants.Seraph                  }, // Seraph
        { 18,   PluginConstants.AutomatonQueen          }, // Automaton Queen
        { 17,   PluginConstants.LivingShadow            }, // Esteem DRK

        { 14,   PluginConstants.Phoenix                 }, // Demi-Phoenix
        { 10,   PluginConstants.Bahamut                 }, // Demi-Bahamut
        { 32,   PluginConstants.GarudaII                }, // Emerald-Garuda
        { 31,   PluginConstants.TitanII                 }, // Topaz-Titan
        { 30,   PluginConstants.IffritII                }, // Ruby-Iffrit
        { 46,   PluginConstants.SolarBahamut            }, // Solar Bahamut
    };

    public readonly Dictionary<PetSkeleton, uint> petIDToAction = new Dictionary<PetSkeleton, uint>()
    {
        { PluginConstants.EmeraldCarbuncle      , 25804 }, // Summon Emerald
        { PluginConstants.RubyCarbuncle         , 25802 }, // Summon Ruby
        { PluginConstants.Carbuncle             , 25798 }, // Summon Carbuncle
        { PluginConstants.TopazCarbuncle        , 25803 }, // Summon Topaz
        { PluginConstants.IfritEgi              , 25805 }, // Summon Ifrit
        { PluginConstants.TitanEgi              , 25806 }, // Summon Titan
        { PluginConstants.GarudaEgi             , 25807 }, // Summon Garuda
        { PluginConstants.Eos                   , 17215 }, // Summon Eos
        { PluginConstants.Selene                , 17215 }, // Summon Eos
        { PluginConstants.AutomatonQueen        , 16501 }, // Automaton Queen
        { PluginConstants.Seraph                , 16545 }, // Summon Seraph
        { PluginConstants.Phoenix               , 25831 }, // Summon Phoenix
        { PluginConstants.LivingShadow          , 16472 }, // Living Shadow
        { PluginConstants.IffritII              , 25838 }, // Summon Ifrit II
        { PluginConstants.GarudaII              , 25840 }, // Summon Garuda II
        { PluginConstants.TitanII               , 25839 }, // Summon Titan II
        { PluginConstants.Bahamut               , 7427  }, // Summon Bahamut
        { PluginConstants.RookAutoTurret        , 2864  }, // Rook Autoturret
        { PluginConstants.SolarBahamut          , 36992 }, // Summon Solar Bahamut        
    };

    public readonly List<PetSkeleton> mutatableID = new List<PetSkeleton>()
    {
        PluginConstants.Eos                 , // Eos
        PluginConstants.Selene              , // Selene
        PluginConstants.EmeraldCarbuncle    , // Emerald Carbuncle
        PluginConstants.RubyCarbuncle       , // Ruby Carbuncle
        PluginConstants.Carbuncle           , // Carbuncle
        PluginConstants.TopazCarbuncle      , // Topaz Carbuncle
        PluginConstants.IfritEgi            , // Ifrit-Egi
        PluginConstants.TitanEgi            , // Titan-Egi
        PluginConstants.GarudaEgi             // Garuda-Egi
    };

    // Sheets wrapper explains why the order is like this... it's crucial it stays like this.
    // Soft Mapping is the most hardcoded thing in this plogon :c
    // 0 --> Karfunkel
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos
    public readonly IReadOnlyList<uint> castToIndex = new List<uint>()
    {
        25798,   // Summon Carbuncle
        25807,   // Summon Garuda
        25806,   // Summon Titan
        25805,   // Summon Ifrit
        17215,   // Summon Eos
    };

    // Hardcoded now
    // This command hasn't changed in a while, and if it does well... I'll have to rework the system anyways
    // 0 --> Carbuncle
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos
    private readonly string[] englishNames  = ["Carbuncle", "Garuda-Egi", "Titan-Egi", "Ifrit-Egi", "Eos"];
    private readonly string[] germanNames   = ["Karfunkel", "Garuda-Egi", "Titan-Egi", "Ifrit-Egi", "Eos"];
    private readonly string[] frenchNames   = ["Carbuncle", "Garuda-Egi", "Titan-Egi", "Ifrit-Egi", "Eos"];
    private readonly string[] japaneseNames = ["カーバンクル", "ガルーダ・エギ", "タイタン・エギ", "イフリート・エギ", "フェアリー・エオス"];

    [System.Obsolete("Classes have been obsolete since 1.4")]
    public readonly Dictionary<int, PetSkeleton[]> battlePetToClass = new Dictionary<int, PetSkeleton[]>()
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
