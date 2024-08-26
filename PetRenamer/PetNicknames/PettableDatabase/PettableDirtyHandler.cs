using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDirtyHandler : IPettableDirtyListener, IPettableDirtyCaller
{
    Action<IPettableDatabase>? OnDatabase = _ => { };
    Action<IPettableDatabaseEntry>? OnEntry = _ => { };
    Action<IPettableDatabaseEntry>? OnClear = _ => { };
    Action<INamesDatabase>? OnName = _ => { };
    Action<IPettableUser>? OnUser = _ => { };

    public void ClearEntry(in IPettableDatabaseEntry entry)
    {
        OnClear?.Invoke(entry);
    }

    public void DirtyDatabase(in IPettableDatabase database)
    {
        OnDatabase?.Invoke(database);
    }

    public void DirtyEntry(in IPettableDatabaseEntry entry)
    {
        OnEntry?.Invoke(entry);
    }

    public void DirtyName(in INamesDatabase nameDatabase)
    {
        OnName?.Invoke(nameDatabase);
    }

    public void DirtyPlayer(IPettableUser user)
    {
        OnUser?.Invoke(user);   
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

    public void RegisterOnPlayerCharacterDirty(Action<IPettableUser> user)
    {
        OnUser -= user;
        OnUser += user;
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

    public void UnregisterOnPlayerCharacterDirty(Action<IPettableUser> user)
    {
        OnUser -= user;
    }
}
