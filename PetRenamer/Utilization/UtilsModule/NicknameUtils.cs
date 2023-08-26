using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Singleton;
using PetRenamer.Core.Updatable.Updatables;
using PetRenamer.Utilization.Attributes;
using PetRenamer.Utilization.Enum;
using System;
using FFCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class NicknameUtils : UtilsRegistryType, ISingletonBase<NicknameUtils>
{
    public static NicknameUtils instance { get; set; } = null!;

    internal unsafe SerializableNickname GetBattlePetFromOwnerPtr(GameObject* gObj)
    {
        return GetFromGameObjectPtr(&((BattleChara*)gObj)->Character.GameObject, PetType.BattlePet);
    }

    internal unsafe SerializableNickname GetFromGameObjectPtr(GameObject* gObj, PetType type)
    {
        if (gObj == null) return null!;
        foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters)
        {
            int curID = -1;
            if (type == PetType.Companion)
            {
                if (!character.HasCompanion()) continue;
                if (character.GetCompanionID() != ((FFCharacter*)gObj)->CharacterData.ModelSkeletonId) continue;
                if (character.GetOwnID() != ((FFCharacter*)gObj)->CompanionOwnerID) continue;
                curID = character.GetCompanionID();
            }
            else if (type == PetType.BattlePet)
            {
                if (!character.HasBattlePet()) continue;
                if (!RemapUtils.instance.battlePetRemap.ContainsKey(((BattleChara*)gObj)->Character.CharacterData.ModelCharaId)) continue; 
                if (character.GetBattlePetObjectID() != ((BattleChara*)gObj)->Character.GameObject.ObjectID) continue; 
                curID = character.GetBattlePetID();
            }
            if (curID == -1) continue; 
            return GetNicknameV2(character.associatedUser!, curID);
        }
        return null!;
    }

    internal SerializableNickname[] GetLocalNicknamesV2()
    {
        SerializableUserV2? user = ConfigurationUtils.instance.GetLocalUserV2();
        if (user == null) return new SerializableNickname[0];
        return user.nicknames;
    }

    internal SerializableNickname GetLocalNicknameV2(int ID)
    {
        SerializableUserV2? user = ConfigurationUtils.instance.GetLocalUserV2();
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
        SerializableUserV2? localUser = ConfigurationUtils.instance.GetLocalUserV2();
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
