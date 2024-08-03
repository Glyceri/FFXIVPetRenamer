using Dalamud.Game;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets2;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.TranslatorSystem;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class SheetsWrapper : IPetSheets
{
    readonly DalamudServices DalamudServices;
    readonly IStringHelper StringHelper;

    readonly List<IPetSheetData> petSheetCache = new List<IPetSheetData>();
    readonly string[] nameToClass = [];

    readonly ExcelSheet<Companion>? petSheet;
    readonly ExcelSheet<Pet>? battlePetSheet;
    readonly ExcelSheet<World>? worlds;
    readonly ExcelSheet<Race>? races;
    readonly ExcelSheet<ClassJob>? classJob;
    readonly ExcelSheet<Action>? actions;
    readonly ExcelSheet<TextCommand>? textCommands;

    public SheetsWrapper(ref DalamudServices dalamudServices, IStringHelper helper)
    {
        DalamudServices = dalamudServices;
        StringHelper = helper;

        petSheet = dalamudServices.DataManager.GetExcelSheet<Companion>();
        worlds = dalamudServices.DataManager.GetExcelSheet<World>();
        races = dalamudServices.DataManager.GetExcelSheet<Race>();
        classJob = dalamudServices.DataManager.GetExcelSheet<ClassJob>();
        battlePetSheet = dalamudServices.DataManager.GetExcelSheet<Pet>();
        actions = dalamudServices.DataManager.GetExcelSheet<Action>();
        textCommands = dalamudServices.DataManager.GetExcelSheet<TextCommand>();

        SetupSheetDataCache();

        nameToClass = DalamudServices.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => japaneseNames,
            ClientLanguage.English => englishNames,
            ClientLanguage.German => germanNames,
            ClientLanguage.French => frenchNames,
            _ => englishNames,
        };
    }

    void SetupSheetDataCache()
    {
        if (petSheet != null)
        {
            foreach (Companion companion in petSheet)
            {
                if (companion == null) continue;

                ModelChara? model = companion.Model.Value;
                if (model == null) continue;

                uint companionIndex = companion.RowId;
                int modelID = (int)model.RowId;
                int legacyModelID = (int)model.Model;

                if (legacyModelID == 0) continue;

                string singular = companion.Singular.ToDalamudString().TextValue;

                if (singular.IsNullOrWhitespace()) continue;

                singular = StringHelper.MakeTitleCase(singular);

                string plural = companion.Plural.ToDalamudString().TextValue;
                uint icon = companion.Icon;

                //uint betterIcon = companion.Icon + (uint)64000; // Thats the cuter icon

                uint footstepIcon = companion.Icon + (uint)65000;
                sbyte pronoun = companion.Pronoun;
                string raceName = companion.MinionRace?.Value?.Name ?? Translator.GetLine("...");

                string behaviourName = companion.Behavior.Value?.Name ?? Translator.GetLine("...");
                petSheetCache.Add(new PetSheetData(modelID, legacyModelID, icon, raceName, behaviourName, footstepIcon, pronoun, singular, plural, singular, companionIndex, in DalamudServices));
            }
        }
        if (battlePetSheet == null) return;

        foreach (Pet pet in battlePetSheet)
        {
            if (pet == null) continue;
            uint sheetSkeleton = pet.RowId;

            if (!battlePetRemap.TryGetValue(sheetSkeleton, out int skeleton)) continue;
            if (!petIDToAction.TryGetValue(skeleton, out uint actionID)) continue;

            Action? petAction = GetAction(actionID);
            if (petAction == null) continue;

            ushort petIcon = petAction.Icon;

            string name = pet.Name;
            name = StringHelper.MakeTitleCase(name);

            string cleanedActionName = StringHelper.CleanupActionName(StringHelper.CleanupString(petAction.Name));

            petSheetCache.Add(new PetSheetData(skeleton, -1, petIcon, 0, name, cleanedActionName, petAction.Name, petAction.RowId, in DalamudServices));
        }
    }

    public TextCommand? GetCommand(uint id) => textCommands?.GetRow(id);
    public Action? GetAction(uint actionID) => actions?.GetRow(actionID);

    public string? GetClassName(int id)
    {
        if (classJob == null) return null;
        foreach (ClassJob cls in classJob)
            if (cls.RowId == id)
                return cls.Name;
        return null;
    }

    public string? GetWorldName(ushort worldID)
    {
        if (worlds == null) return null;
        World? world = worlds.GetRow(worldID);
        if (world == null) return null;
        return world.InternalName;
    }

    public IPetSheetData? GetPet(int skeletonID)
    {
        for (int i = 0; i < petSheetCache.Count; i++)
        {
            if (petSheetCache[i].Model == skeletonID)
                return petSheetCache[i];
        }

        return null;
    }

    public int ToSoftSkeleton(int skeletonID, int[] softSkeletons)
    {
        bool canBeSoft = mutatableID.Contains(skeletonID);

        if (!canBeSoft) return skeletonID;

        int index = -1;

        for (int i = 0; i < PluginConstants.BaseSkeletons.Length; i++)
        {
            if (PluginConstants.BaseSkeletons[i] == skeletonID)
            {
                index = i;
                break;
            }
        }
        if (index >= 0 && index < softSkeletons.Length) return softSkeletons[index];

        return skeletonID;
    }

    public IPetSheetData? GetPetFromName(string name)
    {
        int sheetCount = petSheetCache.Count;
        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = petSheetCache[i];
            if (!pet.IsPet(name)) continue;
            return pet;
        }
        return null;
    }

    public int? NameToSoftSkeletonIndex(string name)
    {
        name = name.Trim();
        if (name == string.Empty || name == null) return null;
        for (int i = 0; i < nameToClass.Length; i++)
        {
            string nameToClassUnmodified = nameToClass[i];
            string converted = StringHelper.CleanupActionName(nameToClassUnmodified);

            if (!string.Equals(nameToClassUnmodified, name, System.StringComparison.InvariantCultureIgnoreCase) && !string.Equals(converted, name, System.StringComparison.InvariantCultureIgnoreCase)) continue;
            
            return i;
        }

        return null;
    }

    public int? CastToSoftIndex(uint castId)
    {
        if (castId == 0) return null;
        for (int i = 0; i < castToIndex.Count; i++)
        {
            uint cast = castToIndex[i];
            if (cast != castId) continue;

            return i;
        }
        return null;
    }

    public List<IPetSheetData> GetListFromLine(string line)
    {
        List<IPetSheetData> list = new List<IPetSheetData> ();
        if (line == string.Empty) return list;
        foreach (IPetSheetData pet in petSheetCache)
        {
            if (string.Equals(pet.BaseSingular, line, System.StringComparison.InvariantCultureIgnoreCase))
            {
                list.Add(pet);
                return list;
            }

            if ((!line.Contains(pet.BaseSingular, System.StringComparison.InvariantCultureIgnoreCase) || pet.BaseSingular == string.Empty) &&
                (!line.Contains(pet.BasePlural, System.StringComparison.InvariantCultureIgnoreCase) || pet.BasePlural == string.Empty) &&
                (!line.Contains(pet.ActionName, System.StringComparison.InvariantCultureIgnoreCase) || pet.ActionName == string.Empty)) continue;

            list.Add(pet);
        }

        if (list.Count == 0) return list;

        list.Sort((i1, i2) => i1.LongestIdentifier().CompareTo(i2.LongestIdentifier()));
        list.Reverse();

        return list;
    }

    public IPetSheetData? GetPetFromString(string baseString, in IPettableUser user, bool soft)
    {
        List<IPetSheetData> data = GetListFromLine(baseString);
        if (data.Count == 0) return null;

        IPetSheetData normalPetData = data[0];
        if (normalPetData.Model > -1) return normalPetData;

        if (!soft) return normalPetData;

        return MakeSoft(in user, in normalPetData);
    }

    public IPetSheetData? GetPetFromIcon(long iconID)
    {
        if (iconID == 0 || iconID == long.MaxValue) return null;

        int sheetCount = petSheetCache.Count;
        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = petSheetCache[i];
            if (pet.Icon  != iconID) continue;
            return pet;
        }
        return null;
    }

    public IPetSheetData? GetPetFromAction(uint actionID, in IPettableUser user, bool IsSoft)
    {
        if (actionID == 0 || actionID == uint.MaxValue) return null;

        IPetSheetData? activePet = null;

        int sheetCount = petSheetCache.Count;
        for (int i = 0; i < sheetCount; i++)
        {
            IPetSheetData pet = petSheetCache[i];
            if (!pet.IsAction(actionID)) continue;
            activePet = pet;
            break;
        }
        if (activePet == null) return null;

        if (!IsSoft) return activePet;

        return MakeSoft(in user, in activePet);
    }

    public IPetSheetData? MakeSoft(in IPettableUser user, in IPetSheetData oldData)
    {
        if (oldData.Model >= -1) return oldData;

        int? softIndex = NameToSoftSkeletonIndex(oldData.BasePlural);
        if (softIndex == null) return oldData;

        return GetFromSoftIndex(in user, in oldData, softIndex.Value);
    }

    public IPetSheetData? GetFromSoftIndex(in IPettableUser user, in IPetSheetData oldData, int softIndex)
    {
        int? softSkeleton = user.DataBaseEntry.GetSoftSkeleton(softIndex);
        if (softSkeleton == null) return oldData;

        IPetSheetData? softPetData = GetPet(softSkeleton.Value);
        if (softPetData == null) return oldData;

        return new PetSheetData(softPetData.Model, softPetData.Icon, softPetData.Pronoun, oldData.BaseSingular, oldData.BasePlural, in DalamudServices);
    }

    public bool IsValidBattlePet(int skeleton) => petIDToAction.ContainsKey(skeleton);

    public List<IPetSheetData> GetLegacyPets(int legacyModelID)
    {
        List<IPetSheetData> legacyPets = new List<IPetSheetData>();

        for (int i = 0; i < petSheetCache.Count; i++)
        {
            IPetSheetData data = petSheetCache[i];
            if (data.LegacyModelID != legacyModelID) continue;
            legacyPets.Add(data);
        }

        return legacyPets;
    }

    [System.Obsolete]
    public int[] GetObsoleteIDsFromClass(int classJob)
    {
        if (battlePetToClass.TryGetValue(classJob, out int[]? id)) return id;
        return System.Array.Empty<int>();  
    }

    public List<IPetSheetData> GetMissingPets(List<int> battlePetSkeletons)
    {
        List<IPetSheetData> sheetData = new List<IPetSheetData>();

        foreach(IPetSheetData data in petSheetCache)
        {
            int model = data.Model;
            if (model >= -1) continue;
            if (battlePetSkeletons.Contains(model)) continue;

            sheetData.Add(data);
        }

        return sheetData;
    }

    public readonly Dictionary<uint, int> battlePetRemap = new Dictionary<uint, int>()
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

    public readonly Dictionary<int, uint> petIDToAction = new Dictionary<int, uint>()
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

    public readonly List<int> mutatableID = new List<int>()
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
    readonly string[] englishNames = ["Carbuncle", "Garuda-Egi", "Titan-Egi", "Ifrit-Egi", "Eos"];
    readonly string[] germanNames = ["Karfunkel", "Garuda-Egi", "Titan-Egi", "Ifrit-Egi", "Eos"];
    readonly string[] frenchNames = ["Carbuncle", "Garuda-Egi", "Titan-Egi", "Ifrit-Egi", "Eos"];
    readonly string[] japaneseNames = ["カーバンクル", "ガルーダ・エギ", "タイタン・エギ", "イフリート・エギ", "フェアリー・エオス"];

    [System.Obsolete("Classes have been obsolete since 1.4")]
    public readonly Dictionary<int, int[]> battlePetToClass = new Dictionary<int, int[]>()
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
