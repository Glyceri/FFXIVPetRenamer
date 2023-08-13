using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core;
using System.Linq;
using PetRenamer.Core.Singleton;
using System.Text.RegularExpressions;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class SheetUtils : UtilsRegistryType, ISingletonBase<SheetUtils>
{
    ExcelSheet<Lumina.Excel.GeneratedSheets.Companion> petSheet { get; set; } = null!;
    ExcelSheet<Pet> battlePetSheet { get; set; } = null!;
    ExcelSheet<World> worlds { get; set; } = null!;
    ExcelSheet<Race> races { get; set; } = null!;
    ExcelSheet<Tribe> tribe { get; set; } = null!;
    ExcelSheet<ClassJob> classJob { get; set; } = null!;
    public static SheetUtils instance { get; set; } = null!;

    internal override void OnRegistered()
    {
        petSheet = PluginHandlers.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Companion>()!;
        worlds = PluginHandlers.DataManager.GetExcelSheet<World>()!;
        races = PluginHandlers.DataManager.GetExcelSheet<Race>()!;
        tribe = PluginHandlers.DataManager.GetExcelSheet<Tribe>()!;
        classJob = PluginHandlers.DataManager.GetExcelSheet<ClassJob>()!;
        battlePetSheet = PluginHandlers.DataManager.GetExcelSheet<Pet>()!;
    }

    public unsafe string GetCurrentClassName()
    {
        PlayerData? playerData = PlayerUtils.instance.GetPlayerData();
        if (playerData == null) return string.Empty;

        return GetClassName(((Character*)playerData.Value.playerGameObject)->CharacterData.ClassJob);
    }

    public unsafe string GetBattlePetName(int id)
    {
        //Look how generous I am. If you send the wrong ID it auto remaps
        if(id > 100) id = RemapUtils.instance.BattlePetSkeletonToNameID(id);
        if (id <= 0) return string.Empty;

        foreach(Pet pet in battlePetSheet)
            if(pet.RowId == id)
                return pet.Name;

        return string.Empty;
    }

    public string GetClassName(int id)
    {
        foreach (ClassJob cls in classJob)
            if(cls.RowId == id)
                return cls.Name;
        return string.Empty;
    }

    public string GetCurrentBattlePetName()
    {
        PlayerData? playerData = PlayerUtils.instance.GetPlayerData();
        if (playerData == null) return string.Empty;
        if (!PluginConstants.allowedJobs.Contains(playerData.Value.job)) return string.Empty;

        return GetBattlePetName(RemapUtils.instance.GetPetIDFromClass(playerData.Value.job));
    }

    public string GetCurrentPetName()
    {
        PlayerData? playerData = PlayerUtils.instance.GetPlayerData();
        if (playerData == null) return string.Empty;
        if (playerData!.Value.companionData == null) return string.Empty;

        return GetPetName(playerData!.Value.companionData!.Value.currentModelID);
    }

    public string GetPetName(int id)
    {
        foreach (Lumina.Excel.GeneratedSheets.Companion pet in petSheet)
        {
            if (pet == null) continue;

            if (pet.Model.Value!.Model == id)
                return pet.Singular.ToString();
        }
        return string.Empty;
    }

    public int GetIDFromName(string name)
    {
        foreach (Lumina.Excel.GeneratedSheets.Companion pet in petSheet)
        {
            if(pet == null) continue;
            if (pet.Singular.ToString().ToLower().Normalize() == name.ToLower().Normalize())
                return pet.Model.Value!.Model;
        }

        return -1;
    }

    public List<SerializableNickname> GetThoseThatContain(string querry)
    {
        querry = querry.ToLower().Trim();
        List<SerializableNickname> serializableNicknames = new List<SerializableNickname>();
        if(querry.Length == 0) return serializableNicknames;

        foreach (Lumina.Excel.GeneratedSheets.Companion pet in petSheet)
        {
            if (pet == null) continue;
            int petModel = pet.Model.Value!.Model;
            string petName = pet.Singular.ToString();

            if (petModel <= 0) continue;
            if (petName.Length == 0) continue;

            if (petModel.ToString().Contains(querry) || petName.Contains(querry))
                serializableNicknames.Add(new SerializableNickname(petModel, petName));
        }

        return serializableNicknames;
    }

    public string GetRace(int r, byte gender)
    {
        foreach(Race race in races)
        {
            if(race == null) continue;

            if (race.RowId == r)
                if (gender == 0) return race.Masculine.ToString();
                else return race.Feminine.ToString();
        }

        return string.Empty;
    }

    public string GetWorldName(ushort worldID)
    {
        foreach(World world in worlds)
        {
            if (world == null) continue;

            if (world.RowId == worldID)
                return world.Name.ToString();
        }
        return string.Empty;
    }

    public string GetTribe(int tribeID, byte gender)
    {
        foreach(Tribe t in tribe)
        {
            if(t == null) continue;
            if (t.RowId == tribeID)
                if (gender == 0)
                    return t.Masculine.ToString();
                else t.Feminine.ToString();
        }
        return string.Empty; 
    }

    public string GetGender(byte gender)
    {
        if (gender == 0) return "Male";
        if (gender == 1) return "Female";

        return "No Gender";
    }
}
