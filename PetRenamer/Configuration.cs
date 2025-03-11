using Dalamud.Configuration;
using Dalamud.Plugin;
using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PN.S;
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
    public const int currentSaveFileVersion = 10;
    public int Version { get; set; } = currentSaveFileVersion;

    public SerializableUserV5[]? SerializableUsersV5 { get; set; } = null;

    // ------------------------- Global Settings -------------------------
    public bool downloadProfilePictures = true;
    public int languageSettings = 0;
    // 0 == All
    // 1 == Only yourself
    // 2 == No colours
    public int showColours = 0;
    // ------------------------------- Pet -------------------------------
    public bool showOnNameplates = true;
    public bool showOnCastbars = true;
    public bool showInBattleChat = true;
    public bool showOnFlyout = true;
    public bool showOnEmotes = true;
    public bool showOnTooltip = true;
    public bool showNamesInMinionBook = true;
    public bool showNamesInActionLog = true;
    public bool useContextMenus = true;
    public bool showOnTargetBars = true;
    public bool showOnPartyList = true;
    public bool showOnIslandPets = true;
    // --------------------------- UI SETTINGS ---------------------------
    public bool showKofiButton = true;
    public bool quickButtonsToggle = true;
    public int listButtonLayout = 0;
    public int minionIconType = 1;
    public bool showIslandWarning = true;

    // ------------------------- Debug SETTINGS --------------------------
    public bool debugModeActive = false;
    public bool openDebugWindowOnStart = false;
    public bool debugShowChatCode = false;

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
        SerializableUsersV5 ??= [];
    }

    public void Save()
    {
        if (currentSaveFileVersion != Version || !isSetup) return;
        SerializableUsersV5 = Database!.SerializeDatabase();
#pragma warning disable CS0618 // Oboslete (Legacy database is supposed to handle obsolete objects
        serializableUsersV3 = LegacyDatabase!.SerializeLegacyDatabase();
#pragma warning restore CS0618 // Obsolete
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
    [Obsolete("Old User Save System. Very innefficient.")]
    public SerializableUserV4[]? SerializableUsersV4 { get; set; } = null;

#pragma warning restore IDE1006

#pragma warning disable CS0618 // Type or member is obsolete
    void LegacyInitialise()
    {
        users ??= Array.Empty<SerializableNickname>();
        serializableUsers ??= Array.Empty<SerializableUser>();
        serializableUsersV2 ??= Array.Empty<SerializableUserV2>();
        serializableUsersV3 ??= Array.Empty<SerializableUserV3>();
        SerializableUsersV4 ??= Array.Empty<SerializableUserV4>();
    }
#pragma warning restore CS0618 // Type or member is obsolete

    #endregion
}
