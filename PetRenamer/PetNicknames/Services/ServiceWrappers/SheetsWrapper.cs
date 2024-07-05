﻿using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text.Payloads;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class SheetsWrapper : IPetSheets
{
    DalamudServices services;
    IStringHelper helper;

    readonly List<PetSheetData> petSheetCache = new List<PetSheetData>();
    readonly List<string> nameToClass = new List<string>();

    public ExcelSheet<Companion>? petSheet { get; init; }
    public ExcelSheet<Pet>? battlePetSheet { get; init; }
    public ExcelSheet<World>? worlds { get; init; }
    public ExcelSheet<Race>? races { get; init; }
    public ExcelSheet<ClassJob>? classJob { get; init; }
    public ExcelSheet<Action>? actions { get; init; }
    public ExcelSheet<TextCommand>? textCommands { get; init; }

    public SheetsWrapper(ref DalamudServices dalamudServices, IStringHelper helper)
    {
        services = dalamudServices;
        this.helper = helper;
        petSheet = dalamudServices.DataManager.GetExcelSheet<Companion>();
        worlds = dalamudServices.DataManager.GetExcelSheet<World>();
        races = dalamudServices.DataManager.GetExcelSheet<Race>();
        classJob = dalamudServices.DataManager.GetExcelSheet<ClassJob>();
        battlePetSheet = dalamudServices.DataManager.GetExcelSheet<Pet>();
        actions = dalamudServices.DataManager.GetExcelSheet<Action>();
        textCommands = dalamudServices.DataManager.GetExcelSheet<TextCommand>();

        SetupSheetDataCache(ref dalamudServices);
        SetupSoftSkeletonIndexList();
    }

    void SetupSheetDataCache(ref DalamudServices dalamudServices)
    {
        if (petSheet != null)
        {
            foreach (Companion companion in petSheet)
            {
                if (companion == null) continue;
                ModelChara? model = companion.Model.Value;
                if (model == null) continue;
                int modelID = (int)model.RowId;
                string singular = companion.Singular.ToDalamudString().TextValue;
                string plural = companion.Plural.ToDalamudString().TextValue;
                ushort icon = companion.Icon;
                sbyte pronoun = companion.Pronoun;
                petSheetCache.Add(new PetSheetData(modelID, icon, pronoun, singular, plural, ref dalamudServices));
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
            sbyte pronoun = 0;
            string name = pet.Name;
            string cleanedActionName = helper.CleanupString(helper.CleanupActionName(petAction.Name));
            petSheetCache.Add(new PetSheetData(skeleton, petIcon, pronoun, name, cleanedActionName, petAction.Name, petAction.RowId, ref dalamudServices));
        }
    }

    // The reason I do it like this is because I need the EXACT /petmirage names
    // If nothing changed (And quite frankly nothing has in YEARS)
    // This should result in a list that looks like this
    // 0 --> Carbuncle
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos
    void SetupSoftSkeletonIndexList()
    {
        TextCommand? command = GetCommand(33);
        if (command == null) return;
        int counter = 0;

        for (int i = 2; i < command.Description.Payloads.Count; i++)
        {
            BasePayload secondTolastPayload = command.Description.Payloads[i - 2];
            BasePayload lastPayload = command.Description.Payloads[i - 1];
            BasePayload curPayload = command.Description.Payloads[i];

            if (!secondTolastPayload.PayloadType.HasFlag(PayloadType.UiColorFill)) continue;
            if (!lastPayload.PayloadType.HasFlag(PayloadType.UiColorBorder)) continue;
            if (!curPayload.PayloadType.HasFlag(PayloadType.Text)) continue;

            string payloadString = (curPayload as TextPayload)!.RawString;

            if (counter < 5) { nameToClass.Add(payloadString); }
            else break;
            counter++;
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

    public PetSheetData? GetPet(int skeletonID)
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

    public PetSheetData? GetPetFromName(string name)
    {
        int sheetCount = petSheetCache.Count;
        for (int i = 0; i < sheetCount; i++)
        {
            PetSheetData pet = petSheetCache[i];
            if (!pet.IsPet(name)) continue;
            return pet;
        }
        return null;
    }

    public PetSheetData? GetPetFromActionName(string actionName)
    {
        int sheetCount = petSheetCache.Count;
        for (int i = 0; i < sheetCount; i++)
        {
            PetSheetData pet = petSheetCache[i];
            if (!pet.IsAction(actionName)) continue;
            return pet;
        }
        return null;
    }

    public int? NameToSoftSkeletonIndex(string name)
    {
        for (int i = 0; i < nameToClass.Count; i++)
        {
            if (!string.Equals(nameToClass[i], name, System.StringComparison.InvariantCultureIgnoreCase)) continue;
            return i;
        }
        return null;
    }

    public int? CastToSoftIndex(uint castId)
    {
        for (int i = 0; i < castToIndex.Count; i++)
        {
            uint cast = castToIndex[i];
            if (cast == castId) return i;
        }
        return null;
    }

    public List<PetSheetData> GetListFromLine(string line)
    {
        List<PetSheetData> list = new List<PetSheetData> ();
        if (line == string.Empty) return list;
        foreach (PetSheetData pet in petSheetCache)
        {
            if (string.Equals(pet.BaseSingular, line, System.StringComparison.InvariantCultureIgnoreCase))
            {
                list.Add(pet);
                return list;
            }

            if (pet.BaseSingular == string.Empty || pet.BasePlural == string.Empty || pet.ActionName == string.Empty) continue;

            if (line.Contains(pet.BaseSingular, System.StringComparison.InvariantCultureIgnoreCase) ||
                line.Contains(pet.BasePlural, System.StringComparison.InvariantCultureIgnoreCase) ||
                line.Contains(pet.ActionName, System.StringComparison.InvariantCultureIgnoreCase)) list.Add(pet);
        }    

        return list;
    }

    public PetSheetData? GetPetFromString(string baseString, ref IPettableUser user, bool soft)
    {
        List<PetSheetData> data = GetListFromLine(baseString);

        if (data.Count == 0) return null;

        data.Sort((i1, i2) => i1.BaseSingular.CompareTo(i2.BaseSingular));
        data.Reverse();

        PetSheetData normalPetData = data[0];

        if (!soft) return normalPetData;

        int? softIndex = NameToSoftSkeletonIndex(normalPetData.BasePlural);
        if (softIndex == null) return normalPetData;

        int? softSkeleton = user.DataBaseEntry.GetSoftSkeleton(softIndex.Value);
        if (softSkeleton == null) return normalPetData;

        PetSheetData? softPetData = GetPet(softSkeleton.Value);
        if (softPetData == null) return normalPetData;

        return new PetSheetData(softPetData.Value.Model, softPetData.Value.Icon, softPetData.Value.Pronoun, normalPetData.BaseSingular, normalPetData.BasePlural, ref services);
    }

    public readonly Dictionary<uint, int> battlePetRemap = new Dictionary<uint, int>()
    {
        { 6,    PluginConstants.Eos                     }, //EOS
        { 7,    PluginConstants.Selene                  }, //Selene

        { 1,    PluginConstants.EmeraldCarbuncle        }, //Emerald Carbuncle
        { 38,   PluginConstants.RubyCarbuncle           }, //Ruby Carbuncle
        { 2,    PluginConstants.TopazCarbuncle          }, //Topaz Carbuncle
        { 36,   PluginConstants.Carbuncle               }, //Carbuncle

        { 27,   PluginConstants.IfritEgi                }, //Ifrit-Egi
        { 28,   PluginConstants.TitanEgi                }, //Titan-Egi
        { 29,   PluginConstants.GarudaEgi               }, //Garuda-Egi 

        { 8,    PluginConstants.RookAutoTurret          }, //Rook Autoturret MCHN
        { 21,   PluginConstants.Seraph                  }, //Seraph
        { 18,   PluginConstants.AutomatonQueen          }, //Automaton Queen
        { 17,   PluginConstants.LivingShadow            }, //Esteem DRK

        { 14,   PluginConstants.Phoenix                 }, //Demi-Phoenix
        { 10,   PluginConstants.Bahamut                 }, //Demi-Bahamut
        { 31,   PluginConstants.TitanII                 }, //Topaz-Titan
        { 32,   PluginConstants.GarudaII                }, //Emerald-Garuda
        { 30,   PluginConstants.IffritII                }, //Ruby-Iffrit
    };

    public readonly Dictionary<int, uint> petIDToAction = new Dictionary<int, uint>()
    {
        { PluginConstants.EmeraldCarbuncle      , 25804 }, //Summon Emerald
        { PluginConstants.RubyCarbuncle         , 25802 }, //Summon Ruby
        { PluginConstants.Carbuncle             , 25798 }, //Summon Carbuncle
        { PluginConstants.TopazCarbuncle        , 25803 }, //Summon Topaz
        { PluginConstants.IfritEgi              , 25805 }, //Summon Ifrit
        { PluginConstants.TitanEgi              , 25806 }, //Summon Titan
        { PluginConstants.GarudaEgi             , 25807 }, //Summon Garuda
        { PluginConstants.Eos                   , 17215 }, //Summon Eos
        { PluginConstants.Selene                , 17215 }, //Summon Eos
        { PluginConstants.AutomatonQueen        , 16501 }, //Automaton Queen
        { PluginConstants.Seraph                , 16545 }, //Summon Seraph
        { PluginConstants.Phoenix               , 25831 }, //Summon Phoenix
        { PluginConstants.LivingShadow          , 16472 }, //Living Shadow
        { PluginConstants.IffritII              , 25838 }, //Summon Ifrit II
        { PluginConstants.GarudaII              , 25840 }, //Summon Garuda II
        { PluginConstants.TitanII               , 25839 }, //Summon Titan II
        { PluginConstants.Bahamut               , 7427  }, //Summon Bahamut
        { PluginConstants.RookAutoTurret        , 2864  }, //Rook Autoturret
    };

    public readonly List<int> mutatableID = new List<int>()
    {
        PluginConstants.Eos                 , //Eos
        PluginConstants.Selene              , //Selene
        PluginConstants.EmeraldCarbuncle    , //Emerald Carbuncle
        PluginConstants.RubyCarbuncle       , //Ruby Carbuncle
        PluginConstants.Carbuncle           , //Carbuncle
        PluginConstants.TopazCarbuncle      , //Topaz Carbuncle
        PluginConstants.IfritEgi            , //Ifrit-Egi
        PluginConstants.TitanEgi            , //Titan-Egi
        PluginConstants.GarudaEgi             //Garuda-Egi
    };

    // Sheets wrapper explains why the order is like this... it's crucial it stays like this.
    // Soft Mapping is the most hardcoded thing in this plogon :c
    // 0 --> Karfunkel
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos

    public static readonly IReadOnlyList<uint> castToIndex = new List<uint>()
    {
        25798,   // Summon Carbuncle
        25806,   // Summon Garuda
        25807,   // Summon Titan
        25805,   // Summon Ifrit
        17215,   // Summon Eos
    };
}
