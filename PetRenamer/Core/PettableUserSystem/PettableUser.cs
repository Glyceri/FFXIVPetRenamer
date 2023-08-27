using System;
using System.Runtime.InteropServices;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.PettableUserSystem;

internal unsafe class PettableUser
{
    readonly string _username;
    readonly ushort _homeworld;

    IntPtr _user;
    IntPtr _companion;
    IntPtr _BattlePet;

    int _BattlePetSkeletonID = -1; // [407 = EOS, 408 = SELENE.....]
    int _LastBattlePetSkeletonID = -1;

    int _BattlePetID = -1; // [-2, -3, -4.....]
    int _LastBattlePetID = -1;
    int _CompanionID = -1; // [0, 1, 2, 3.....]
    int _LastCompanionID = -1;

    string _CustomBattlePetName = string.Empty; // [Sally]
    string _CustomCompanionName = string.Empty; // [George]

    string _BattlePetBaseName = string.Empty; // [Eos]
    string _CompanionBaseName = string.Empty; // [Hedgehoglet]

    bool _userChangedCompanion = false;
    bool _userChangedBattlePet = false;

    SerializableUserV3 _serializableUser;

    public BattleChara* User => (BattleChara*)_user;
    public BattleChara* BattlePet => (BattleChara*)_BattlePet;
    public Companion* Companion => (Companion*)_companion;
    public SerializableUserV3 SerializableUser => _serializableUser; 
    public string UserName => _username;
    public ushort Homeworld => _homeworld;

    public bool LocalUser
    {
        get
        {
            if (!UserExists) return false;
            if (User->Character.GameObject.ObjectIndex == 0) return true;
            return false;
        }
    }
    public bool UserExists => _user != nint.Zero;
    public bool HasCompanion => _companion != nint.Zero;
    public bool HasBattlePet => _BattlePet != nint.Zero;

    public int BattlePetSkeletonID => _BattlePetSkeletonID;
    public int BattlePetID => _BattlePetID;
    public string BattlePetCustomName => _CustomBattlePetName;
    public string BaseBattelPetName => _BattlePetBaseName;

    public int CompanionID => _CompanionID;
    public string CustomCompanionName => _CustomCompanionName;
    public string CompanionBaseName => _CompanionBaseName;

    public bool BattlePetChanged => _userChangedBattlePet;
    public bool CompanionChanged => _userChangedCompanion;
    public bool AnyPetChanged => _userChangedBattlePet || _userChangedCompanion;

    public PettableUser(string username, ushort homeworld, SerializableUserV3 serializableUser)
    {
        _username = username;
        _homeworld = homeworld;
        _serializableUser = serializableUser;
    }

    public PettableUser(string username, ushort homeworld, SerializableUserV3 serializableUser, nint user) : this(username, homeworld, serializableUser)
    {
        _user = user;
    }

    public void SetUser(BattleChara* user)
    {
        _user = (nint)user;

        
    }

    public void SetBattlePet(BattleChara* battlePet)
    {
        _BattlePet = (nint)battlePet;
        if (_BattlePet == nint.Zero) ResetBattlePet();
        else
        {
            _BattlePetSkeletonID = battlePet->Character.CharacterData.ModelCharaId;
            _BattlePetID = RemapUtils.instance.GetPetIDFromClass(User->Character.CharacterData.ClassJob);
            if (_LastBattlePetID != _BattlePetID || _LastBattlePetSkeletonID != _BattlePetSkeletonID)
            {
                _userChangedBattlePet = true;
                _LastBattlePetSkeletonID = _BattlePetSkeletonID;
                _LastBattlePetID = _BattlePetID;
                _BattlePetBaseName = Marshal.PtrToStringUTF8((IntPtr)battlePet->Character.GameObject.Name)!;
                SerializableUser.LoopThroughBreakable((nickname) => 
                {
                    if (_BattlePetID == nickname.Item1)
                    {
                        _CustomBattlePetName = nickname.Item2;
                        return true;
                    }
                    return false;
                });
            }
        }
    }

    public void SetCompanion(Companion* companion)
    {
        _companion = (nint)companion;
        if (_companion == nint.Zero) ResetCompanion();
        else
        {
            _CompanionID = companion->Character.CharacterData.ModelSkeletonId;
            if(_LastCompanionID != _CompanionID)
            {
                _userChangedCompanion = true;
                _LastCompanionID = _CompanionID;
                _CompanionBaseName = Marshal.PtrToStringUTF8((IntPtr)companion->Character.GameObject.Name)!;
                SerializableUser.LoopThroughBreakable((nickname) =>
                {
                    if (_CompanionID == nickname.Item1)
                    {
                        _CustomCompanionName = nickname.Item2;
                        return true;
                    }
                    return false;
                });
            }
        }
    }

    public void Reset()
    {
        _user = nint.Zero;
        _BattlePet = nint.Zero;
        _companion = nint.Zero;
        _userChangedCompanion = false;
        _userChangedBattlePet = false;

        if (_serializableUser?.ToggleBackChanged() ?? false)
        {
            _LastBattlePetID = -1;
            _LastBattlePetSkeletonID = -1;
            _LastCompanionID = -1;
        }

        ResetBattlePet();
        ResetCompanion();
    }

    void ResetCompanion()
    {
        _CompanionID = -1;
    }

    void ResetBattlePet()
    {
        _BattlePetID = -1;
        _BattlePetSkeletonID = -1;
    }

    public string GetCustomName(int skeletonID)
    {
        string str = string.Empty;
        _serializableUser?.LoopThroughBreakable((nickname) =>
        {
            if(nickname.Item1 == skeletonID)
            {
                str = nickname.Item2;
                return true;
            }
            return false;
        });

        return str;
    }
}
