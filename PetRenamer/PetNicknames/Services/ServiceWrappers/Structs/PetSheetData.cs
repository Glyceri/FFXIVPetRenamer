using Dalamud.Game;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal struct PetSheetData : IPetSheetData
{
    public int      Model         { get; private set; }
    public uint     Icon          { get; private set; }
    public sbyte    Pronoun       { get; private set; }

    public string[] Singular      { get; private set; }
    public string[] Plural        { get; private set; }

    public string   BaseSingular  { get; private set; }
    public string   BasePlural    { get; private set; }

    public string   ActionName    { get; private set; } = string.Empty;
    public uint     ActionID      { get; private set; } = 0;

    public int      LegacyModelID { get; private set; }

    public uint     RaceID        { get; private set; } = 0;
    public string?  RaceName      { get; private set; } = null;
    public string?  BehaviourName { get; private set; } = null;
    public uint     FootstepIcon  { get; private set; } = 0;
    

    public PetSheetData(int Model, int legacyModelID, uint Icon, string? raceName, uint raceID, string? behaviourName, sbyte Pronoun, string Singular, string Plural, string actionName, uint actionID, in DalamudServices services)
        : this(Model, legacyModelID, Icon, Pronoun, Singular, Plural, actionName, actionID, in services)
    {
        RaceName      = raceName;
        BehaviourName = behaviourName;
        RaceID        = raceID;
    }

    public PetSheetData(int Model, int legacyModelID, uint Icon, sbyte Pronoun, string Singular, string Plural, string actionName, uint actionID, in DalamudServices services)
        : this(Model, Icon, Pronoun, Singular, Plural, in services)
    {
        ActionID      = actionID;
        ActionName    = actionName;
        LegacyModelID = legacyModelID;
    }

    public PetSheetData(int model, uint icon, sbyte pronoun, string singular, string plural, in DalamudServices services)
    {
        Model   = model;
        Icon    = icon;
        Pronoun = pronoun;

        ClientLanguage clientLanguage = services.ClientState.ClientLanguage;

        if (clientLanguage == ClientLanguage.German)
        {
            BaseSingular = GermanReplace(singular, Pronoun);
            BasePlural   = GermanReplace(plural, Pronoun);
        }
        else
        {
            BaseSingular = singular;
            BasePlural   = plural;
        }

        string[] starterList = GetList(clientLanguage);

        if (clientLanguage == ClientLanguage.German)
        {
            string[] sArr = GetGermanNamedList(starterList, singular);
            string[] pArr = GetGermanNamedList(starterList, plural);

            string[] tempSArr = sArr;
            string[] tempPArr = pArr;

            tempSArr = AddSuffixToArray(tempSArr, "s");
            tempPArr = AddSuffixToArray(tempPArr, "s");

            Singular = [.. sArr, .. tempSArr];
            Plural   = [.. pArr, .. tempPArr];
        }
        else
        {
            Singular = AddSuffixToArray(starterList, singular);
            Plural   = AddSuffixToArray(starterList, plural);
        }
    }

    private readonly string[] GetGermanNamedList(string[] starterList, string suffix)
    {
        List<string> list = new List<string>();

        for (int i = 0; i < 4; i++)
        {
            string   germanReplacedString = GermanReplace(suffix, (sbyte)i);

            string[] arrayWithSuffixes    = AddSuffixToArray(starterList, germanReplacedString);

            list.AddRange(arrayWithSuffixes);
        }

        return list.ToArray();
    }

    private readonly string[] AddSuffixToArray(string[] baseArray, string suffix)
    {
        string[] newBaseArray = [.. baseArray];

        for (int i = 0; i < newBaseArray.Length; i++)
        {
            newBaseArray[i] = newBaseArray[i] + suffix;
        }

        return newBaseArray;
    }

    private readonly string GermanReplace(string baseString, sbyte pronoun)
    {
        if (pronoun < 0 || pronoun >= pronounList.Length)
        {
            return baseString;
        }

        baseString = baseString.InvariantReplace("[p]", string.Empty);
        baseString = baseString.InvariantReplace("[a]", pronounList[pronoun]);   

        return baseString;
    }

    private readonly string[] pronounList      = ["er", "e", "es", "en"];
    private readonly string[] englishStarters  = ["the ", string.Empty];
    private readonly string[] germanStarters   = ["den ", "des ", "dem ", "die ", "der ", "das ", string.Empty];
    private readonly string[] frenchStarters   = ["le ", "la ", "l'", string.Empty];
    private readonly string[] japaneseStarters = [string.Empty];

    private readonly string[] GetList(ClientLanguage clientLanguage) 
        => clientLanguage switch
        {
            ClientLanguage.English  => englishStarters,
            ClientLanguage.German   => germanStarters,
            ClientLanguage.French   => frenchStarters,
            ClientLanguage.Japanese => japaneseStarters,
            _ => englishStarters
        };

    public readonly bool IsPet(string name)
        => BaseSingular.InvariantEquals(name);

    public readonly bool IsAction(string action)
        => ActionName.InvariantEquals(action);

    public readonly bool Contains(string line)
    {
        if (line.InvariantContains(Singular))
        {
            return true;
        }

        if (line.InvariantContains(Plural))
        {
            return true;
        }

        return false;
    }

    public readonly string LongestIdentifier()
    {
        string curIdentifier = BaseSingular;

        if (curIdentifier.Length < BasePlural.Length)
        {
            curIdentifier = BasePlural;
        }

        if (curIdentifier.Length < ActionName.Length)
        {
            curIdentifier = ActionName;
        }

        return curIdentifier;
    }

    public readonly bool IsAction(uint action) 
        => (ActionID == action);
}
