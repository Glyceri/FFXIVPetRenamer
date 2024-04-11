using Dalamud.Configuration;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using PetRenamer.Theming.Themes;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PetRenamer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    [JsonIgnore]
    public const int currentSaveFileVersion = 8;
    public int Version { get; set; } = currentSaveFileVersion;

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

    // ------------------------------ MAPPY ------------------------------
    public bool enableMappyIntegration = true;
    public bool readMappyIntegration = false;

    // ----------------------- PERFORMANCE SETTINGS ----------------------

    public SerializableUserV3[]? serializableUsersV3 = null;

    public BaseTheme  CustomBaseTheme = null!;
    public RedTheme   CustomRedTheme = null!;
    public GreenTheme CustomGreenTheme = null!;

    public void Initialize()
    {
        LegacyInitialize();
        CurrentInitialize();
    }

    void CurrentInitialize()
    {
        serializableUsersV3 ??= Array.Empty<SerializableUserV3>();
        CustomBaseTheme ??= new BaseTheme();
        CustomRedTheme ??= new RedTheme();
        CustomGreenTheme ??= new GreenTheme();
    }

    public void Save() 
    {
        if (currentSaveFileVersion < Version) return;
        List<SerializableUserV3> users = new List<SerializableUserV3>();

        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
            if(user is not PettableIPCUser)
                users.Add(user.SerializableUser);

        serializableUsersV3 = users.ToArray();
        PluginLink.DalamudPlugin.SavePluginConfig(this); 
    }

    #region OBSOLETE

    //---------------------------Legacy Variables---------------------------
    // Will be kept for backwards compatibility
    //---------------------------Legacy Variables---------------------------
    [Obsolete("NEVER USE THIS VALUE!")]
    public string __Obsolete_Values__ { get; set; } = "\nThe values from here onwards are obsolete, editing these will result in NOTHING";
    [Obsolete("Old nickname Save System. Nowadays nicknames get saved per User")] 
    public SerializableNickname[]? users { get; set; } = null;
    [Obsolete("Old User Save System. Very innefficient. Use SerializableUserV2 now")]
    public SerializableUser[]? serializableUsers { get; set; } = null;
    [Obsolete("Old User Save System. Very innefficient. Use SerializableUserV3 now")]
    public SerializableUserV2[]? serializableUsersV2 { get; set; } = null;
    [Obsolete("Issue fixed. Just keeping it here so I dont accidentally overwrite it later and fock over people with old savefiles :D")]
    public bool usePartyList { get; set; } = false;

    [Obsolete("Use the type specific variable instead.")] public bool replaceEmotes { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool allowTooltips { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool useContextMenus { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool useCustomNamesInChat { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool useCustomFlyoutInChat { get; set; } = true;
    [Obsolete("Use the type specific variable instead.")] public bool allowCastBar { get; set; } = true;

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
    void LegacyInitialize()
    {
        users ??= Array.Empty<SerializableNickname>();
        serializableUsers ??= Array.Empty<SerializableUser>();
        serializableUsersV2 ??= Array.Empty<SerializableUserV2>();
    }
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete

    #endregion
}
