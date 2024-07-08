using Dalamud.Game;
using PetRenamer.PetNicknames.Services;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.TranslatorSystem;

internal static class Translator
{
    static DalamudServices DalamudServices = null!;

    static PetNicknamesLanguage OverridenLanguage = PetNicknamesLanguage.Default;

    static Dictionary<string, string> EnglishTranslations = new Dictionary<string, string>()
    {
        { "PetRenameNode.Species", "Species" },
        { "PetRenameNode.Race", "Race" },
        { "PetRenameNode.Behaviour", "Behaviour" },
        { "PetRenameNode.Nickname", "Nickname" },
        { "PetRenameNode.Edit", "Edit" },
        { "PetRenameNode.Clear", "Clear" },
        { "PetRenameNode.Save", "Save" },
        { "PetRenameNode.Cancel", "Cancel" },
    };

    static Dictionary<string, string> GermanTranslations = new Dictionary<string, string>()
    {
        { "PetRenameNode.Species", "Spezies" },
        { "PetRenameNode.Race", "Rasse" },
        { "PetRenameNode.Behaviour", "Verhalten" },
        { "PetRenameNode.Nickname", "Spitzname" },
        { "PetRenameNode.Edit", "Bearbeiten" },
        { "PetRenameNode.Clear", "Löschen" },
        { "PetRenameNode.Save", "Speichern" },
        { "PetRenameNode.Cancel", "Abbrechen" },
    };

    static Dictionary<string, string> FrenchTranslations = new Dictionary<string, string>()
    {
        
    };

    static Dictionary<string, string> JapaneseTranslations = new Dictionary<string, string>()
    {
        
    };

    internal static void Initialise(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;
    }

    internal static void OverrideLanguage(PetNicknamesLanguage petNicknamesLanguage)
    {
        OverridenLanguage = petNicknamesLanguage;
    }

    internal static string GetLine(string identifier)
    {
        ClientLanguage language = DalamudServices.ClientState.ClientLanguage;

        if (OverridenLanguage != PetNicknamesLanguage.Default)
        {
            if (OverridenLanguage == PetNicknamesLanguage.English) language = ClientLanguage.English;
            else if (OverridenLanguage == PetNicknamesLanguage.German) language = ClientLanguage.German;
            else if (OverridenLanguage == PetNicknamesLanguage.French) language = ClientLanguage.French;
            else if (OverridenLanguage == PetNicknamesLanguage.Japanese) language = ClientLanguage.Japanese;
        }

        if (language == ClientLanguage.German) return GetTranslation(ref GermanTranslations, identifier);
        if (language == ClientLanguage.French) return GetTranslation(ref FrenchTranslations, identifier);
        if (language == ClientLanguage.Japanese) return GetTranslation(ref JapaneseTranslations, identifier);

        return GetTranslation(ref EnglishTranslations, identifier);
    }

    static string GetTranslation(ref Dictionary<string, string> translationDictionary, string identifier)
    {
        if (translationDictionary.TryGetValue(identifier, out string? translation)) return translation;
        if (EnglishTranslations.TryGetValue(identifier, out string? englishTranslations)) return englishTranslations;

        return identifier;
    }
}

internal enum PetNicknamesLanguage
{
    Default,
    English,
    German,
    French,
    Japanese,
}
