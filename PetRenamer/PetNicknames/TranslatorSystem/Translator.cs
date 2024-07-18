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
        { "PVPWarning", "Pet Nicknames is disabled in PVP zones excluding the Wolves'Den Pier." },

        { "ShareWindow.Export", "Export to Clipboard" },
        { "ShareWindow.Import", "Import from Clipboard" },
        { "ShareWindow.ExportError", "No data available.\nYou need to log in to a character to export your data." },
        { "ShareWindow.ExportSuccess", "Data successfully copied." },
        { "ShareWindow.ImportError", "Failed to import data:\n{0}" },
        { "ShareWindow.ImportSuccess", "Successfully imported data from {0}" },

        { "Config.Title", "Settings" },
        { "Config.Header.GeneralSettings", "General Settings" },
        { "Config.Header.UISettings", "UI Settings" },
        { "Config.Header.NativeSettings", "Native Settings" },
        { "Config.PVPMessage", "Disable PVP warning message." },
        { "Config.ProfilePictures", "Automatically download profile pictures." },
        { "Config.UISettings.UIScale.Header.Title", "Custom UI Scale" },
        { "Config.Toggle", "Quick Buttons toggle instead of open." },
        { "Config.Kofi", "Show Ko-fi Button." },
        { "Config.TransparentBackground", "Background goes transparent on inactivity." },
        { "Config.UIFlare", "Show extra UI decorations." },
        { "Config.Nameplate", "Show nicknames on Nameplates." },
        { "Config.Castbar", "Show nicknames on Cast bars." },
        { "Config.BattleChat", "Show nicknames in the Battle Chat." },
        { "Config.Emote", "Show nicknames for Emotes." },
        { "Config.Tooltip", "Show nicknames on Tooltips." },
        { "Config.Notebook", "Show nicknames in the Minion Notebook." },
        { "Config.ActionLog", "Show nicknames in the Action List." },
        { "Config.Targetbar", "Show nicknames for Targets." },
        { "Config.Partylist", "Show nicknames on the Party List." },
        { "Config.ContextMenu", "Allow Context Menus." },

        { "Kofi.Title", "Ko-fi" },
        { "Kofi.Line1", "This is about real life money." },
        { "Kofi.Line2", "It will be used to buy dog toys!" },
        { "Kofi.TakeMe", "Take me" },

        { "Command.Petname", "Open the Pet Rename window." },
        { "Command.Petlist", "Open the Pet List window." },
        { "Command.PetSettings", "Open the Settings window." },
        { "Command.PetSharing", "Open the Sharing window." },
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
        { "PVPWarning", "„Pet Nicknames“ ist in PVP-Zonen mit Ausnahme des Wolfshöhlen-Pier deaktiviert." },
        { "ShareWindow.Export", "Exportieren" },
        { "ShareWindow.Import", "Importieren" },
        { "ShareWindow.ExportError", "Keine Daten verfügbar.\nSie müssen sich mit einem Charakter anmelden, um Ihre Daten zu exportieren." },
        { "ShareWindow.ExportSuccess", "Daten erfolgreich kopiert." },
        { "ShareWindow.ImportError", "Fehler beim Importieren der Daten:\n{0}" },
        { "ShareWindow.ImportSuccess", "Daten erfolgreich importiert von {0}" },
        { "Config.Title", "Einstellungen" },
        { "Config.Header.GeneralSettings", "Allgemeine Einstellungen" },
        { "Config.Header.UISettings", "UI Einstellungen" },
        { "Config.Header.NativeSettings", "Native Einstellungen" },
        { "Config.PVPMessage", "PVP-Warnmeldung deaktivieren." },
        { "Config.ProfilePictures", "Profilbilder automatisch herunterladen." },
        { "Config.UISettings.UIScale.Header.Title", "Benutzerdefinierte UI-Skalierung" },
        { "Config.Toggle", "Schnelltasten werden umgeschaltet statt geöffnet." },
        { "Config.Kofi", "Ko-Fi-Button anzeigen." },
        { "Config.TransparentBackground", "Der Hintergrund wird bei Inaktivität transparent." },
        { "Config.UIFlare", "Zusätzliche UI-Dekorationen anzeigen." },
        { "Config.Nameplate", "Spitznamen auf „Nameplate“ anzeigen." },
        { "Config.Castbar", "Spitznamen auf „Castbar“ anzeigen." },
        { "Config.BattleChat", "Spitznamen im „Battle-Chat“ anzeigen." },
        { "Config.Emote", "Spitznamen für „Emotes“ anzeigen." },
        { "Config.Tooltip", "Spitznamen in „Tooltips“ anzeigen." },
        { "Config.Notebook", "Spitznamen im „Begleiter-Verzeichnis“ anzeigen." },
        { "Config.ActionLog", "Spitznamen in der „Kommandoliste“ anzeigen." },
        { "Config.Targetbar", "Spitznamen für Ziele anzeigen." },
        { "Config.Partylist", "Spitznamen auf der „Partyliste“ anzeigen." },
        { "Config.ContextMenu", "Kontextmenüs zulassen." },

        { "Kofi.Title", "Ko-fi" },
        { "Kofi.Line1", "Hier geht es um echtes Geld." },
        { "Kofi.Line2", "Es wird für den Kauf von Hundespielzeug verwendet!" },
        { "Kofi.TakeMe", "Los geht's" },

        { "Command.Petname", "Öffnet das Fenster „Haustierausweis“." },
        { "Command.Petlist", "Öffnet das Fenster „Spitznamenliste“." },
        { "Command.PetSettings", "Öffnet das Fenster „Einstellungen“." },
        { "Command.PetSharing", "Öffnet das Fenster „Teilen“." },
    };


    static Dictionary<string, string> FrenchTranslations = new Dictionary<string, string>()
    {
        { "...", "..." },
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
