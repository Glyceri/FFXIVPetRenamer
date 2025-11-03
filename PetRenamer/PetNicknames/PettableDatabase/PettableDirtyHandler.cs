using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Enums;
using System;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDirtyHandler : IPettableDirtyListener, IPettableDirtyCaller
{
    private Action<IPettableDatabase>?      OnDatabase      = _  => { };
    private Action<IPettableDatabaseEntry>? OnEntry         = _  => { };
    private Action<IPettableDatabaseEntry>? OnClear         = _  => { };
    private Action<INamesDatabase>?         OnName          = _  => { };
    private Action<IPettableUser>?          OnUser          = _  => { };
    private Action<Configuration>?          OnConfiguration = _  => { };
    private Action<PetWindowMode>?          OnPetModeChange = _  => { };
    private Action?                         OnWindowDirty   = () => { };

    public void ClearEntry(in IPettableDatabaseEntry entry)
    {
        OnClear?.Invoke(entry);
    }

    public void DirtyConfiguration(Configuration configuration)
    {
        OnConfiguration?.Invoke(configuration);
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

    public void DirtyPetMode(PetWindowMode petMode)
    {
        OnPetModeChange?.Invoke(petMode);
    }

    public void DirtyPlayer(IPettableUser user)
    {
        OnUser?.Invoke(user);   
    }

    public void DirtyWindow()
    {
        OnWindowDirty?.Invoke();
    }

    public void RegisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry)
    {
        OnClear -= onEntry;
        OnClear += onEntry;
    }

    public void RegisterOnConfigurationDirty(Action<Configuration> configuration)
    {
        OnConfiguration -= configuration;
        OnConfiguration += configuration;
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

    public void RegisterOnPetModeDirty(Action<PetWindowMode> petWindowMode)
    {
        OnPetModeChange -= petWindowMode;
        OnPetModeChange += petWindowMode;
    }

    public void RegisterOnPlayerCharacterDirty(Action<IPettableUser> user)
    {
        OnUser -= user;
        OnUser += user;
    }

    public void RegisterOnWindowDirty(Action windowDirty)
    {
        OnWindowDirty -= windowDirty;
        OnWindowDirty += windowDirty;
    }

    public void UnregisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry)
    {
        OnClear -= onEntry;
    }

    public void UnregisterOnConfigurationDirty(Action<Configuration> configuration)
    {
        OnConfiguration -= configuration;
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

    public void UnregisterOnPetModeDirty(Action<PetWindowMode> petWindowMode)
    {
        OnPetModeChange -= petWindowMode;
    }

    public void UnregisterOnPlayerCharacterDirty(Action<IPettableUser> user)
    {
        OnUser -= user;
    }

    public void UnregisterOnWindowDirty(Action windowDirty)
    {
        OnWindowDirty -= windowDirty;
    }
}
