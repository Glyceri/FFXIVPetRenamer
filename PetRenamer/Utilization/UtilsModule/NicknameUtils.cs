using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.Attributes;
using System;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class NicknameUtils : UtilsRegistryType
{
    internal SerializableNickname GetLocalNicknameV2(int ID)
    {
        SerializableUserV2? user = PluginLink.Utils.Get<ConfigurationUtils>().GetLocalUserV2();
        if (user == null) return null!;
        return GetNicknameV2(user, ID);
    }

    internal SerializableNickname GetNicknameV2(SerializableUserV2 user, int ID)
    {
        if(user == null) return null!;

        for (int i = 0; i < user.nicknames!.Length; i++)
            if (user.nicknames[i].ID == ID)
                return user.nicknames[i];

        return null!;
    }

    internal bool ContainsLocalV2(int ID)
    {
        if (PluginLink.Configuration.serializableUsersV2!.Length == 0) return false; 
        SerializableUserV2? localUser = PluginLink.Utils.Get<ConfigurationUtils>().GetLocalUserV2();
        if (localUser == null) return false;

        foreach (SerializableNickname nickname in localUser.nicknames)
        {
            if (nickname == null) continue;
            if (nickname.ID == ID) return true;
        }

        return false;
    }

    internal bool IsSameV2(SerializableUserV2 user, int ID, string name)
    {
        if(user == null) return false;
        foreach (SerializableNickname nickname in user.nicknames)
        {
            if (nickname == null) continue;
            if (nickname.ID == ID && nickname.Name == name) return true;
        }
        return false;
    }

    internal bool HasIDV2(SerializableUserV2 user, int ID)
    {
        if(user == null) return false;

        foreach (SerializableNickname nickname in user.nicknames)
        {
            if (nickname == null) continue;
            if (nickname.ID == ID) return true;
        }
        return false;
    }

    #region Obsolete

    [Obsolete]
    internal SerializableNickname GetLocalNickname(int ID)
    {
        SerializableUser? user = PluginLink.Utils.Get<ConfigurationUtils>().GetLocalUser();
        if (user == null) return null!;
        return GetNickname(user, ID);
    }

    [Obsolete]
    internal SerializableNickname GetNickname(SerializableUser user, int ID)
    {
        if (user == null) return null!;

        for (int i = 0; i < user.nicknames!.Length; i++)
            if (user.nicknames[i].ID == ID)
                return user.nicknames[i];

        return null!;
    }


    [Obsolete]
    internal bool ContainsLocal(int ID)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return false;
        SerializableUser? localUser = PluginLink.Utils.Get<ConfigurationUtils>().GetLocalUser();
        if (localUser == null) return false;

        foreach (SerializableNickname nickname in localUser.nicknames)
        {
            if (nickname == null) continue;
            if (nickname.ID == ID) return true;
        }

        return false;
    }

    [Obsolete]
    internal bool IsSame(SerializableUser user, int ID, string name)
    {
        if (user == null) return false;
        foreach (SerializableNickname nickname in user.nicknames)
        {
            if (nickname == null) continue;
            if (nickname.ID == ID && nickname.Name == name) return true;
        }
        return false;
    }

    [Obsolete]
    internal bool HasID(SerializableUser user, int ID)
    {
        if (user == null) return false;

        foreach (SerializableNickname nickname in user.nicknames)
        {
            if (nickname == null) continue;
            if (nickname.ID == ID) return true;
        }
        return false;
    }

    [Obsolete]
    internal bool Contains(int ID)
    {
        foreach (SerializableNickname nickname in PluginLink.Configuration.users!)
        {
            if (nickname == null) continue;
            if (nickname.ID == ID) return true;
        }

        return false;
    }
    [Obsolete]
    internal SerializableNickname GetNickname(int ID)
    {
        for (int i = 0; i < PluginLink.Configuration.users!.Length; i++)
            if (PluginLink.Configuration.users[i].ID == ID)
                return PluginLink.Configuration.users[i];

        return null!;
    }

    #endregion
}
