using Dalamud.Game;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal struct PetSheetData
{
    public int Model { get; private set; }
    public uint Icon { get; private set; }

    public string[] Singular { get; private set; }
    public string[] Plural   { get; private set; }

    public string BaseSingular { get; private set; }
    public string BasePlural { get; private set; }

    public PetSheetData(int Model, uint Icon, sbyte Pronoun, string Singular, string Plural, ref DalamudServices services)
    {
        this.Model = Model;
        this.Icon = Icon;

        ClientLanguage clientLanguage = services.ClientState.ClientLanguage;

        if (clientLanguage == ClientLanguage.German)
        {
            BaseSingular = GermanReplace(Singular, Pronoun);
            BasePlural = GermanReplace(Plural, Pronoun);
        }
        else
        {
            BaseSingular = Singular;
            BasePlural = Plural;
        }

        string[] starterList = GetList(clientLanguage);
        if (clientLanguage == ClientLanguage.German)
        {
            this.Singular = GetGermanNamedList(starterList, Singular);
            this.Plural = GetGermanNamedList(starterList, Plural);
        }
        else
        {
            this.Singular = GetNamedList(starterList, Singular);
            this.Plural = GetNamedList(starterList, Plural);
        }
    }

    string[] GetGermanNamedList(string[] starterList, string suffix)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            list.AddRange(GetNamedList(starterList, GermanReplace(suffix, (sbyte)i)));
        }
        return list.ToArray();
    }

    string[] GetNamedList(string[] baseArray, string suffix)
    {
        string[] newBaseArray = baseArray.ToArray();
        for(int i = 0; i < newBaseArray.Length; i++)
        {
            newBaseArray[i] = newBaseArray[i] + suffix;
        }
        return newBaseArray;
    }

    string GermanReplace(string baseString, sbyte pronoun)
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

    string[] GetList(ClientLanguage clientLanguage) => clientLanguage switch
    {
        ClientLanguage.English => englishStarters,
        ClientLanguage.German => germanStarters,
        ClientLanguage.French => frenchStarters,
        ClientLanguage.Japanese => japaneseStarters,
        _ => englishStarters
    };
}
