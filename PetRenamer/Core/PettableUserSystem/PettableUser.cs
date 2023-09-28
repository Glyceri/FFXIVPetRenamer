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

    PetBase _companionBase = new PetBase();
    PetBase _battlePetBase = new PetBase();

    IntPtr _user;

    SerializableUserV3 _serializableUser;

    public nint nintUser => _user;
    public nint nintBattlePet => _battlePetBase.Pet;
    public nint nintCompanion => _companionBase.Pet;

    public BattleChara* User => (BattleChara*)_user;
    public BattleChara* BattlePet => (BattleChara*)nintBattlePet;
    public Companion* Companion => (Companion*)nintCompanion;
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
    public bool HasCompanion => nintCompanion != nint.Zero;
    public bool HasBattlePet => nintBattlePet != nint.Zero;
    public bool HasAny => HasCompanion || HasBattlePet;

    public int BattlePetSkeletonID => _battlePetBase.SkeletonID;
    public int BattlePetID => _battlePetBase.ID;
    public string BattlePetCustomName => _battlePetBase.CustomName;
    public string BaseBattlePetName => _battlePetBase.BaseName;

    public int BattlePetIndex => _battlePetBase.Index;
    public int MinionIndex => _companionBase.Index;

    public int CompanionID => _companionBase.ID;
    public string CustomCompanionName => _companionBase.CustomName;
    public string CompanionBaseName => _companionBase.BaseName;

    public bool BattlePetChanged => _battlePetBase.Changed;
    public bool CompanionChanged => _companionBase.Changed;
    public bool AnyPetChanged => _battlePetBase.Changed || _companionBase.Changed;

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

        if (_serializableUser?.ToggleBackChanged() ?? false)
        {
            _battlePetBase.Clear();
            _companionBase.Clear();
        }
    }

    public void SetBattlePet(BattleChara* battlePet)
    {
        int skeletonID = -1;
        int id = -1;

        if (battlePet != null)
        {
            skeletonID = ((Character*)battlePet)->CharacterData.ModelCharaId;
            id = RemapUtils.instance.GetPetIDFromClass(User->Character.CharacterData.ClassJob);
        }

        _battlePetBase.Set((Character*)battlePet, id, skeletonID, _serializableUser);
    }

    public void SetCompanion(Companion* companion)
    {
        int id = -1;

        if(companion != null)
        {
            id = ((Character*)companion)->CharacterData.ModelSkeletonId;
        }

        _companionBase.Set((Character*)companion, id, -1, _serializableUser);
    }

    public void Reset()
    {
        _user = nint.Zero;
        _battlePetBase.FullReset();
        _companionBase.FullReset();
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
