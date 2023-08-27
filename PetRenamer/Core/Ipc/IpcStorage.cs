using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Updatable.Updatables;
using System;
using System.Collections.Generic;

namespace PetRenamer;

public class IpcStorage : IDisposable
{
    // (string, uint) is the Equivelant of PetRenamer.Core.Serialization.SerializableUser
    // Which is now Obsolete ;)

    public delegate void OnIpcChange(Dictionary<(string, uint), NicknameData> change);
    public event OnIpcChange IpcChange = null!;

    private Dictionary<(string, uint), NicknameData> _IpcAssignedNicknames = new Dictionary<(string, uint), NicknameData>();

    bool touched = false;

    public Dictionary<(string, uint), NicknameData> IpcAssignedNicknames 
    {
        get 
        {
            touched = true;
            return _IpcAssignedNicknames;
        }
        set
        {
            touched = true;
            _IpcAssignedNicknames = value;
        }
    }

    public void OnUpdate(Framework framework)
    {
        if (touched)
        {
            touched = false;
            IpcChange?.Invoke(_IpcAssignedNicknames);
        }
    }

    public void Register(OnIpcChange IpcChange)
    {
        if (this.IpcChange == null) this.IpcChange = IpcChange;
        this.IpcChange += IpcChange;
    }

    public void Deregister(OnIpcChange IpcChange)
    {
        this.IpcChange -= IpcChange;
    }

    public void LateInitialize()
    {
        PluginHandlers.Framework.Update += OnUpdate;
    }

    public void Dispose()
    {
        PluginHandlers.Framework.Update -= OnUpdate;
        _IpcAssignedNicknames.Clear();
    }
}
