using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class SheetsWrapper : IPetSheets
{
    List<PetSheetData> petSheetCache = new List<PetSheetData>();

    public ExcelSheet<Companion>? petSheet { get; init; }
    public ExcelSheet<Pet>? battlePetSheet { get; init; }
    public ExcelSheet<World>? worlds { get; init; }
    public ExcelSheet<Race>? races { get; init; }
    public ExcelSheet<ClassJob>? classJob { get; init; }
    public ExcelSheet<Action>? actions { get; init; }
    public ExcelSheet<TextCommand>? textCommands { get; init; }

    public SheetsWrapper(ref DalamudServices dalamudServices)
    {
        petSheet = dalamudServices.DataManager.GetExcelSheet<Companion>();
        worlds = dalamudServices.DataManager.GetExcelSheet<World>();
        races = dalamudServices.DataManager.GetExcelSheet<Race>();
        classJob = dalamudServices.DataManager.GetExcelSheet<ClassJob>();
        battlePetSheet = dalamudServices.DataManager.GetExcelSheet<Pet>();
        actions = dalamudServices.DataManager.GetExcelSheet<Action>();
        textCommands = dalamudServices.DataManager.GetExcelSheet<TextCommand>();

        SetupSheetDataCache(ref dalamudServices);
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
            petSheetCache.Add(new PetSheetData(skeleton, petIcon, pronoun, name, name, ref dalamudServices));
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

    public readonly Dictionary<uint, int> battlePetRemap = new Dictionary<uint, int>()
    {
        { 6, -407   }, //EOS
        { 7, -408   }, //Selene

        { 1,  -409  }, //Emerald Carbuncle
        { 38, -410  }, //Ruby Carbuncle
        { 2,  -412  }, //Topaz Carbuncle
        { 36, -411  }, //Carbuncle

        { 27, -415  }, //Ifrit-Egi
        { 28, -416  }, //Titan-Egi
        { 29, -417  }, //Garuda-Egi 

        { 8, -1027  }, //Rook Autoturret MCHN
        { 21,-2619  }, //Seraph
        { 18, -2618 }, //Automaton Queen
        { 17, -2621 }, //Esteem DRK

        { 14, -2620 }, //Demi-Phoenix
        { 10, -1930 }, //Demi-Bahamut
        { 31, -3124 }, //Topaz-Titan
        { 32, -3123 }, //Emerald-Garuda
        { 30, -3122 }, //Ruby-Iffrit
    };

    public readonly Dictionary<int, uint> petIDToAction = new Dictionary<int, uint>()
    {
        { -409, 25804 }, //Summon Emerald
        { -410, 25802 }, //Summon Ruby
        { -411, 25798 }, //Summon Carbuncle
        { -412, 25803 }, //Summon Topaz
        { -415, 25805 }, //Summon Ifrit
        { -416, 25806 }, //Summon Titan
        { -417, 25807 }, //Summon Garuda
        { -407, 17215 }, //Summon Eos
        { -408, 17215 }, //Summon Eos
        { -2618, 16501 }, //Automaton Queen
        { -2619, 16545 }, //Summon Seraph
        { -2620, 25831 }, //Summon Phoenix
        { -2621, 16472 }, //Living Shadow
        { -3122, 25838 }, //Summon Ifrit II
        { -3123, 25840 }, //Summon Garuda II
        { -3124, 25839 }, //Summon Titan II
        { -1930, 7427 }, //Summon Bahamut
        { -1027, 2864 }, //Rook Autoturret
    };
}
