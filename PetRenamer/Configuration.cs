using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;
using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PN.S;
using System;

namespace PetRenamer;

[Serializable]
internal class Configuration : IPluginConfiguration
{
    [JsonIgnore]
    public const int currentSaveFileVersion = 13;

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
    public SerializableUserV3[]? serializableUsersV3 = null;

    // ------------------------- Global Settings -------------------------
    public bool downloadProfilePictures = true;
    public PetNicknamesLanguage currentLanguage = PetNicknamesLanguage.Default;
    public bool showCommandFeedback = true;
    public bool showNotifications = true;
    
    public ColourMode SelectedColourMode = ColourMode.All;
    // ------------------------------- Pet -------------------------------
    
    public ColourConfig ShowOnNameplatesColour      = new ColourConfig();
    public ColourConfig ShowOnCastbarsColour        = new ColourConfig();
    public ColourConfig ShowInBattleChatColour      = new ColourConfig();
    public ColourConfig ShowOnFlyoutColour          = new ColourConfig();
    public ColourConfig ShowOnEmotesColour          = new ColourConfig();
    public ColourConfig ShowOnTooltipColour         = new ColourConfig();
    public ColourConfig ShowNamesInMinionBookColour = new ColourConfig();
    public ColourConfig ShowNamesInActionLogColour  = new ColourConfig();
    public ColourConfig ShowOnTargetBarsColour      = new ColourConfig();
    public ColourConfig ShowOnPartyListColour       = new ColourConfig();
    
    public bool allowPartySummonCutoff = true;
    public bool showOnIslandPets = true;
    public bool useContextMenus  = true;
    
    // --------------------------- UI SETTINGS ---------------------------
    public bool showKofiButton = true;
    public bool quickButtonsToggle = true;
    public int listButtonLayout = 0;
    public int minionIconType = 1;
    public bool showIslandWarning = true;
    public bool oldBarStyleLayout = false;
    public bool showLanguageAsNative = true;
    
    // ------------------------ PENUMBRA SETTINGS ------------------------
    public bool attachToPCP = true;
    public bool readFromPCP = true;

    // --------------------------- STATE PRESERVING ---------------------------
    public ulong LastIslandContentId = ulong.MinValue;
    
    // ------------------------- Debug SETTINGS --------------------------
    public bool debugModeActive        = false;
    public bool openDebugWindowOnStart = false;
    public bool debugShowChatCode      = false;
    public int  lastDebugTab           = 0;
    public bool showFailedTranslations = true;

    public void Initialise(IDalamudPluginInterface petNicknamesPlugin, IPettableDatabase database, ILegacyDatabase legacyDatabase, IPetServices petServices)
    {
        PetNicknamesPlugin = petNicknamesPlugin;

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
        serializableUsersV3 ??= [];
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
    [Obsolete("Old User Save System. Very innefficient.")]
    public SerializableUserV4[]? SerializableUsersV4 { get; set; } = null;
    [Obsolete("Pre Skeleton Type Update")]
    public SerializableUserV5[]? SerializableUsersV5 { get; set; } = null;

    // 0 == All
    // 1 == Only yourself
    // 2 == No colours
    [Obsolete]
    public int showColours = 0;
    [Obsolete]
    public bool showOnNameplates = true;
    [Obsolete]
    public bool showOnCastbars = true;
    [Obsolete]
    public bool showInBattleChat = true;
    [Obsolete]
    public bool showOnFlyout = true;
    [Obsolete]
    public bool showOnEmotes = true;
    [Obsolete]
    public bool showOnTooltip = true;
    [Obsolete]
    public bool showNamesInMinionBook = true;
    [Obsolete]
    public bool showNamesInActionLog = true;
    [Obsolete]
    public bool showOnTargetBars = true;
    [Obsolete]
    public bool showOnPartyList = true;
    [Obsolete]
    public int languageSettings = 0;
    
#pragma warning restore IDE1006

#pragma warning disable CS0618 // Type or member is obsolete
    void LegacyInitialise()
    {
        users               ??= [];
        serializableUsers   ??= [];
        serializableUsersV2 ??= [];
        SerializableUsersV4 ??= [];
        SerializableUsersV5 ??= [];
    }
#pragma warning restore CS0618 // Type or member is obsolete

    #endregion
    
    [Serializable]
    public struct ColourConfig
    {
        public bool       Enabled            = true;
        public bool       OverrideColourMode = false;
        public ColourMode ColourMode         = ColourMode.All;
        
        public ColourConfig()
            { }
        
        public ColourConfig(bool enabled, bool overrideColourMode, ColourMode colourMode)
        {
            Enabled            = enabled;
            OverrideColourMode = overrideColourMode;
            ColourMode         = colourMode;
        }
    }
    
    [Serializable]
    public enum ColourMode
    {
        All      = 0,
        Personal = 1,
        None     = 2,
    }
}
