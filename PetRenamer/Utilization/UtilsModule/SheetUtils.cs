using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class SheetUtils : UtilsRegistryType
{
    ExcelSheet<Companion> petSheet { get; set; } = null!;
    ExcelSheet<Pet> battlePetSheet { get; set; } = null!;
    ExcelSheet<World> worlds { get; set; } = null!;
    ExcelSheet<Race> races { get; set; } = null!;
    ExcelSheet<Tribe> tribe { get; set; } = null!;

    internal override void OnRegistered()
    {
        petSheet = PluginHandlers.DataManager.GetExcelSheet<Companion>()!;
        worlds = PluginHandlers.DataManager.GetExcelSheet<World>()!;
        races = PluginHandlers.DataManager.GetExcelSheet<Race>()!;
        tribe = PluginHandlers.DataManager.GetExcelSheet<Tribe>()!;
        battlePetSheet = PluginHandlers.DataManager.GetExcelSheet<Pet>()!;
    }

    public void GetCurrentBattlePetName()
    {
        foreach(Pet pet in battlePetSheet) 
        { 
            Dalamud.Logging.PluginLog.Log(pet.Name + " : " + pet.RowId);
        }
    }

    public string GetCurrentPetName()
    {
        PlayerData? playerData = PluginLink.Utils.Get<PlayerUtils>().GetPlayerData();
        if (playerData == null) return string.Empty;
        if (playerData!.Value.companionData == null) return string.Empty;

        return GetPetName(playerData!.Value.companionData!.Value.currentModelID);
    }

    public string GetPetName(int id)
    {
        foreach (Companion pet in petSheet)
        {
            if (pet == null) continue;

            if (pet.Model.Value!.Model == id)
                return pet.Singular.ToString();
        }
        return string.Empty;
    }

    public List<SerializableNickname> GetThoseThatContain(string querry)
    {
        querry = querry.ToLower().Trim();
        List<SerializableNickname> serializableNicknames = new List<SerializableNickname>();
        if(querry.Length == 0) return serializableNicknames;

        foreach (Companion pet in petSheet)
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
