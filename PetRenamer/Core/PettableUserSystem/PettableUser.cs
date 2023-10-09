using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;

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
    public NicknameData NicknameData => new NicknameData(_minion.ID, _minion.CustomName, _battlePet.ID, _battlePet.CustomName);

    uint _objectID = 0;
    public uint ObjectID => _objectID;
    public int UserChangedID => _ChangedID;
    public bool UserChanged => _UserChanged || AnyPetChanged;
    int _ChangedID = 0;
    bool _UserChanged = false;

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
        _user = (nint)user;
        bool _cType = SerializableUser.changed;
        _UserChanged = _cType;
        _ChangedID = SerializableUser.lastTouchedID;
        if (_ChangedID == Minion.ID) _minion.Clear();
        if (_ChangedID == BattlePet.ID) _battlePet.Clear();
    }

    int lastID = -1;
    int lastCast = -1;

    void HandleCast(int id)
    {
        int cast = PluginLink.PettableUserHandler.LastCastSoft.castID;
        if (id == lastID && lastCast == cast) return;

        lastID = id;
        lastCast = cast;
        if (!RemapUtils.instance.mutatableID.Contains(id)) return;

        foreach (KeyValuePair<int, uint> kvp in RemapUtils.instance.petIDToAction)
        {
            if (cast != kvp.Value) continue;
            int index = -1;
            for (int i = 0; i < PluginConstants.baseSkeletons.Length; i++)
            {
                if (PluginConstants.baseSkeletons[i] != kvp.Key) continue;
                index = i;
                break;
            }

            if (index == -1) break;

            if (SerializableUser.mainSkeletons[index] == id) break;
            SerializableUser.mainSkeletons[index] = id;
            SerializableUser.softSkeletons[index] = id;
            PluginLink.Configuration.Save();

            break;
        }
    }

    public void SetBattlePet(BattleChara* battlePet)
    {
        int id = -1;
        if (battlePet != null)
        {
            id = -battlePet->Character.CharacterData.ModelCharaId;
            HandleCast(id);
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
        bool valid = RemapUtils.instance.mutatableID.Contains(additional);

        if (!valid) return additional;

        int[] array;

        if (!soft) array = SerializableUser.mainSkeletons;
        else array = SerializableUser.softSkeletons;

        int index = -1;

        for (int i = 0; i < PluginConstants.baseSkeletons.Length; i++)
        {
            if (PluginConstants.baseSkeletons[i] == additional)
            {
                index = i;
                break;
            }
        }
        if (index >= 0 && index < array.Length) return array[index];

        return additional;
    }

    public string GetCustomName(int skeletonID) => _serializableUser.GetNameFor(skeletonID)!;
}
