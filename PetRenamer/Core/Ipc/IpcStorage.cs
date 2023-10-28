using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using System;
using System.Collections.Generic;

namespace PetRenamer;

public class IpcStorage : IDisposable
{
    public delegate void OnIpcChange(ref List<(nint, string)> change);
    public event OnIpcChange IpcChange = null!;

    List<(nint, string)> nicknames = new List<(nint, string)>();
    bool touched = false;

    public void Register((nint, string) nickname)
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
        nicknames.Clear();
    }
}
