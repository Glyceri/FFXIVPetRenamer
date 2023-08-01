using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.Attributes;
using System;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class NicknameUtils : UtilsRegistryType
{
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
        {
            if (PluginLink.Configuration.users[i].ID == ID)
                return PluginLink.Configuration.users[i];
        }

        return null!;
    }
}
