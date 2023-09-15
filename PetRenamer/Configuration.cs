using Dalamud.Configuration;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PetRenamer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    [JsonIgnore]
    public const int currentSaveFileVersion = 6;

    public int Version { get; set; } = 6;

    public bool understoodWarningThirdPartySettings = false;
    // ------------------------- Global Settings -------------------------
    public bool displayCustomNames = true;
    public bool useCustomTheme = true;
    public bool downloadProfilePictures = false;
    public bool displayImages = true;
    // ----------------------- Battle Pet Settings -----------------------
    public bool allowCastBarPet = true;
    public bool useCustomFlyoutPet = true;
    public bool useCustomPetNamesInBattleChat = true;
    public bool useContextMenuOnBattlePets = true;
    public bool allowTooltipsBattlePets = true;
    public bool replaceEmotesBattlePets = true;
    // ---- Penumbra REQUIRED ----
    public bool redrawBattlePetOnSpawn = true;
    public bool redrawMinionOnSpawn = true;
    // ------------------------- Minion Settings -------------------------
    public bool useContextMenuOnMinions = true;
    public bool allowTooltipsOnMinions = true;
    public bool replaceEmotesOnMinions = true;
    public bool showNamesInMinionBook = true;
    // ---------------------- Sharing Mode Settings ----------------------
    public bool alwaysOpenAdvancedMode = false;

    public SerializableUserV3[]? serializableUsersV3 = null;
    
    public void Initialize()
    {
        LegacyInitialize();
        CurrentInitialize();
    }

    void CurrentInitialize()
    {
        serializableUsersV3 ??= new SerializableUserV3[0];
    }

    public void Save() 
    {
        List<SerializableUserV3> users = new List<SerializableUserV3>();
        foreach(PettableUser user in PluginLink.PettableUserHandler.Users)
            users.Add(user.SerializableUser);
        serializableUsersV3 = users.ToArray();
        PluginLink.DalamudPlugin.SavePluginConfig(this); 
    }

    public void ClearAllNicknames()
    {
        PluginLink.PettableUserHandler.Users.Clear();
        Save();
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

    [Obsolete] public bool replaceEmotes { get; set; } = true;
    [Obsolete] public bool allowTooltips { get; set; } = true;
    [Obsolete] public bool useContextMenus { get; set; } = true;
    [Obsolete] public bool useCustomNamesInChat { get; set; } = true;
    [Obsolete] public bool useCustomFlyoutInChat { get; set; } = true;
    [Obsolete] public bool allowCastBar { get; set; } = true;

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
    void LegacyInitialize()
    {
        users ??= new SerializableNickname[0];
        serializableUsers ??= new SerializableUser[0];
        serializableUsersV2 ??= new SerializableUserV2[0];
    }
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete

    #endregion
}
