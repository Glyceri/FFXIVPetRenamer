using Dalamud.Game;
using PetRenamer.PetNicknames.Services;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.TranslatorSystem;

// Trying to stay away from statics, but in this case it just made MUCH more sense.
internal static class Translator
{
    static DalamudServices DalamudServices = null!;

    static PetNicknamesLanguage OverridenLanguage = PetNicknamesLanguage.Default;

    static Dictionary<string, string> EnglishTranslations = new Dictionary<string, string>()
    {
        { "...", "..." },
        { "Name", "Username" },
        { "Homeworld", "Homeworld" },
        { "Petcount", "Nicknames" },
        { "Search", "Search" },
        { "DateTime.Unkown", "Date Unknown" },
        { "Version.Unkown", "Version Unknown" },
        { "ContextMenu.Rename", "Give Nickname" },
        { "PetRenameNode.Species", "Minion" },
        { "PetRenameNode.Race", "Race" },
        { "PetRenameNode.Behaviour", "Behaviour" },
        { "PetRenameNode.Nickname", "Nickname" },
        { "PetRenameNode.Edit", "Edit" },
        { "PetRenameNode.Clear", "Clear" },
        { "PetRenameNode.Save", "Save" },
        { "PetRenameNode.Cancel", "Cancel" },
        { "WindowHandler.Title", "Pet Passport" },
        { "PetList.Title", "Pet List" },
        { "PetList.Navigation", "Navigation" },
        { "PetList.UserList", "User List" },
        { "PetList.MyList", "My List" },
        { "PetList.Sharing", "Sharing" },
        { "PetListWindow.ListHeaderPersonalMinion", "Your Minions" },
        { "PetListWindow.ListHeaderPersonalBattlePet", "Your Battle Pets" },
        { "PetListWindow.ListHeaderOtherMinion", "{0}'s Minions" },
        { "PetListWindow.ListHeaderOtherBattlePet", "{0}'s Battle Pets" },
        { "ClearButton.Label", "Hold \"Left Ctrl\" + \"Left Shift\" to delete an entry." },
        { "UserListElement.WarningClear", "You cannot clear yourself." },
        { "UserListElement.WarningIPC", "This user is temporarily added via an\nexternal plugin and will not be saved." },
        { "UserListElement.WarningOldUser", "This user is from your old save file.\nPlease meet them in game so it can update." },
        { "PVPWarning", "Pet Nicknames is disabled in PVP zones excluding the Wolves'Den Pier." }
    };

    static Dictionary<string, string> GermanTranslations = new Dictionary<string, string>()
    {
        { "...", "..." },
        { "Name", "Nutzername" },
        { "Homeworld", "Stammwelt" },
        { "Petcount", "Spitznamenanzahl" },
        { "Search", "Durchsuchung" },
        { "DateTime.Unkown", "Datum unbekannt" },
        { "Version.Unkown", "Version unbekannt" },
        { "ContextMenu.Rename", "Spitznamen vergeben" },
        { "PetRenameNode.Species", "Begleiter" },
        { "PetRenameNode.Race", "Rasse" },
        { "PetRenameNode.Behaviour", "Verhalten" },
        { "PetRenameNode.Nickname", "Spitzname" },
        { "PetRenameNode.Edit", "Bearbeiten" },
        { "PetRenameNode.Clear", "Löschen" },
        { "PetRenameNode.Save", "Speichern" },
        { "PetRenameNode.Cancel", "Abbrechen" },
        { "WindowHandler.Title", "Heimtierausweis" },
        { "PetList.Title", "Spitznamenliste" },
        { "PetList.Navigation", "Navigation" },
        { "PetList.UserList", "Benutzerliste" },
        { "PetList.MyList", "Meine Liste" },
        { "PetList.Sharing", "Teilen" },
        { "PetListWindow.ListHeaderPersonalMinion", "Ihre Begleiter" },
        { "PetListWindow.ListHeaderPersonalBattlePet", "Ihre Kampftiere" },
        { "PetListWindow.ListHeaderOtherMinion", "Begleiter von {0}" },
        { "PetListWindow.ListHeaderOtherBattlePet", "Kampftiere von {0}" },
        { "ClearButton.Label", "Halten Sie die Tasten „Linke Strg“ + „Linke Umschalttaste“ gedrückt,\num einen Eintrag zu löschen." },
        { "UserListElement.WarningClear", "Sie können sich nicht selbst entfernen." },
        { "UserListElement.WarningIPC", "Dieser Benutzer wird über ein externes Plugin\nvorübergehend hinzugefügt und nicht gespeichert." },
        { "UserListElement.WarningOldUser", "Dieser Benutzer stammt aus Ihrer alten Sicherungsdatei.\nTreffen Sie ihn im Spiel, damit es aktualisiert wird." },
        { "PVPWarning", "„Pet Nicknames“ ist in PVP-Zonen mit Ausnahme des Wolfshöhlen-Pier deaktiviert." }
    };

    static Dictionary<string, string> FrenchTranslations = new Dictionary<string, string>()
    {

    };

    static Dictionary<string, string> JapaneseTranslations = new Dictionary<string, string>()
    {
        { "...", "。。。" },
        { "DateTime.Unkown", "不明" },
        { "Version.Unkown", "不明" },
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
