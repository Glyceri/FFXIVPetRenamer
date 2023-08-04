using Dalamud.Configuration;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using System;

namespace PetRenamer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 3;

    public SerializableUser[]? serializableUsers = null;

    public bool displayCustomNames = true;
    public bool useCustomTheme = true;

    public void Initialize()
    {
        LegacyInitialize();
        CurrentInitialize();
    }

    void CurrentInitialize()
    {
        if(serializableUsers == null) serializableUsers = new SerializableUser[0];
    }

    public void ClearAllUsers()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        serializableUsers = new SerializableUser[0];
        Save();
    }

    public void ClearNicknamesForAllUsers()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        foreach (SerializableUser user in serializableUsers!)
            ClearNicknamesForUser(user, false);
        Save();
    }

    public void ClearNicknamesForLocalUser()
    {
        SerializableUser? user = PluginLink.Utils.Get<ConfigurationUtils>().GetLocalUser();
        if (user == null) return;
        ClearNicknamesForUser(user);
    }

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

    public void Save() => PluginLink.DalamudPlugin.SavePluginConfig(this);

    //---------------------------Legacy Variables---------------------------
    // Will be kept for backwards compatibility
    //---------------------------Legacy Variables---------------------------
    [Obsolete("Old nickname Save System. Nowadays nicknames get saved per User")] 
    public SerializableNickname[]? users = null;


#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
    void LegacyInitialize()
    {
        if (users == null) users = new SerializableNickname[0];
    }
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete
}
