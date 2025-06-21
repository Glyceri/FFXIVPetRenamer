using Dalamud.Game;
using PetRenamer.PetNicknames.Services;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.TranslatorSystem;

// Trying to stay away from statics, but in this case it just made MUCH more sense.
internal static class Translator
{
    static DalamudServices DalamudServices = null!;
    static Configuration Configuration = null!;

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
        { "PetRenameNode.Species2", "Battle Pet" },
        { "PetRenameNode.Race", "Type" },
        { "PetRenameNode.Behaviour", "Behaviour" },
        { "PetRenameNode.Nickname", "Nickname" },
        { "PetRenameNode.Edit", "Edit" },
        { "PetRenameNode.Clear", "Clear" },
        { "PetRenameNode.Save", "Save" },
        { "PetRenameNode.Cancel", "Cancel" },
        { "PetRenameNode.PleaseSummonWarningLabel", "Warning" },
        { "PetRenameNode.PleaseSummonWarning", "Please summon a pet" },
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
        { "ClearButton.ColourLabel", "Hold \"Left Ctrl\" + \"Left Shift\" to clear the colour." },
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
        { "Config.Flyout", "Show nicknames on Flyout Text." },
        { "Config.Tooltip", "Show nicknames on Tooltips." },
        { "Config.Notebook", "Show nicknames in the Minion Notebook." },
        { "Config.ActionLog", "Show nicknames in the Action List." },
        { "Config.Targetbar", "Show nicknames for Targets." },
        { "Config.Partylist", "Show nicknames on the Party List." },
        { "Config.ContextMenu", "Allow Context Menus." },
        { "Config.IslandWarning", "Show a warning upon unresolved Island Owner." },
        { "Config.IslandPets", "Show names on Island Pets." },

        { "Kofi.Title", "Ko-fi" },
        { "Kofi.Line1", "This is about real life money." },
        { "Kofi.Line2", "It will be used to my cat toys!" },
        { "Kofi.TakeMe", "Take me" },

        { "Command.Petname", "Opens the Pet Rename window.\n        Type /petname help for information on naming via command." },
        { "Command.Petlist", "Opens the Pet List window." },
        { "Command.PetSettings", "Opens the Settings window." },
        { "Command.PetSharing", "Opens the Sharing window." },
        { "Command.PetTheme", "Opens the Colour Editor window." },

        { "Style.Title.Default", "Default" },

        { "ColourEditorWindow.Name", "Name" },
        { "ColourEditorWindow.Author", "Author" },

        { "ColourSetting.Outline", "Outlines" },
        { "ColourSetting.Outline:Fade", "Outline Fade" },
        { "ColourSetting.Window.Background", "Window Background" },
        { "ColourSetting.Window.BackgroundLight", "Faded Window Background" },
        { "ColourSetting.BackgroundImageColour", "Window Background Image Colour" },
        { "ColourSetting.SearchBarBackground", "Search Bar" },
        { "ColourSetting.ListElementBackground", "List Element Background" },
        { "ColourSetting.Window.TextLight", "Text" },
        { "ColourSetting.Window.TextOutline", "Text Outline" },
        { "ColourSetting.Window.Text", "Text Disabled" },
        { "ColourSetting.Window.TextOutlineButton", "Text Outline Disabled" },
        { "ColourSetting.WindowBorder:Active", "Border Active" },
        { "ColourSetting.WindowBorder:Inactive", "Border Inactive" },
        { "ColourSetting.Button.Background", "Button" },
        { "ColourSetting.Button.Background:Hover", "Button Hover" },
        { "ColourSetting.Button.Background:Inactive", "Button Disabled" },
        { "ColourSetting.FlareImageColour", "Image" },

        { "ColourEditorWindow.Title", "Colour Editor" },
        { "ColourSettings.PresetListHeader", "Presets" },
        { "ColourSettings.Header", "Colour Settings" },

        { "Config.LanguageSettingsBar.Header.Title", "Language Settings (restart plugin to take effect)" },
    };

    static Dictionary<string, string> GermanTranslations = new Dictionary<string, string>()
    {

    };

    static Dictionary<string, string> FrenchTranslations = new Dictionary<string, string>()
    {
        
    };

    static Dictionary<string, string> JapaneseTranslations = new Dictionary<string, string>()
    {
       
    };

    internal static void Initialise(DalamudServices dalamudServices, Configuration configuration)
    {
        DalamudServices = dalamudServices;
        Configuration = configuration;
    }

    internal static string GetLine(string identifier)
    {
        ClientLanguage language = DalamudServices.ClientState.ClientLanguage;

        PetNicknamesLanguage Language = (PetNicknamesLanguage)Configuration.languageSettings;

        if (Language != PetNicknamesLanguage.Default)
        {
            if (Language == PetNicknamesLanguage.English) language = ClientLanguage.English;
            else if (Language == PetNicknamesLanguage.German) language = ClientLanguage.German;
            else if (Language == PetNicknamesLanguage.French) language = ClientLanguage.French;
            else if (Language == PetNicknamesLanguage.Japanese) language = ClientLanguage.Japanese;
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
