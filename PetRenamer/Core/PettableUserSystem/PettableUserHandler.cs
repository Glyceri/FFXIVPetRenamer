using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
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

    public void DeclareUser(SerializableUserV3 user, UserDeclareType userDeclareType)
    {
        if(userDeclareType == UserDeclareType.Add)
        {
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

    bool Contains(SerializableUserV3 user)
    {
        for(int i = 0; i < _users.Count; i++)
            if (_users[i].UserName == user.username && _users[i].Homeworld == user.homeworld) 
                return true;
        return false;
    } 
}
