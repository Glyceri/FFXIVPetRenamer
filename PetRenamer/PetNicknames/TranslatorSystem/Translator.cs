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

        { "Command.Petname", "Opens the Pet Rename window." },
        { "Command.Petlist", "Opens the Pet List window." },
        { "Command.PetSettings", "Opens the Settings window." },
        { "Command.PetSharing", "Opens the Sharing window." },
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
        { "Name", "Nom d'utilisateur" },
        { "Homeworld", "Monde d'origine" },
        { "Petcount", "Nombre de surnoms" },
        { "Search", "Chercher" },
        { "DateTime.Unkown", "Date inconnue" },
        { "Version.Unkown", "Version inconnue" },
        { "ContextMenu.Rename", "Donner un surnom" },
        { "PetRenameNode.Species", "Mascotte" },
        { "PetRenameNode.Race", "Race" },
        { "PetRenameNode.Behaviour", "Comportement" },
        { "PetRenameNode.Nickname", "Surnom" },
        { "PetRenameNode.Edit", "Modifier" },
        { "PetRenameNode.Clear", "Effacer" },
        { "PetRenameNode.Save", "Enregistrer" },
        { "PetRenameNode.Cancel", "Annuler" },
        { "WindowHandler.Title", "Passeport pour animaux" },
        { "PetList.Title", "Liste de surnoms" },
        { "PetList.Navigation", "Navigation" },
        { "PetList.UserList", "Liste des utilisateurs" },
        { "PetList.MyList", "Ma liste" },
        { "PetList.Sharing", "Partager" },
        { "PetListWindow.ListHeaderPersonalMinion", "Vos mascottes" },
        { "PetListWindow.ListHeaderPersonalBattlePet", "Vos animaux de combat" },
        { "PetListWindow.ListHeaderOtherMinion", "Mascottes de {0}" },
        { "PetListWindow.ListHeaderOtherBattlePet", "Animaux de combat de {0}" },
        { "ClearButton.Label", "Maintenez les touches « Ctrl gauche » + « Maj gauche »\npour supprimer une entrée." },
        { "UserListElement.WarningClear", "Vous ne pouvez pas vous retirer." },
        { "UserListElement.WarningIPC", "Cet utilisateur est temporairement ajouté via un plugin externe et n'est pas enregistré." },
        { "UserListElement.WarningOldUser", "Cet utilisateur provient de votre ancien fichier de sauvegarde.\nRencontrez-le dans le jeu pour le mettre à jour." },
        { "PVPWarning", "« Pet Nicknames » est désactivé dans les zones PVP, sauf sur la Jetée des l'Antre des Loups." },
        { "ShareWindow.Export", "Exporter" },
        { "ShareWindow.Import", "Importer" },
        { "ShareWindow.ExportError", "Aucune donnée disponible.\nVous devez vous connecter avec un personnage pour exporter vos données." },
        { "ShareWindow.ExportSuccess", "Données copiées avec succès." },
        { "ShareWindow.ImportError", "Erreur lors de l'importation des données :\n{0}" },
        { "ShareWindow.ImportSuccess", "Données importées avec succès de {0}" },
        { "Config.Title", "Paramètres" },
        { "Config.Header.GeneralSettings", "Paramètres généraux" },
        { "Config.Header.UISettings", "Paramètres de l'interface utilisateur" },
        { "Config.Header.NativeSettings", "Paramètres natifs" },
        { "Config.PVPMessage", "Désactiver le message d'avertissement PVP." },
        { "Config.ProfilePictures", "Télécharger automatiquement les photos de profil." },
        { "Config.UISettings.UIScale.Header.Title", "Échelle personnalisée de l'interface utilisateur" },
        { "Config.Toggle", "Les raccourcis sont basculés au lieu d'être ouverts." },
        { "Config.Kofi", "Afficher le bouton Ko-Fi." },
        { "Config.TransparentBackground", "L'arrière-plan devient transparent en cas d'inactivité." },
        { "Config.UIFlare", "Afficher des décorations supplémentaires de l'interface utilisateur." },
        { "Config.Nameplate", "Afficher les surnoms sur la « Nameplate »." },
        { "Config.Castbar", "Afficher les surnoms sur la « Castbar »." },
        { "Config.BattleChat", "Afficher les surnoms dans le chat de combat." },
        { "Config.Emote", "Afficher les surnoms pour « Emote »." },
        { "Config.Tooltip", "Afficher les surnoms dans les infobulles." },
        { "Config.Notebook", "Afficher les surnoms dans le carnet de Mascottes." },
        { "Config.ActionLog", "Afficher les surnoms dans la liste de Actions et traits." },
        { "Config.Targetbar", "Afficher les surnoms pour les « Target »." },
        { "Config.Partylist", "Afficher les surnoms sur la « Party List »." },
        { "Config.ContextMenu", "Autoriser les menus contextuels." },

        { "Kofi.Title", "Ko-fi" },
        { "Kofi.Line1", "Ceci concerne de l'argent réel." },
        { "Kofi.Line2", "Il est utilisé pour acheter des jouets pour chiens !" },
        { "Kofi.TakeMe", "Allons-y" },

        { "Command.Petname", "Ouvre la fenêtre « Passeport pour animaux »." },
        { "Command.Petlist", "Ouvre la fenêtre « Liste de surnoms »." },
        { "Command.PetSettings", "Ouvre la fenêtre « Paramètres »." },
        { "Command.PetSharing", "Ouvre la fenêtre « Partager »." },
    };

    static Dictionary<string, string> JapaneseTranslations = new Dictionary<string, string>()
{
    { "...", "。。。" },
    { "Name", "ユーザーネーム" },
    { "Homeworld", "ホームワールド" },
    { "Petcount", "ニックネーム数" },
    { "Search", "検索" },
    { "DateTime.Unkown", "日付不明" },
    { "Version.Unkown", "バージョン不明" },
    { "ContextMenu.Rename", "ニックネームを付ける" },
    { "PetRenameNode.Species", "ミニオン" },
    { "PetRenameNode.Race", "種" },
    { "PetRenameNode.Behaviour", "行動" },
    { "PetRenameNode.Nickname", "ニックネーム" },
    { "PetRenameNode.Edit", "編集" },
    { "PetRenameNode.Clear", "クリア" },
    { "PetRenameNode.Save", "セーブ" },
    { "PetRenameNode.Cancel", "キャンセル" },
    { "WindowHandler.Title", "ペットパスポート" },
    { "PetList.Title", "ニックネームリスト" },
    { "PetList.Navigation", "ナビゲーション" },
    { "PetList.UserList", "ユーザーリスト" },
    { "PetList.MyList", "私のリスト" },
    { "PetList.Sharing", "共有" },
    { "PetListWindow.ListHeaderPersonalMinion", "あなたのミニオン" },
    { "PetListWindow.ListHeaderPersonalBattlePet", "あなたの戦闘ペット" },
    { "PetListWindow.ListHeaderOtherMinion", "{0}のミニオン" },
    { "PetListWindow.ListHeaderOtherBattlePet", "{0}の戦闘ペット" },
    { "ClearButton.Label", "エントリーを削除するには、「左Ctrl」+「左Shift」キーを押し続けてください。" },
    { "UserListElement.WarningClear", "削除できない。" },
    { "UserListElement.WarningIPC", "このユーザーは外部プラグインを介して一時的に追加され、保存されません。" },
    { "UserListElement.WarningOldUser", "このユーザーは古いバックアップファイルから来ています。更新するにはゲーム内で彼に会ってください。" },
    { "PVPWarning", "「Pet Nicknames」は、ウルヴズジェイル係船場くPVPゾーンでは無効です。" },
    { "ShareWindow.Export", "エクスポート" },
    { "ShareWindow.Import", "インポート" },
    { "ShareWindow.ExportError", "利用可能なデータがありません。データをエクスポートするには、キャラクターでログインする必要があります。" },
    { "ShareWindow.ExportSuccess", "データが正常にコピーされました。" },
    { "ShareWindow.ImportError", "データのインポートエラー：\n{0}" },
    { "ShareWindow.ImportSuccess", "{0}からのデータが正常にインポートされました。" },
    { "Config.Title", "設定" },
    { "Config.Header.GeneralSettings", "一般設定" },
    { "Config.Header.UISettings", "UI設定" },
    { "Config.Header.NativeSettings", "プラグイン設定" },
    { "Config.PVPMessage", "PVP警告メッセージを無効にする。" },
    { "Config.ProfilePictures", "プロフィール写真を自動ダウンロードする。" },
    { "Config.UISettings.UIScale.Header.Title", "カスタムUIスケーリング" },
    { "Config.Toggle", "ショートカットボタンを切り替える" },
    { "Config.Kofi", "Ko-Fiボタンを表示する。" },
    { "Config.TransparentBackground", "非アクティブ時に背景を透明にする。" },
    { "Config.UIFlare", "追加のUIデコレーションを表示する。" },
    { "Config.Nameplate", "ネームプレートにニックネームを表示する。" },
    { "Config.Castbar", "キャストバーにニックネームを表示する。" },
    { "Config.BattleChat", "バトルチャットにニックネームを表示する。" },
    { "Config.Emote", "エモートにニックネームを表示する。" },
    { "Config.Tooltip", "ツールチップにニックネームを表示する。" },
    { "Config.Notebook", "ミニオンリストにニックネームを表示する。" },
    { "Config.ActionLog", "アクションログにニックネームを表示する。" },
    { "Config.Targetbar", "ターゲットバーにニックネームを表示する。" },
    { "Config.Partylist", "パーティリストにニックネームを表示する。" },
    { "Config.ContextMenu", "コンテキストメニューを許可する。" },

    { "Kofi.Title", "Ko-fi" },
    { "Kofi.Line1", "これは実際のお金に関することです。" },
    { "Kofi.Line2", "それは犬のおもちゃの購入に使用されます！" },
    { "Kofi.TakeMe", "行きましょう" },

    { "Command.Petname", "「ペットパスポート」ウィンドウを開く。" },
    { "Command.Petlist", "「ニックネームリスト」ウィンドウを開く。" },
    { "Command.PetSettings", "「設定」ウィンドウを開く。" },
    { "Command.PetSharing", "「共有」ウィンドウを開く。" },
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
