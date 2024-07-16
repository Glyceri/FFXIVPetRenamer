using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using System;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDirtyHandler : IPettableDirtyListener, IPettableDirtyCaller
{
    Action<IPettableDatabase>? OnDatabase = _ => { };
    Action<IPettableDatabaseEntry>? OnEntry = _ => { };
    Action<IPettableDatabaseEntry>? OnClear = _ => { };
    Action<INamesDatabase>? OnName = _ => { };

    readonly DalamudServices DalamudServices;

    public PettableDirtyHandler(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;
    }

    public void ClearEntry(in IPettableDatabaseEntry entry)
    {
        DalamudServices.PluginLog.Debug("Clear entry");
        OnClear?.Invoke(entry);
    }

    public void DirtyDatabase(in IPettableDatabase database)
    {
        DalamudServices.PluginLog.Debug("Database Dirty");
        OnDatabase?.Invoke(database);
    }

    public void DirtyEntry(in IPettableDatabaseEntry entry)
    {
        DalamudServices.PluginLog.Debug("Entry Dirty");
        OnEntry?.Invoke(entry);
    }

    public void DirtyName(in INamesDatabase nameDatabase)
    {
        DalamudServices.PluginLog.Debug("Name Dirty");
        OnName?.Invoke(nameDatabase);
    }

    public void RegisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry)
    {
        OnClear -= onEntry;
        OnClear += onEntry;
    }

    public void RegisterOnDirtyDatabase(Action<IPettableDatabase> onDatabase)
    {
        OnDatabase -= onDatabase;
        OnDatabase += onDatabase;
    }

    public void RegisterOnDirtyEntry(Action<IPettableDatabaseEntry> onEntry)
    {
        OnEntry -= onEntry;
        OnEntry += onEntry;
    }

    public void RegisterOnDirtyName(Action<INamesDatabase> onNamesDatabase)
    {
        OnName -= onNamesDatabase;
        OnName += onNamesDatabase;
    }

    public void UnregisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry)
    {
        OnClear -= onEntry;
    }

    public void UnregisterOnDirtyDatabase(Action<IPettableDatabase> onDatabase)
    {
        OnDatabase -= onDatabase;
    }

    public void UnregisterOnDirtyEntry(Action<IPettableDatabaseEntry> onEntry)
    {
        OnEntry -= onEntry;
    }

    public void UnregisterOnDirtyName(Action<INamesDatabase> onNamesDatabase)
    {
        OnName -= onNamesDatabase;
    }
}
