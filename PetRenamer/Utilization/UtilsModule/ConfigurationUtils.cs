using Dalamud.Game.ClientState.Objects.SubKinds;
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
    public void SetLocalNicknameV2(int forPet, string nickname)
    {
        if (PluginLink.Configuration.serializableUsersV2!.Length == 0) return;
        SerializableUserV2 localUser = GetLocalUserV2()!;
        if(localUser == null) return;
        if (!PluginLink.Utils.Get<NicknameUtils>().ContainsLocalV2(forPet))
        {
            List<SerializableNickname> nicknames = localUser.nicknames.ToList();
            nicknames.Insert(0, (new SerializableNickname(forPet, nickname)));
            localUser.nicknames = nicknames.ToArray();
        }

        SerializableNickname nick = PluginLink.Utils.Get<NicknameUtils>().GetLocalNicknameV2(forPet);
        if (nick != null)
            nick.Name = nickname;

        PluginLink.Configuration.Save();
    }

    public void RemoveLocalNicknameV2(int forPet)
    {
        if (PluginLink.Configuration.serializableUsersV2!.Length == 0) return;

        SerializableUserV2 localUser = GetLocalUserV2()!;
        if (localUser == null) return;
        if (PluginLink.Utils.Get<NicknameUtils>().ContainsLocalV2(forPet))
        {
            List<SerializableNickname> nicknames = localUser.ToList();
            for (int i = nicknames.Count - 1; i >= 0; i--)
                if (nicknames[i].ID == forPet)
                    nicknames.RemoveAt(i);

            localUser.nicknames = nicknames.ToArray();
            PluginLink.Configuration.Save();
        }
    }

    public void AddNewUserV2(SerializableUserV2 user)
    {
        if (UserExistsV2(user)) return;

        List<SerializableUserV2> users = PluginLink.Configuration.serializableUsersV2!.ToList();
        users.Add(user);
        PluginLink.Configuration.serializableUsersV2 = users.ToArray();
        PluginLink.Configuration.Save();
    }

    public bool UserExistsV2(SerializableUserV2 testForUser)
    {
        if (PluginLink.Configuration.serializableUsersV2!.Length == 0) return false;

        foreach (SerializableUserV2 user in PluginLink.Configuration.serializableUsersV2!)
            if (user.username == testForUser.username && user.homeworld == testForUser.homeworld)
                return true;

        return false;
    }

    public SerializableUserV2 GetUserV2(SerializableUserV2? testForUser)
    {
        if (testForUser == null) return null!;
        foreach (SerializableUserV2 user in PluginLink.Configuration.serializableUsersV2!)
            if (user.username.Trim().ToLower() == testForUser.username.Trim().ToLower() && user.homeworld == testForUser.homeworld)
                return user;

        return null!;
    }

    public SerializableUserV2? GetLocalUserV2()
    {
        PlayerCharacter? chara = PluginHandlers.ClientState.LocalPlayer;
        if (chara == null) return null!;
        return GetUserV2(new SerializableUserV2(chara.Name.ToString(), (ushort)chara.HomeWorld.Id));
    }

    #region OBSOLETE

    [Obsolete("Use SetLocalNicknameV2() Instead")]
    public void SetLocalNickname(int forPet, string nickname)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return;

        if (!PluginLink.Utils.Get<NicknameUtils>().ContainsLocal(forPet))
        {
            List<SerializableNickname> nicknames = GetLocalUser()!.nicknames.ToList();
            nicknames.Insert(0, (new SerializableNickname(forPet, nickname)));
            GetLocalUser()!.nicknames = nicknames.ToArray();
        }

        SerializableNickname nick = PluginLink.Utils.Get<NicknameUtils>().GetLocalNickname(forPet);
        if (nick != null)
            nick.Name = nickname;

        PluginLink.Configuration.Save();
    }

    [Obsolete("Use RemoveLocalNicknameV2() Instead")]
    public void RemoveLocalNickname(int forPet)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return;

        if (PluginLink.Utils.Get<NicknameUtils>().ContainsLocal(forPet))
        {
            List<SerializableNickname> nicknames = GetLocalUser()!.nicknames.ToList();
            for (int i = nicknames.Count - 1; i >= 0; i--)
                if (nicknames[i].ID == forPet)
                    nicknames.RemoveAt(i);

            GetLocalUser()!.nicknames = nicknames.ToArray();
            PluginLink.Configuration.Save();
        }
    }

    [Obsolete("Use AddNewUserV2 Instead")]
    public void AddNewUser(SerializableUser user)
    {
        if (UserExists(user)) return;

        List<SerializableUser> users = PluginLink.Configuration.serializableUsers!.ToList();
        users.Add(user);
        PluginLink.Configuration.serializableUsers = users.ToArray();
        PluginLink.Configuration.Save();
    }

    [Obsolete("Use UserExistsV2() Instead")]
    public bool UserExists(SerializableUser testForUser)
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return false;

        foreach (SerializableUser user in PluginLink.Configuration.serializableUsers!)
            if (user.username == testForUser.username && user.homeworld == testForUser.homeworld)
                return true;

        return false;
    }

    [Obsolete("Use GetUserV2() Instead")]
    public SerializableUser GetUser(SerializableUser? testForUser)
    {
        if (testForUser == null) return null!;
        foreach (SerializableUser user in PluginLink.Configuration.serializableUsers!)
            if (user.username.Trim().ToLower() == testForUser.username.Trim().ToLower() && user.homeworld == testForUser.homeworld)
                return user;

        return null!;
    }

    [Obsolete("Use GetLocalUserV2() Instead")]
    public SerializableUser? GetLocalUser()
    {
        PlayerCharacter? chara =  PluginHandlers.ClientState.LocalPlayer;
        if (chara == null) return null!;
        return GetUser(new SerializableUser(new SerializableNickname[0], chara.Name.ToString(), (ushort)chara.HomeWorld.Id));
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
    #endregion
}
