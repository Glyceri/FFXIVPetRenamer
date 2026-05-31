using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class DirtyHandler : IDirtyListener, IDirtyCaller
{
    Action<IPettableDatabase>?      OnDatabase  = _ => { };
    Action<IPettableDatabaseEntry>? OnEntry     = _ => { };
    Action<IPettableDatabaseEntry>? OnClear     = _ => { };
    Action<INamesDatabase>?         OnName      = _ => { };
    Action<IPettableUser>?          OnUser      = _ => { };
    Action<Configuration>?          OnConfig    = _ => { };
    Action<IPettablePet>?           OnPet       = _ => { };

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

    public void DirtyConfig(Configuration configuration)
    {
        OnConfig?.Invoke(configuration);
    }

    public void DirtyPet(IPettablePet pet)
    {
        OnPet?.Invoke(pet);
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

    public void RegisterOnDirtyPet(Action<IPettablePet> pet)
    {
        OnPet -= pet;
        OnPet += pet;
    }

    public void RegisterOnDirtyConfig(Action<Configuration> config)
    {
        OnConfig -= config;
        OnConfig += config;
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

    public void UnregisterOnDirtyConfig(Action<Configuration> config)
    {
        OnConfig -= config;
    }

    public void UnregisterOnDirtyPet(Action<IPettablePet> pet)
    {
        OnPet -= pet;
    }
}
