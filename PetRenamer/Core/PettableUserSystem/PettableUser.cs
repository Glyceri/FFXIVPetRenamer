using System;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.PettableUserSystem;

public unsafe class PettableUser
{
    readonly string _username;
    readonly ushort _homeworld;
    readonly string _homeworldName;

    IntPtr _user;

    readonly PetBase _minion;
    readonly PetBase _battlePet;

    public PetBase Minion => _minion;
    public PetBase BattlePet => _battlePet;

    public PetBase[] Pets => new PetBase[2] { _minion, _battlePet };

    readonly SerializableUserV3 _serializableUser;

    public nint nintUser => _user;

    public SerializableUserV3 SerializableUser => _serializableUser;
    public string UserName => _username;
    public ushort Homeworld => _homeworld;
    public string HomeWorldName => _homeworldName;
    public (string, ushort) Data => (_username, _homeworld);
    public byte ClassJob => _jobClass;
    public NicknameData NicknameData => new NicknameData(_minion.ID, _minion.CustomName, _battlePet.ID, _battlePet.CustomName);

    uint _objectID = 0;
    public uint ObjectID => _objectID;
    public int UserChangedID => _ChangedID;
    public bool UserChanged => _UserChanged || AnyPetChanged;
    int _ChangedID = 0;
    bool _UserChanged = false;
    byte _jobClass = 0;

    public bool LocalUser
    {
        get
        {
            if (!UserExists) return false;
            if (((BattleChara*)_user)->Character.GameObject.ObjectIndex == 0) return true;
            return false;
        }
    }
    public bool UserExists => _user != nint.Zero;
    public bool AnyPetChanged => _battlePet.Changed || _minion.Changed;
    public bool HasAny => SerializableUser.length > 0;

    public PettableUser(string username, ushort homeworld, SerializableUserV3 serializableUser)
    {
        _username = username;
        _homeworld = homeworld;
        _homeworldName = SheetUtils.instance.GetWorldName(homeworld);
        _serializableUser = serializableUser;
        _battlePet = new PetBase();
        _minion = new PetBase();
    }

    public PettableUser(string username, ushort homeworld, SerializableUserV3 serializableUser, nint user) : this(username, homeworld, serializableUser) => _user = user;
    public PettableUser(string username, ushort homeworld) : this(username, homeworld, new SerializableUserV3(username, homeworld)) { }
    public bool EqualsUser(SerializableUserV3 user) => UserName.ToLower().Trim() == user.username.ToLower().Trim() && Homeworld == user.homeworld;

    public void SetUser(BattleChara* user)
    {
        if (user == null) return;
        _objectID = user->Character.GameObject.ObjectID;
        _jobClass = user->Character.CharacterData.ClassJob;
        _user = (nint)user;
        bool _cType = SerializableUser.changed;
        _UserChanged = _cType;
        _ChangedID = SerializableUser.lastTouchedID;
        if (_ChangedID == Minion.ID) _minion.Clear();
        if (_ChangedID == BattlePet.ID) _battlePet.Clear();
    }

    public void SetBattlePet(BattleChara* battlePet)
    {
        int id = -1;
        if (battlePet != null)
        {
            id = -battlePet->Character.CharacterData.ModelCharaId;
            if (SerializableUser.mainSchlrSkeleton != id && _jobClass == PluginConstants.scholarJob)
            {
                SerializableUser.mainSchlrSkeleton = id;
                PluginLink.Configuration.Save();
            }
            if (SerializableUser.mainSmnrSkeleton != id && (_jobClass == PluginConstants.summonerJob || _jobClass == PluginConstants.arcanistJob))
            {
                SerializableUser.mainSmnrSkeleton = id;
                PluginLink.Configuration.Save();
            }
        }
        _battlePet.Set((nint)battlePet, id, _serializableUser);
    }

    public void SetCompanion(Companion* companion)
    {
        int minionID = -1;
        if (companion != null) minionID = companion->Character.CharacterData.ModelCharaId;
        _minion.Set((nint)companion, minionID, _serializableUser);
    }

    public void Reset()
    {
        _user = nint.Zero;
        _battlePet.SoftReset();
        _minion.SoftReset();
    }

    public int GetPetSkeleton(bool soft, int additional)
    {
        if (!soft)
        {
            if ((ClassJob == PluginConstants.arcanistJob || ClassJob == PluginConstants.summonerJob) && additional == -2) return SerializableUser.mainSmnrSkeleton;
            if ((ClassJob == PluginConstants.scholarJob) && additional == -3) return SerializableUser.mainSchlrSkeleton;
        }
        else if (soft)
        {
            if ((ClassJob == PluginConstants.arcanistJob || ClassJob == PluginConstants.summonerJob) && additional == -2) return SerializableUser.softSmnrSkeleton;
            if ((ClassJob == PluginConstants.scholarJob) && additional == -3) return SerializableUser.softSchlrSkeleton;
        }

        return additional;
    }

    public string GetCustomName(int skeletonID) => _serializableUser.GetNameFor(skeletonID)!;
}
