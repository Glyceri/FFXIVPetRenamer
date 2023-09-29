using System;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
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

    ChangedType _cType;

    public PetBase Minion => _minion;
    public PetBase BattlePet => _battlePet;

    public PetBase[] Pets => new PetBase[2] { _minion, _battlePet };

    readonly SerializableUserV3 _serializableUser;

    public nint nintUser => _user;

    public SerializableUserV3 SerializableUser => _serializableUser;
    public ChangedType ChangedType => _cType;
    public string UserName => _username;
    public ushort Homeworld => _homeworld;
    public string HomeWorldName => _homeworldName;
    public NicknameData NicknameData => new NicknameData(_minion.ID, _minion.CustomName, _battlePet.ID, _battlePet.CustomName);

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
        _user = (nint)user;

        if ((_cType = _serializableUser.ToggleBackChanged()) != ChangedType.Not)
        {
            _minion.Clear();
            _battlePet.Clear();
        }
    }

    public void SetBattlePet(BattleChara* battlePet)
    {
        int id = -1;
        if (battlePet != null) id = RemapUtils.instance.GetPetIDFromClass(((BattleChara*)_user)->Character.CharacterData.ClassJob);
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
        _battlePet.FullReset();
        _minion.FullReset();
    }

    public string GetCustomName(int skeletonID) => _serializableUser.GetNameFor(skeletonID)!;
}
