using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Core.Serialization;
using PetRenamer.Logging;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.PettableUserSystem;

public unsafe class PettableUser
{
    readonly string _username;
    readonly string _initialsUserName;
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
    public string UserDisplayName => PluginLink.Configuration.anonymousMode ? _initialsUserName : _username;
    public ushort Homeworld => _homeworld;
    public string HomeWorldName => _homeworldName;

    uint _objectID = 0;
    public uint ObjectID => _objectID;
    public int UserChangedID => _ChangedID;
    public bool UserChanged => _UserChanged || AnyPetChanged;
    public bool IsIPCOnlyUser { get; init; } = false;
    public bool Declared { get; init; } = false;

    int _ChangedID = 0;
    int _LastChangedID = 0;
    bool _UserChanged = false;

    int _objectIndex = -1;

    public bool LocalUser
    {
        get
        {
            if (!UserExists) return false;
            if (_objectIndex == 0) return true;
            return false;
        }
    }
    public bool UserExists => _user != nint.Zero;
    public bool AnyPetChanged => _battlePet.Changed || _minion.Changed;
    public bool HasAny => SerializableUser.length > 0;
    public bool IsPettableClass => _isPettableClass;

    public PettableUser(string username, ushort homeworld, SerializableUserV3 serializableUser)
    {
        _username = username;
        _initialsUserName = StringUtils.instance.GetInitials(_username);
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
        _objectIndex = -1;
        if (user == null) return;
        _objectIndex = user->Character.GameObject.ObjectIndex;
        if (_objectIndex == 0) PluginLink.PettableUserHandler.SetLocalUser(this);
        _resetCounter = 0;
         _objectID = user->Character.GameObject.GetGameObjectId().ObjectId;
        _class = user->Character.CharacterData.ClassJob;
        if (_lastClass != _class)
        {
            _lastClass = _class;
            _isPettableClass = RemapUtils.instance.pettableClasses.Contains(_class);
        }
        _user = (nint)user;
        bool _cType = SerializableUser.changed;
        _UserChanged = _cType;
        _ChangedID = SerializableUser.lastTouchedID;
        if (_LastChangedID != _ChangedID)
        {
            _LastChangedID = _ChangedID;
            if (_ChangedID == Minion.ID) _minion.Clear();
            if (_ChangedID == BattlePet.ID) _battlePet.Clear();
        }
    }

    int lastID = -1;
    int lastCast = -1;

    int _resetCounter = 0;

    bool _isPettableClass = false;
    byte _lastClass = byte.MaxValue;
    byte _class = byte.MaxValue;

    void HandleCast(int id)
    {
        if (id == -1) return; // No Pet Active
        int cast = PluginLink.PettableUserHandler.LastCastSoft.castID; // Last Soft Cast (does this trigger before summoning the pet?)
        if (id == lastID || lastCast == cast) return; // If both are invalid go for a change!

        lastID = id; // Set the last ID
        lastCast = cast; // Set the last Cast
        if (!RemapUtils.instance.basePetIDToAction.ContainsValue((uint)cast)) return; // Is the action that we just cast a valid action to perform a rename on?
        if (!RemapUtils.instance.mutatableID.Contains(id)) return; // Is the id of the pet we just cast one that can be mutated by /petmirage?

        foreach (KeyValuePair<int, uint> kvp in RemapUtils.instance.petIDToAction) // Loop through every Pet and their respective Action
        {
            if (cast != kvp.Value) continue; // Is the current cast not equal to the pets given corresponding cast
            int index = -1; // Preset index to -1
            for (int i = 0; i < PluginConstants.baseSkeletons.Length; i++) // Base skeletons is an array with all default skeletons for pet mirage. Loop through it
            {
                if (PluginConstants.baseSkeletons[i] != kvp.Key) continue; // Is the base skeleton not equal to the key, continue
                index = i; // If it is, set the current index to i
                break; // We got the index we are looking for
            }

            if (index == -1) break; // if index is -1 it means nothing is found. So break

            if (SerializableUser.mainSkeletons[index] == id) break; // If the main skeleton we are trying to alter is already equal to the skeleton we are altering it to. break
            SerializableUser.mainSkeletons[index] = id; // Set main skeletons to the new ID
            SerializableUser.softSkeletons[index] = id; // Set soft skeletons to the new ID
            PluginLink.Configuration.Save(); // Save

            break; // Stop running
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
        if (companion != null)
        {
            minionID = companion->Character.CharacterData.ModelCharaId;
        }
        _minion.Set((nint)companion, minionID, _serializableUser);
    }

    public void Reset()
    {
        if (_resetCounter < 10) _resetCounter++;
        else if (_resetCounter == 10) SerializableUser.ClearAllIPC();
        _user = nint.Zero;
        _objectIndex = -1;
        _battlePet.FullReset();
        _minion.FullReset();
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

    public string GetCustomName(int skeletonID) => _serializableUser.GetNameFor(skeletonID, false)!;

    bool _isDestroying = false;
    int _destroyCounter = 0;
    public void Destroy()
    {
        if (_isDestroying) return;
        _isDestroying = true;
        SerializableUser.ClearAllIPC();
        _destroyCounter = 2;
        _UserChanged = true;
    }
    public bool DeathsMark => _isDestroying && --_destroyCounter <= 0;
}
