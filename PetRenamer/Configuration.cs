using Dalamud.Configuration;
using Dalamud.Plugin;
using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Serialization;
using System;
using System.Text.Json.Serialization;

namespace PetRenamer;

[Serializable]
internal class Configuration : IPluginConfiguration
{
    [JsonIgnore]
    IDalamudPluginInterface? PetNicknamesPlugin;
    [JsonIgnore]
    IPettableDatabase? Database = null;
    [JsonIgnore]
    ILegacyDatabase? LegacyDatabase = null;
    [JsonIgnore]
    bool isSetup = false;
    [JsonIgnore]
    public const int currentSaveFileVersion = 9;
    public int Version { get; set; } = currentSaveFileVersion;

    public SerializableUserV4[]? SerializableUsersV4 { get; set; } = null;

    // ------------------------ Unrelated Settings -----------------------
    public bool limitLocalSearch = false;
    // ------------------------- Global Settings -------------------------
    public bool displayCustomNames = true;
    public bool downloadProfilePictures = false;
    public bool displayImages = true;
    public bool automaticallySwitchPetmode = true;
    public bool disablePVPChatMessage = false;
    // ----------------------- Battle Pet Settings -----------------------
    public bool allowCastBarPet = true;
    public bool useCustomFlyoutPet = true;
    public bool useCustomPetNamesInBattleChat = true;
    public bool useCustomPetNamesInInfoChat = true;
    public bool useContextMenuOnBattlePets = true;
    public bool allowTooltipsBattlePets = true;
    public bool replaceEmotesBattlePets = true;
    // ------------------------- Minion Settings -------------------------
    public bool useContextMenuOnMinions = true;
    public bool allowTooltipsOnMinions = true;
    public bool replaceEmotesOnMinions = true;
    public bool showNamesInMinionBook = true;
    // ---------------------- Sharing Mode Settings ----------------------
    public bool alwaysOpenAdvancedMode = false;
    // --------------------------- UI SETTINGS ---------------------------
    public bool anonymousMode = false;
    public bool spaceOutSettings = false;
    public bool startSettingsOpen = false;
    public bool quickButtonsToggle = false;
    public bool newUseCustomTheme = false;
    public bool showKofiButton = true;
    public string activeElement = "Event";
    public bool hideHelpButton = false;
    public bool hidePetListButton = false;
    // -------------------------- DEBUG SETTINGS -------------------------
    public bool debugMode = false;
    public bool autoOpenDebug = true;
    public bool showChatID = false;

    public void Initialise(IDalamudPluginInterface PetNicknamesPlugin, IPettableDatabase database, ILegacyDatabase legacyDatabase)
    {
        this.PetNicknamesPlugin = PetNicknamesPlugin;
        Database = database;
        LegacyDatabase = legacyDatabase;
        LegacyInitialise();
        CurrentInitialise();
        isSetup = true;
    }

    void CurrentInitialise()
    {
        SerializableUsersV4 ??= Array.Empty<SerializableUserV4>();
    }

    public void Save()
    {
        if (currentSaveFileVersion != Version || !isSetup || Database == null || LegacyDatabase == null) return;
        SerializableUsersV4 = Database.SerializeDatabase();
#pragma warning disable CS0618
        serializableUsersV3 = LegacyDatabase.SerializeLegacyDatabase();
#pragma warning restore CS0618
        PetNicknamesPlugin?.SavePluginConfig(this);

    }

    #region OBSOLETE

#pragma warning disable IDE1006
    //---------------------------Legacy Variables---------------------------
    // Will be kept for backwards compatibility
    //---------------------------Legacy Variables---------------------------
    [Obsolete("NEVER USE THIS VALUE!")]
    public string __Obsolete_Values__ { get; set; } = "\nThe values from here onwards are obsolete, editing these will result in NOTHING";
    [Obsolete("Old nickname Save System. Nowadays nicknames get saved per User")]
    public SerializableNickname[]? users { get; set; } = null;
    [Obsolete("Old User Save System. Very innefficient.")]
    public SerializableUser[]? serializableUsers { get; set; } = null;
    [Obsolete("Old User Save System. Very innefficient.")]
    public SerializableUserV2[]? serializableUsersV2 { get; set; } = null;
    [Obsolete("Old User Save System. Very innefficient. Please... OH PLEASE")]
    public SerializableUserV3[]? serializableUsersV3 = null;
    [Obsolete("Issue fixed. Just keeping it here so I dont accidentally overwrite it later and fock over people with old savefiles :D")]
    public bool usePartyList { get; set; } = false;

    [Obsolete("Use the type specific variable instead.")] public bool replaceEmotes { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool allowTooltips { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool useContextMenus { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool useCustomNamesInChat { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool useCustomFlyoutInChat { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool allowCastBar { get; set; } = true;

#pragma warning restore IDE1006

#pragma warning disable CS0618 // Type or member is obsolete
    void LegacyInitialise()
    {
        users ??= Array.Empty<SerializableNickname>();
        serializableUsers ??= Array.Empty<SerializableUser>();
        serializableUsersV2 ??= Array.Empty<SerializableUserV2>();
        serializableUsersV3 ??= Array.Empty<SerializableUserV3>();
    }
#pragma warning restore CS0618 // Type or member is obsolete

    #endregion
}
