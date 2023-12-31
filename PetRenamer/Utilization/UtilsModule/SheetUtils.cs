using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Singleton;
using PetRenamer.Logging;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using System.Linq;

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
    ExcelSheet<TextCommand> textCommands { get; set; } = null!;
    public static SheetUtils instance { get; set; } = null!;

    const int cacheSizes = 55;

    readonly Dictionary<string, bool> lastPets = new Dictionary<string, bool>(cacheSizes + 1);
    readonly Dictionary<int, string> lastBattleIds = new Dictionary<int, string>(cacheSizes + 1);
    readonly Dictionary<(int, NameType), string> lastIds = new Dictionary<(int, NameType), string>(cacheSizes + 1);
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
        textCommands = PluginHandlers.DataManager.GetExcelSheet<TextCommand>()!;
    }

    //226 is /egiglamour
    //33 is /petmirage
    public TextCommand GetCommand(uint id) => textCommands.GetRow(id)!;

    public bool PetExistsInANY(string petname)
    {
        if(lastPets.TryGetValue(petname, out bool exists))
            return exists;

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

    public Action GetAction(uint actionID) => actions?.GetRow(actionID)!;

    public string GetBattlePetName(int id)
    {
        //Look how generous I am. If you send the wrong ID it auto remaps
        if (id > 100) id = RemapUtils.instance.BattlePetSkeletonToNameID(-id);
        else if (id <= 0) return string.Empty;
        if (id < -1) id = -id;

        if(lastBattleIds.TryGetValue(id, out string? battleName))
            return battleName;

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

    public string GetPetName(int id, NameType nameType = NameType.Singular)
    {
        if (lastIds.TryGetValue((id, nameType), out string? petName))
            return petName;

        lastIds.Add((id, nameType), string.Empty);
        if (lastIds.Count > cacheSizes)
            lastIds.Remove(lastIds.Keys.ToArray().First());

        if (id < -1)
        {
            string tempName = RemapUtils.instance.PetIDToName(id);
            if (tempName != string.Empty)
            {
                lastIds[(id, nameType)] = tempName;
                return tempName;
            }
        }

        foreach (Companion pet in petSheet)
        {
            if (pet == null) continue;

            if (pet.Model.Value!.RowId == id)
            {
                string endName = nameType == NameType.Singular ? pet.Singular.ToString() : pet.Plural.ToString();
                lastIds[(id, nameType)] = endName;
                return endName;
            }
        }
        return string.Empty;
    }

    public int GetIDFromName(string name)
    {
        if(lastNames.TryGetValue(name, out int id)) 
            return id;

        lastNames.Add(name, -1);
        if (lastNames.Count > cacheSizes)
            lastNames.Remove(lastNames.Keys.ToArray().First());

        foreach (Companion pet in petSheet)
        {
            if (pet == null) continue;
            if (pet.Singular.ToString().ToLower().Normalize() == name.ToLower().Normalize())
            {
                int val = (int)pet.Model.Value!.RowId;
                lastNames[name] = val;
                return val;
            }
        }

        return -1;
    }

    public List<SerializableNickname> GetThoseThatContain(string querry, bool forceRecheck)
    {
        querry = querry.ToLower().Trim();

        if (lastQuerry == querry && !forceRecheck) return lastList;
        lastQuerry = querry;

        List<SerializableNickname> serializableNicknames = new List<SerializableNickname>();
        if (querry.Length == 0) return lastList = serializableNicknames;

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return serializableNicknames;
        foreach (Companion pet in petSheet)
        {
            if (pet == null) continue;
            uint petModel = pet.Model.Value!.RowId;
            string petName = pet.Singular.ToString();

            if (petName.Length == 0) continue;
            if (petModel <= 0) continue;

            string customPetName = user.SerializableUser.GetNameFor(petName) ?? string.Empty;

            if (petModel.ToString().Contains(querry, System.StringComparison.CurrentCultureIgnoreCase) || petName.Contains(querry, System.StringComparison.CurrentCultureIgnoreCase) || (customPetName.Contains(querry, System.StringComparison.CurrentCultureIgnoreCase) && customPetName.Length != 0))
                if ((PluginLink.Configuration.limitLocalSearch && user.SerializableUser.Contains((int)petModel)) || !PluginLink.Configuration.limitLocalSearch)
                    serializableNicknames.Add(new SerializableNickname((int)petModel, customPetName));
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

public enum NameType
{
    Singular,
    Plural
}
