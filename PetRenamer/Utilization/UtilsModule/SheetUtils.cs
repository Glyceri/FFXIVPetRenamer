using Dalamud.Utility;
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
    public List<PNCompanion> petSheet = new List<PNCompanion>();
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
        worlds = PluginHandlers.DataManager.GetExcelSheet<World>()!;
        races = PluginHandlers.DataManager.GetExcelSheet<Race>()!;
        classJob = PluginHandlers.DataManager.GetExcelSheet<ClassJob>()!;
        battlePetSheet = PluginHandlers.DataManager.GetExcelSheet<Pet>()!;
        actions = PluginHandlers.DataManager.GetExcelSheet<Action>()!;
        textCommands = PluginHandlers.DataManager.GetExcelSheet<TextCommand>()!;

        ExcelSheet<Companion> petSheet = PluginHandlers.DataManager.GetExcelSheet<Companion>()!;
        foreach (Companion companion in petSheet)
        {
            if (companion == null) continue;
            this.petSheet.Add(new PNCompanion(companion.Model!.Value!.RowId, companion.Singular.ToDalamudString().TextValue, companion.Plural.ToDalamudString().TextValue, companion.Icon, companion.Pronoun));
        }
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

        foreach (PNCompanion pet in petSheet)
            if (pet.Singular.Contains(petname))
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

        foreach (PNCompanion pet in petSheet)
        {
            if (pet.Model == id)
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

        foreach (PNCompanion pet in petSheet)
        {
            if (pet.Singular.ToLower().Normalize() == name.ToLower().Normalize())
            {
                int val = (int)pet.Model;
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
        foreach (PNCompanion pet in petSheet)
        {
            uint petModel = pet.Model;
            string petName = pet.Singular;

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

    public string GetWorldName(ushort worldID)
    {
        try
        {
            World? world = worlds.GetRow(worldID);
            if (world == null) return string.Empty;
            return world.InternalName;
        }catch { PetLog.Log("ERROR!");  return string.Empty; }
    }
}

public enum NameType
{
    Singular,
    Plural
}

public struct PNCompanion
{
    public readonly uint Model;
    public readonly string Singular;
    public readonly string Plural;
    public readonly uint Icon;

    public PNCompanion(uint Model, string Singular, string Plural, uint Icon, sbyte pronoun)
    {
        this.Model = Model;
        this.Singular = SanitizeString(Singular, pronoun);
        this.Plural = SanitizeString(Plural, pronoun);
        this.Icon = Icon;
    }

    string SanitizeString(string baseString, sbyte pronoun)
    {
        try
        {
            checked
            {
                string newString = baseString;
                if (PluginHandlers.ClientState.ClientLanguage == Dalamud.ClientLanguage.German)
                {
                    newString = newString.Replace("[p]", "");
                    if (newString.Contains("[a]")) 
                        newString = newString.Replace("[a]", pronounList[pronoun]);
                }
                return newString;
            }
        }
        catch 
        { 
            return baseString;
        }
    }

    string[] pronounList = new string[]
    {
        "er",
        "e",
        "es",
        "en"
    };
}
