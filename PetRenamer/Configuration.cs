using Dalamud.Configuration;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PetRenamer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    [JsonIgnore]
    public const int currentSaveFileVersion = 4;

    public int Version { get; set; } = 4;

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
        if (serializableUsersV2 == null) serializableUsersV2 = new SerializableUserV2[0];
        if (serializableUsersV3 == null) serializableUsersV3 = new SerializableUserV3[0];

        // TODO: this is temporary
        List<SerializableUserV3> serializableUserV3 = new List<SerializableUserV3>();
        foreach(SerializableUserV2 serializableUserV2 in serializableUsersV2)
        {
            serializableUserV3.Add(new SerializableUserV3(serializableUserV2.ids, serializableUserV2.names, serializableUserV2.username, serializableUserV2.homeworld));
        }
        serializableUsersV3 = serializableUserV3.ToArray();
    }

    public void Save() => PluginLink.DalamudPlugin.SavePluginConfig(this);

    public void ClearAllUsersV2()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        serializableUsersV2 = new SerializableUserV2[0];
        Save();
    }

    public void ClearNicknamesForAllUsersV2()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        foreach (SerializableUserV2 user in serializableUsersV2!)
            ClearNicknamesForUserV2(user, false);
        Save();
    }

    public void ClearNicknamesForUserV2(SerializableUserV2 user, bool autosave = true)
    {
        user.ids = new int[0];
        user.names = new string[0];
        if (autosave) Save();
    }

    public void ClearNicknamesForLocalUserV2()
    {
        SerializableUserV2? user = ConfigurationUtils.instance.GetLocalUserV2();
        if (user == null) return;
        ClearNicknamesForUserV2(user);
    }

    #region OBSOLETE

    [Obsolete("Use ClearAllUsersV2() Instead")]
    public void ClearAllUsers()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        serializableUsers = new SerializableUser[0];
        Save();
    }

    [Obsolete("Use ClearNicknamesForAllUsersV2() Instead")]
    public void ClearNicknamesForAllUsers()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        foreach (SerializableUser user in serializableUsers!)
            ClearNicknamesForUser(user, false);
        Save();
    }

    [Obsolete("Use ClearNicknamesForLocalUserV2() Instead")]
    public void ClearNicknamesForLocalUser()
    {
        SerializableUser? user = PluginLink.Utils.Get<ConfigurationUtils>().GetLocalUser();
        if (user == null) return;
        ClearNicknamesForUser(user);
    }

    [Obsolete("Use ClearNicknamesForUserV2() instead")]
    public void ClearNicknamesForUser(SerializableUser user, bool autosave = true)
    {
        user.nicknames = new SerializableNickname[0];
        if(autosave) Save();
    }

    [Obsolete("Use ClearNicknamesForUser() instead")]
    public void ClearNicknames()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        users = new SerializableNickname[0];
        Save();
    }



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
    }
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete

    #endregion
}
