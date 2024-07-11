using Dalamud.Game;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal struct PetSheetData : IPetSheetData
{
    public int Model { get; private set; }
    public uint Icon { get; private set; }
    public sbyte Pronoun { get; private set; }

    public string[] Singular { get; private set; }
    public string[] Plural { get; private set; }

    public string BaseSingular { get; private set; }
    public string BasePlural { get; private set; }

    public string ActionName { get; private set; } = "";
    public uint ActionID { get; private set; } = 0;

    public int LegacyModelID { get; private set; }

    public string? RaceName { get; private set; } = null;
    public string? BehaviourName { get; private set; } = null;
    public uint FootstepIcon { get; private set; } = 0;

    public PetSheetData(int Model, int legacyModelID, uint Icon, string? raceName, string? behaviourName, uint footstepIcon, sbyte Pronoun, string Singular, string Plural, string actionName, uint actionID, in DalamudServices services)
        : this(Model, legacyModelID, Icon, Pronoun, Singular, Plural, actionName, actionID, in services)
    {
        this.RaceName = raceName;
        this.BehaviourName = behaviourName;
        this.FootstepIcon = footstepIcon;
    }

    public PetSheetData(int Model, int legacyModelID, uint Icon, sbyte Pronoun, string Singular, string Plural, string actionName, uint actionID, in DalamudServices services)
        : this(Model, Icon, Pronoun, Singular, Plural, in services)
    {
        ActionID = actionID;
        ActionName = actionName;
        LegacyModelID = legacyModelID;
    }

    public PetSheetData(int model, uint icon, sbyte pronoun, string singular, string plural, in DalamudServices services)
    {
        Model = model;
        Icon = icon;
        Pronoun = pronoun;

        ClientLanguage clientLanguage = services.ClientState.ClientLanguage;

        if (clientLanguage == ClientLanguage.German)
        {
            BaseSingular = GermanReplace(singular, Pronoun);
            BasePlural = GermanReplace(plural, Pronoun);
        }
        else
        {
            BaseSingular = singular;
            BasePlural = plural;
        }

        string[] starterList = GetList(clientLanguage);
        if (clientLanguage == ClientLanguage.German)
        {
            Singular = GetGermanNamedList(starterList, singular);
            Plural = GetGermanNamedList(starterList, plural);
        }
        else
        {
            Singular = AddSuffixToArray(starterList, singular);
            Plural = AddSuffixToArray(starterList, plural);
        }
    }

    readonly string[] GetGermanNamedList(string[] starterList, string suffix)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            list.AddRange(AddSuffixToArray(starterList, GermanReplace(suffix, (sbyte)i)));
        }
        return list.ToArray();
    }

    readonly string[] AddSuffixToArray(string[] baseArray, string suffix)
    {
        string[] newBaseArray = baseArray.ToArray();
        for (int i = 0; i < newBaseArray.Length; i++)
        {
            newBaseArray[i] = newBaseArray[i] + suffix;
        }
        return newBaseArray;
    }

    readonly string GermanReplace(string baseString, sbyte pronoun)
    {
        try
        {
            baseString = baseString.Replace("[p]", "");
            baseString = baseString.Replace("[a]", checked(pronounList[pronoun]));
            return baseString;
        }
        catch
        {
            return baseString;
        }
    }

    readonly string[] pronounList = ["er", "e", "es", "en"];
    readonly string[] englishStarters = ["the ", string.Empty];
    readonly string[] germanStarters = ["den ", "des ", "dem ", "die ", "der ", "das ", string.Empty];
    readonly string[] frenchStarters = ["le ", "la ", string.Empty];
    readonly string[] japaneseStarters = [string.Empty];

    readonly string[] GetList(ClientLanguage clientLanguage) => clientLanguage switch
    {
        ClientLanguage.English => englishStarters,
        ClientLanguage.German => germanStarters,
        ClientLanguage.French => frenchStarters,
        ClientLanguage.Japanese => japaneseStarters,
        _ => englishStarters
    };

    public readonly bool IsPet(string name)
    {
        if (name == string.Empty || name == null) return false;
        return string.Equals(BaseSingular, name, System.StringComparison.InvariantCultureIgnoreCase);
    }

    public readonly bool IsAction(string action)
    {
        if (action == string.Empty || action == null) return false;
        return string.Equals(ActionName, action, System.StringComparison.InvariantCultureIgnoreCase);
    }

    public readonly bool Contains(string line)
    {
        for (int i = 0; i < Singular.Length; i++)
            if (line.Contains(Singular[i], System.StringComparison.InvariantCultureIgnoreCase))
                return true;

        for (int i = 0; i < Plural.Length; i++)
            if (line.Contains(Plural[i], System.StringComparison.InvariantCultureIgnoreCase))
                return true;

        return false;
    }

    public readonly string LongestIdentifier()
    {
        string curIdentifier = BaseSingular;
        if (curIdentifier.Length < BasePlural.Length) curIdentifier = BasePlural;
        if (curIdentifier.Length < ActionName.Length) curIdentifier = ActionName;
        return curIdentifier;
    }

    public readonly bool IsAction(uint action) => ActionID == action;
}
