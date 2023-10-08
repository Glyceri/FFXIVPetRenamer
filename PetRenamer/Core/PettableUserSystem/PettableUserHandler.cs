using PetRenamer.Core.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Networking.NetworkingElements;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Core.Serialization;
using PetRenamer.Logging;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;

namespace PetRenamer.Core.PettableUserSystem;

internal class PettableUserHandler : IDisposable, IInitializable
{
    List<PettableUser> _users = new List<PettableUser>();

    public List<PettableUser> Users { get => _users; set => _users = value; }

    LastActionUsed _lastCastSoft;
    LastActionUsed _lastCast;
    public LastActionUsed LastCast { get => _lastCast; private set => _lastCast = value; }
    public LastActionUsed LastCastSoft { get => _lastCastSoft; private set => _lastCastSoft = value; }

    public void LoopThroughUsers(Action<PettableUser> action)
    {
        if (action == null) return;
        foreach (PettableUser user in _users)
            action.Invoke(user);
    }

    public void Dispose()
    {
        _users?.Clear();
    }

    public void Initialize()
    {
        int length = PluginLink.Configuration.serializableUsersV3!.Length;
        for (int i = 0; i < length; i++)
        {
            SerializableUserV3 user = PluginLink.Configuration.serializableUsersV3![i];
            DeclareUser(user, UserDeclareType.Add);
        }
    }

    public void DeclareUser(SerializableUserV3 user, UserDeclareType userDeclareType, bool force = false)
    {
        if (userDeclareType == UserDeclareType.Add) AddUser(user, force);
        else if (userDeclareType == UserDeclareType.Remove) RemoveUser(user);
    }

    void RemoveUser(SerializableUserV3 user)
    {
        for (int i = _users.Count - 1; i >= 0; i--)
        {
            if (_users[i].UserName != user.username || _users[i].Homeworld != user.homeworld) continue;

            try
            {
                ProfilePictureNetworked.instance.OnDeclare(_users[i], UserDeclareType.Remove, false);
            }
            catch (Exception e) { PetLog.Log(e.Message); }
            _users.RemoveAt(i);
        }
    }

    void AddUser(SerializableUserV3 user, bool force = false)
    {
        if (force) ForceRemoveUser(user);
        if (Contains(user)) return;

        PettableUser u;
        _users.Add(u = new PettableUser(user.username, user.homeworld, user));
        try
        {
            ProfilePictureNetworked.instance.OnDeclare(u, UserDeclareType.Add, false);
        }
        catch (Exception e) { PetLog.Log(e.Message); }
    }

    void ForceRemoveUser(SerializableUserV3 user)
    {
        for (int i = _users.Count - 1; i >= 0; i--)
            if (_users[i].EqualsUser(user))
            {
                _users.RemoveAt(i);
                return;
            }
    }

    public bool LocalPetChanged()
    {
        PettableUser user = LocalUser()!;
        if (user == null) return false;
        return user.AnyPetChanged;
    }

    public PettableUser? GetUser(string name) => GetUser(name, 9999);
    public PettableUser? GetUser(string name, ushort homeworld)
    {
        foreach (PettableUser user in _users)
            if (name.Contains(user.UserName) && (homeworld == 9999 || homeworld == user.Homeworld))
                return user;
        return null!;
    }

    public PettableUser GetUser(nint address)
    {
        if (address == nint.Zero) return null!;
        foreach (PettableUser user in _users)
        {
            if (user.nintUser == address) return user;
            foreach (PetBase pet in user.Pets)
            {
                if (pet.Pet != address) continue;
                return user;
            }
        }

        return null!;
    }

    public PetBase GetPet(nint address)
    {
        if (address == nint.Zero) return null!;
        foreach (PettableUser user in _users)
        {
            PetBase pet = GetPet(user, address);
            if (pet == null) continue;
            return pet;
        }
        return null!;
    }

    public PetBase GetPet(PettableUser user, nint address)
    {
        if (address == nint.Zero) return null!;
        if (!user.UserExists) return null!;
        foreach (PetBase pet in user.Pets)
        {
            if (pet.Pet != address) continue;
            return pet;
        }
        return null!;
    }

    public PettableUser? LocalUser()
    {
        foreach (PettableUser user in _users)
            if (user.LocalUser)
                return user;
        return null!;
    }

    public PettableUser? LastCastedUser()
    {
        PettableUser user = null!;
        foreach (PettableUser user1 in _users)
        {
            if (user1 == null) continue;
            if (!user1.UserExists) continue;
            if (user1.nintUser != _lastCast.castDealer && user1.BattlePet.Pet != _lastCast.castDealer) continue;
            user = user1;
            break;
        }

        return user;
    }

    public (string, string)[] GetValidNames(PettableUser user, string beContainedIn, bool strict, bool soft = true)
    {
        List<(string, string)> validNames = new List<(string, string)>();
        if (beContainedIn == null) return validNames.ToArray();
        if (user == null) return validNames.ToArray();
        if (!user.UserExists) return validNames.ToArray();
        foreach (int skelID in RemapUtils.instance.battlePetRemap.Keys)
        {
            int sId = skelID;
            string bPetname = SheetUtils.instance.GetBattlePetName(-sId) ?? string.Empty;
            if (bPetname == string.Empty) continue;
            if (!beContainedIn.ToString().Contains(bPetname)) continue;
            int tempId = user.GetPetSkeleton(soft, sId);
            if (tempId != -1) sId = tempId;

            string cName = user.SerializableUser.GetNameFor(sId) ?? string.Empty;
            if (cName == string.Empty || cName == null) continue;
            validNames.Add((bPetname, cName));
        }
        validNames.Sort((el1, el2) => el1.Item1.Length.CompareTo(el2.Item1.Length));
        return validNames.ToArray();
    }

    bool Contains(SerializableUserV3 user)
    {
        for (int i = 0; i < _users.Count; i++)
            if (_users[i].EqualsUser(user))
                return true;
        return false;
    }

    public void SetLastCast(IntPtr castUser, IntPtr castDealer, int castID) => _lastCast = new LastActionUsed(castUser, castDealer, castID);
    public void SetLastCastSoft(IntPtr castUser, IntPtr castDealer, int castID) => _lastCastSoft = new LastActionUsed(castUser, castDealer, castID);
}

public struct LastActionUsed
{
    public IntPtr castUser;
    public IntPtr castDealer;
    public int castID;

    public LastActionUsed(IntPtr castUser, IntPtr castDealer, int castID)
    {
        this.castUser = castUser;
        this.castDealer = castDealer;
        this.castID = castID;
    }
}

public enum LastActionType
{
    Cast,
    ActionUsed
}
