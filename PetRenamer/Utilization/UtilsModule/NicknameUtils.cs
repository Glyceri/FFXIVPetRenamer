using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.Attributes;
using System;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class NicknameUtils : UtilsRegistryType
{
    internal SerializableNickname GetLocalNickname(int ID)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return null!;
        return GetNickname(PluginLink.Configuration.serializableUsers![0], ID);
    }

    internal SerializableNickname GetNickname(SerializableUser user, int ID)
    {
        if(user == null) return null!;

        for (int i = 0; i < user.nicknames!.Length; i++)
            if (user.nicknames[i].ID == ID)
                return user.nicknames[i];

        return null!;
    }

    internal bool ContainsLocal(int ID)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return false; 
        SerializableUser localUser = PluginLink.Configuration.serializableUsers![0];

        foreach (SerializableNickname nickname in localUser.nicknames)
        {
            if (nickname == null) continue;
            if (nickname.ID == ID) return true;
        }

        return false;
    }

    internal bool IsSame(SerializableUser user, int ID, string name)
    {
        if(user == null) return false;
        foreach (SerializableNickname nickname in user.nicknames)
        {
            if (nickname == null) continue;
            if (nickname.ID == ID && nickname.Name == name) return true;
        }
        return false;
    }

    internal bool HasID(SerializableUser user, int ID)
    {
        if(user == null) return false;

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
}
