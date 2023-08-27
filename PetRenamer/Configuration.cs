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
    public const int currentSaveFileVersion = 5;

    public int Version { get; set; } = 5;

    public SerializableUserV2[]? serializableUsersV2 = null;
    public SerializableUserV3[]? serializableUsersV3 = null;

    public bool displayCustomNames = true;
    public bool useCustomTheme = true;
    public bool replaceEmotes = true;
    public bool allowTooltips = true;
    public bool useContextMenus = true;

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
    [Obsolete("Old nickname Save System. Nowadays nicknames get saved per User")] 
    public SerializableNickname[]? users = null;
    [Obsolete("Old User Save System. Very innefficient. Use SerializableUserV2 now")]
    public SerializableUser[]? serializableUsers = null;
    [Obsolete("Issue fixed. Just keeping it here so I dont accidentally overwrite it later and fock over people with old savefiles :D")]
    public bool usePartyList = false;

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
    void LegacyInitialize()
    {
        if (users == null) users = new SerializableNickname[0];
        if (serializableUsers == null) serializableUsers = new SerializableUser[0];
        if (serializableUsersV2 == null) serializableUsersV2 = new SerializableUserV2[0];
    }
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete

    #endregion
}
