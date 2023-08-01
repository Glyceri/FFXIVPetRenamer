using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.Attributes;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class NicknameUtils : UtilsRegistryType
{
    internal SerializableNickname GetLocalNickname(int ID)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return null!;
        SerializableUser localUser = PluginLink.Configuration.serializableUsers![0];

        for (int i = 0; i < localUser.nicknames!.Length; i++)
            if (localUser.nicknames[i].ID == ID)
                return localUser.nicknames[i];

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

    internal SerializableNickname GetNickname(int ID)
    {
        for (int i = 0; i < PluginLink.Configuration.users!.Length; i++)
            if (PluginLink.Configuration.users[i].ID == ID)
                return PluginLink.Configuration.users[i];

        return null!;
    }
}
