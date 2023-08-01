using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class ConfigurationUtils : UtilsRegistryType
{
    [Obsolete]
    public void SetNickname(int forPet, string nickname)
    {
        if (!PluginLink.Utils.Get<NicknameUtils>().Contains(forPet))
        {
            List<SerializableNickname> nicknames = PluginLink.Configuration.users!.ToList();
            nicknames.Add(new SerializableNickname(forPet, nickname));
            PluginLink.Configuration.users = nicknames.ToArray();
        }

        SerializableNickname nick = PluginLink.Utils.Get<NicknameUtils>().GetNickname(forPet);
        if (nick != null)
            nick.Name = nickname;

        PluginLink.Configuration.Save();
    }

    [Obsolete]
    public void RemoveNickname(int forPet)
    {
        if (PluginLink.Utils.Get<NicknameUtils>().Contains(forPet))
        {
            List<SerializableNickname> nicknames = PluginLink.Configuration.users!.ToList();
            for (int i = nicknames.Count - 1; i >= 0; i--)
                if (nicknames[i].ID == forPet)
                    nicknames.RemoveAt(i);
            
            PluginLink.Configuration.users = nicknames.ToArray();
            PluginLink.Configuration.Save();
        }
    }
}
