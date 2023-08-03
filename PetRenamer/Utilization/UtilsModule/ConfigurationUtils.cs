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
    public SerializableUser? GetLocalPlayer()
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return null;
        return PluginLink.Configuration.serializableUsers[0];
    }

    public void SetLocalNickname(int forPet, string nickname)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return;

        if (!PluginLink.Utils.Get<NicknameUtils>().ContainsLocal(forPet))
        {
            List<SerializableNickname> nicknames = PluginLink.Configuration.serializableUsers[0]!.nicknames.ToList();
            nicknames.Insert(0, (new SerializableNickname(forPet, nickname)));
            PluginLink.Configuration.serializableUsers[0]!.nicknames = nicknames.ToArray();
        }

        SerializableNickname nick = PluginLink.Utils.Get<NicknameUtils>().GetLocalNickname(forPet);
        if (nick != null)
            nick.Name = nickname;

        PluginLink.Configuration.Save();
    }

    public void RemoveLocalNickname(int forPet)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return;

        if (PluginLink.Utils.Get<NicknameUtils>().ContainsLocal(forPet))
        {
            List<SerializableNickname> nicknames = PluginLink.Configuration.serializableUsers[0]!.nicknames.ToList();
            for (int i = nicknames.Count - 1; i >= 0; i--)
                if (nicknames[i].ID == forPet)
                    nicknames.RemoveAt(i);

            PluginLink.Configuration.serializableUsers[0]!.nicknames = nicknames.ToArray();
            PluginLink.Configuration.Save();
        }
    }


    public void AddNewUser(SerializableUser user)
    {
        if (UserExists(user)) return;

        List<SerializableUser> users = PluginLink.Configuration.serializableUsers!.ToList();
        users.Add(user);
        PluginLink.Configuration.serializableUsers = users.ToArray();
        PluginLink.Configuration.Save();
    }

    public bool UserExists(SerializableUser testForUser)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return false;

        foreach(SerializableUser user in PluginLink.Configuration.serializableUsers!)
            if(user.username == testForUser.username && user.homeworld == testForUser.homeworld) 
                return true;
        
        return false;
    }

    public SerializableUser GetUser(SerializableUser testForUser)
    {
        foreach (SerializableUser user in PluginLink.Configuration.serializableUsers!)
            if (user.username == testForUser.username && user.homeworld == testForUser.homeworld)
                return user;

        return null!;
    }

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
