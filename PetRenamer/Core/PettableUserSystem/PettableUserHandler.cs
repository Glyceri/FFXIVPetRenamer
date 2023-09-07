using Dalamud.Logging;
using PetRenamer.Core.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core.Serialization;
using System;
using System.Collections.Generic;

namespace PetRenamer.Core.PettableUserSystem;

internal class PettableUserHandler : IDisposable, IInitializable
{
    List<PettableUser> _users = new List<PettableUser>();

    public List<PettableUser> Users { get => _users; set => _users = value; }

    LastActionUsed _lastCast;
    public LastActionUsed LastCast { get => _lastCast; private set => _lastCast = value; }

    public void BackwardsSAFELoopThroughUser(Action<PettableUser> action)
    {
        if (action == null) return;
        for(int i = _users.Count - 1; i >= 0; i--)
            action.Invoke(_users[i]);
    }

    public void LoopThroughUsers(Action<PettableUser> action)
    {
        if (action == null) return;
        foreach (PettableUser user in _users)
            action.Invoke(user);
    }

    public void LoopThroughBreakable(Func<PettableUser, bool> func)
    {
        if(func == null) return;
        foreach (PettableUser user in _users)
            if (func.Invoke(user))
                break;
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
        if(userDeclareType == UserDeclareType.Add)
        {
            if (force)
                for (int i = _users.Count - 1; i >= 0; i--)
                    if (_users[i].UserName == user.username && _users[i].Homeworld == user.homeworld)
                        _users.RemoveAt(i);
            if (!Contains(user))
                _users.Add(new PettableUser(user.username, user.homeworld, user));
        }
        else if (userDeclareType == UserDeclareType.Remove)
        {
            for (int i = _users.Count - 1; i >= 0; i--)
                if (_users[i].UserName == user.username && _users[i].Homeworld == user.homeworld)
                    _users.RemoveAt(i);
        }
    }

    public bool LocalPetChanged()
    {
        PettableUser user = LocalUser()!;
        if (user == null) return false;
        return user.AnyPetChanged;
    }

    public PettableUser? LocalUser()
    {
        PettableUser? returnThis = null;
        LoopThroughBreakable((user) =>
        {
            if (user.LocalUser)
            {
                returnThis = user;
                return true;
            }
            return false;
        });
        return returnThis;
    }

    bool Contains(SerializableUserV3 user)
    {
        for(int i = 0; i < _users.Count; i++)
            if (_users[i].UserName.ToLowerInvariant().Trim().Normalize() == user.username.ToLowerInvariant().Trim().Normalize() && _users[i].Homeworld == user.homeworld) 
                return true;
        return false;
    }

    public void SetLastCast(IntPtr castUser, IntPtr castDealer) 
    {
        _lastCast = new LastActionUsed(castUser, castDealer); 
    }
}

public struct LastActionUsed
{
    public IntPtr castUser;
    public IntPtr castDealer;

    public LastActionUsed(IntPtr castUser, IntPtr castDealer)
    {
        this.castUser = castUser;
        this.castDealer = castDealer;
    }
}

public enum LastActionType
{
    Cast,
    ActionUsed
}
