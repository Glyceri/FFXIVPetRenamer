using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;
using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PN.S;
using System;

namespace PetRenamer;

[Serializable]
internal class Configuration : IPluginConfiguration
{
    [JsonIgnore]
    public const int currentSaveFileVersion = 11;

    [JsonIgnore]
    private IDalamudPluginInterface? PetNicknamesPlugin;
    [JsonIgnore]
    private IPettableDatabase? Database = null;
    [JsonIgnore]
    private ILegacyDatabase? LegacyDatabase = null;
    [JsonIgnore]
    private IPetServices? PetServices = null;
    [JsonIgnore]
    private bool isSetup = false;

    public int Version { get; set; } = currentSaveFileVersion;

    public SerializableUserV6[]? SerializableUsersV6 { get; set; } = null;

    // ------------------------- Global Settings -------------------------
    public bool downloadProfilePictures = true;
    public int languageSettings = 0;
    // 0 == All
    // 1 == Only yourself
    // 2 == No colours
    public int showColours = 0;
    public bool showCommandFeedback = true;
    public bool showNotifications = true;
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

    // ------------------------ PENUMBRA SETTINGS ------------------------
    public bool attachToPCP = true;
    public bool readFromPCP = true;

    // ------------------------- Debug SETTINGS --------------------------
    public bool debugModeActive        = false;
    public bool openDebugWindowOnStart = false;
    public bool debugShowChatCode      = false;

    public void Initialise(IDalamudPluginInterface PetNicknamesPlugin, IPettableDatabase database, ILegacyDatabase legacyDatabase, IPetServices petServices)
    {
        this.PetNicknamesPlugin = PetNicknamesPlugin;

        Database        = database;
        LegacyDatabase  = legacyDatabase;
        PetServices     = petServices;

        LegacyInitialise();
        CurrentInitialise();

        isSetup = true;
    }

    private void CurrentInitialise()
    {
        SerializableUsersV6 ??= [];
    }

    public void Save()
    {
        if (currentSaveFileVersion != Version || !isSetup)
        {
            return;
        }

        PetServices?.PetLog.LogVerbose("Pet Nicknames will now attempt to save");   // I need to add more verbose logging, pet nicknames is kind of silent.

        SerializableUsersV6 = Database!.SerializeDatabase();

#pragma warning disable CS0618 // Oboslete (Legacy database is supposed to handle obsolete objects
        serializableUsersV3 = LegacyDatabase!.SerializeLegacyDatabase();
#pragma warning restore CS0618 // Obsolete

        try
        {
            PetNicknamesPlugin?.SavePluginConfig(this);
        }
        catch (Exception ex)
        {
            PetServices?.PetLog.LogError(ex, "Pet Nicknames failed to save. This is actually a bit of a problem :bceStare2: and I am sorry if this causes issues :c");
        }
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
    [Obsolete("Pre Skeleton Type Update")]
    public SerializableUserV5[]? SerializableUsersV5 { get; set; } = null;

#pragma warning restore IDE1006

#pragma warning disable CS0618 // Type or member is obsolete
    void LegacyInitialise()
    {
        users               ??= [];
        serializableUsers   ??= [];
        serializableUsersV2 ??= [];
        serializableUsersV3 ??= [];
        SerializableUsersV4 ??= [];
        SerializableUsersV5 ??= [];
    }
#pragma warning restore CS0618 // Type or member is obsolete

    #endregion
}
