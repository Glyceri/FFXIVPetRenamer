using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using System;
using System.Collections.Generic;

namespace PetRenamer;

public class IpcStorage : IDisposable
{
    public delegate void OnIpcChange(ref List<(IPlayerCharacter, string)> change);
    public event OnIpcChange IpcChange = null!;

    List<(IPlayerCharacter, string)> nicknames = new List<(IPlayerCharacter, string)>();
    bool touched = false;

    public void Register((IPlayerCharacter, string) nickname)
    {
        nicknames.Add(nickname);
        touched = true;
    }

    public void OnUpdate(IFramework framework)
    {
        if (touched)
        {
            touched = false;
            IpcChange?.Invoke(ref nicknames);
        }
        nicknames.Clear();
    }

    public void Register(OnIpcChange IpcChange)
    {
        if (this.IpcChange == null) this.IpcChange = IpcChange;
        else this.IpcChange += IpcChange;
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
        nicknames.Clear();
    }
}
