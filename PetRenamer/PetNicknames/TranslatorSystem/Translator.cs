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
        { "PetRenameNode.IsRenamed", "Your {0} has the nickname:" },
        { "PetRenameNode.IsNotRenamed", "Your {0} has no nickname." },
        { "PetRenameNode.SaveNickname", "Save Nickname" },
        { "PetRenameNode.ClearNickname", "Clear Nickname" },
    };

    static Dictionary<string, string> GermanTranslations = new Dictionary<string, string>()
    {
        { "PetRenameNode.IsRenamed", "Ihr {0} trägt den Spitznamen:" },
        { "PetRenameNode.IsNotRenamed", "Ihr {0} hat keinen Spitznamen." },
        { "PetRenameNode.SaveNickname", "Spitznamen Speichern" },
        { "PetRenameNode.ClearNickname", "Spitznamen Löschen" },
    };

    static Dictionary<string, string> FrenchTranslations = new Dictionary<string, string>()
    {
        
    };

    static Dictionary<string, string> JapaneseTranslations = new Dictionary<string, string>()
    {
        { "PetRenameNode.IsRenamed", "{0}のニックネームは" },
        { "PetRenameNode.IsNotRenamed", "{0}にはニックネームがない" },
        { "PetRenameNode.SaveNickname", "ニックネームをセーブ" },
        { "PetRenameNode.ClearNickname", "ニックネームを削除" },
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
