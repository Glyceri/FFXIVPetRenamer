using Dalamud.Configuration;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using System;

namespace PetRenamer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 4;

    public SerializableUserV2[]? serializableUsersV2 = null;

    public bool displayCustomNames = true;
    public bool useCustomTheme = true;

    public void Initialize()
    {
        LegacyInitialize();
        CurrentInitialize();
    }

    void CurrentInitialize()
    {
        if (serializableUsersV2 == null) serializableUsersV2 = new SerializableUserV2[0];
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
        SerializableUserV2? user = PluginLink.Utils.Get<ConfigurationUtils>().GetLocalUserV2();
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
