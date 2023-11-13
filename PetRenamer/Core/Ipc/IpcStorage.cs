using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Logging;
using System;
using System.Collections.Generic;

namespace PetRenamer;

public class IpcStorage : IDisposable
{
    public delegate void OnIpcChange(ref List<(PlayerCharacter, string)> change);
    public event OnIpcChange IpcChange = null!;

    List<(PlayerCharacter, string)> nicknames = new List<(PlayerCharacter, string)>();
    bool touched = false;

    public void Register((PlayerCharacter, string) nickname)
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
