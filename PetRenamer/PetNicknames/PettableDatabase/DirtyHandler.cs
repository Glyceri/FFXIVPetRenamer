using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class DirtyHandler : IDirtyListener, IDirtyCaller
{
    private readonly IPetLog PetLog;
    
    private Action<IPettableDatabase>?      OnDatabase  = _ => { };
    private Action<IPettableDatabaseEntry>? OnEntry     = _ => { };
    private Action<IPettableDatabaseEntry>? OnClear     = _ => { };
    private Action<INamesDatabase>?         OnName      = _ => { };
    private Action<IPettableUser>?          OnUser      = _ => { };
    private Action<Configuration>?          OnConfig    = _ => { };
    private Action<IPettablePet>?           OnPet       = _ => { };

    public DirtyHandler(IPetLog petLog)
        => PetLog = petLog;
    
    public void ClearEntry(in IPettableDatabaseEntry entry)
    {
        PetLog.DevLog($"Invoked Dirty Database: {entry.Name}");
        
        OnClear?.Invoke(entry);
    }

    public void DirtyDatabase(in IPettableDatabase database)
    {
        PetLog.DevLog($"Invoked Dirty Database.");
        
        OnDatabase?.Invoke(database);
    }

    public void DirtyEntry(in IPettableDatabaseEntry entry)
    {
        PetLog.DevLog($"Invoked Dirty Entry: {entry.Name}.");
        
        OnEntry?.Invoke(entry);
    }

    public void DirtyName(in INamesDatabase nameDatabase)
    {
        PetLog.DevLog($"Invoked Dirty Name.");
        
        OnName?.Invoke(nameDatabase);
    }

    public void DirtyPlayer(IPettableUser user)
    {
        PetLog.DevLog($"Invoked Dirty Player: {user.DataBaseEntry.Name}.");
        
        OnUser?.Invoke(user);   
    }

    public void DirtyConfig(Configuration configuration)
    {
        PetLog.DevLog($"Invoked Dirty Config.");
        
        OnConfig?.Invoke(configuration);
    }

    public void DirtyPet(IPettablePet pet)
    {
        PetLog.DevLog($"Invoked Dirty Pet: {pet.PetData?.Singular}.");
        
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
