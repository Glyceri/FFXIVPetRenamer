using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using PetRenamer.Core.Singleton;
using System.Linq;
using PetRenamer.Core.PettableUserSystem;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class SheetUtils : UtilsRegistryType, ISingletonBase<SheetUtils>
{
    public ExcelSheet<Companion> petSheet { get; set; } = null!;
    ExcelSheet<Pet> battlePetSheet { get; set; } = null!;
    ExcelSheet<World> worlds { get; set; } = null!;
    ExcelSheet<Race> races { get; set; } = null!;
    ExcelSheet<ClassJob> classJob { get; set; } = null!;
    public ExcelSheet<Action> actions { get; set; } = null!;
    ExcelSheet<Map> maps { get; set; } = null!;
    public static SheetUtils instance { get; set; } = null!;

    const int cacheSizes = 25;

    readonly Dictionary<string, bool> lastPets = new Dictionary<string, bool>(cacheSizes + 1);
    readonly Dictionary<int, string> lastBattleIds = new Dictionary<int, string>(cacheSizes + 1);
    readonly Dictionary<int, string> lastIds = new Dictionary<int, string>(cacheSizes + 1);
    readonly Dictionary<string, int> lastNames = new Dictionary<string, int>(cacheSizes + 1);
    List<SerializableNickname> lastList = new List<SerializableNickname>();
    string lastQuerry = string.Empty;

    internal override void OnRegistered()
    {
        petSheet = PluginHandlers.DataManager.GetExcelSheet<Companion>()!;
        worlds = PluginHandlers.DataManager.GetExcelSheet<World>()!;
        races = PluginHandlers.DataManager.GetExcelSheet<Race>()!;
        classJob = PluginHandlers.DataManager.GetExcelSheet<ClassJob>()!;
        battlePetSheet = PluginHandlers.DataManager.GetExcelSheet<Pet>()!;
        actions = PluginHandlers.DataManager.GetExcelSheet<Action>()!;
        maps = PluginHandlers.DataManager.GetExcelSheet<Map>()!;
    }

    public bool PetExistsInANY(string petname)
    {
        if (lastPets.ContainsKey(petname))
            return lastPets[petname];

        lastPets.Add(petname, false);
        if (lastPets.Count > cacheSizes)
            lastPets.Remove(lastPets.Keys.ToArray().First());

        foreach (Pet pet in battlePetSheet)
            if (pet.Name.ToString().Contains(petname))
                return lastPets[petname] = true;

        foreach (Companion pet in petSheet)
            if (pet.Singular.ToString().Contains(petname))
                return lastPets[petname] = true;
            
        return false;
    }

    public Map GetMap(uint id)
    {
        return maps.GetRow(id)!;
    }

    public Action GetAction(uint actionID) => actions?.GetRow(actionID)!;

    public string GetBattlePetName(int id)
    {
        //Look how generous I am. If you send the wrong ID it auto remaps
        if (id > 100) id = RemapUtils.instance.BattlePetSkeletonToNameID(id);
        if (id <= 0) return string.Empty;

        if (lastBattleIds.ContainsKey(id))
            return lastBattleIds[id];

        lastBattleIds.Add(id, string.Empty);
        if (lastBattleIds.Count > cacheSizes)
            lastBattleIds.Remove(lastBattleIds.Keys.ToArray().First());

        foreach (Pet pet in battlePetSheet)
            if (pet.RowId == id)
            {
                string endName = pet.Name;
                lastBattleIds[id] = endName;
                return endName;
            }
        return string.Empty;
    }

    public string GetClassName(int id)
    {
        foreach (ClassJob cls in classJob)
            if (cls.RowId == id)
                return cls.Name;
        return string.Empty;
    }

    public string GetPetName(int id)
    {
        if (lastIds.ContainsKey(id))
            return lastIds[id];

        lastIds.Add(id, string.Empty);
        if (lastIds.Count > cacheSizes)
            lastIds.Remove(lastIds.Keys.ToArray().First());

        foreach (Companion pet in petSheet)
        {
            if (pet == null) continue;

            if (pet.Model.Value!.Model == id)
            {
                string endName = pet.Singular.ToString();
                lastIds[id] = endName;
                return endName;
            }
        }
        return string.Empty;
    }

    public int GetIDFromName(string name)
    {
        if (lastNames.ContainsKey(name))
            return lastNames[name];

        lastNames.Add(name, -1);
        if (lastNames.Count > cacheSizes)
            lastNames.Remove(lastNames.Keys.ToArray().First());

        foreach (Companion pet in petSheet)
        {
            if (pet == null) continue;
            if (pet.Singular.ToString().ToLower().Normalize() == name.ToLower().Normalize())
            {
                int val = pet.Model.Value!.Model;
                lastNames[name] = val;
                return val;
            }
        }

        return -1;
    }

    public List<SerializableNickname> GetThoseThatContain(string querry)
    {
        querry = querry.ToLower().Trim();

        if (lastQuerry == querry) return lastList;
        lastQuerry = querry;

        List<SerializableNickname> serializableNicknames = new List<SerializableNickname>();
        if (querry.Length == 0) return lastList = serializableNicknames;

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;

        foreach (Companion pet in petSheet)
        {
            if (pet == null) continue;
            int petModel = pet.Model.Value!.Model;
            string petName = pet.Singular.ToString();
            string customPetName = string.Empty;

            if (petName.Length == 0) continue;
            if (petModel <= 0) continue;

            if (user != null)
                customPetName = user.SerializableUser.GetNameFor(petName) ?? string.Empty;

            if (petModel.ToString().Contains(querry) || petName.Contains(querry) || (customPetName.Contains(querry) && customPetName.Length != 0))
                serializableNicknames.Add(new SerializableNickname(petModel, customPetName));
        }

        return lastList = serializableNicknames;
    }

    public string GetRace(int r, byte gender)
    {
        foreach (Race race in races)
        {
            if (race == null) continue;

            if (race.RowId == r)
                if (gender == 0) return race.Masculine.ToString();
                else return race.Feminine.ToString();
        }

        return string.Empty;
    }

    public string GetWorldName(ushort worldID) => worlds.GetRow(worldID)?.InternalName ?? string.Empty;
}
