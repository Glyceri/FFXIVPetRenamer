using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking;

internal abstract class HookableElement : IHookableElement
{
    protected readonly DalamudServices DalamudServices;
    protected readonly IPetServices    PetServices;

    protected HookableElement(DalamudServices services, IPetServices petServices)
    {
        DalamudServices = services;
        PetServices     = petServices;

        PetServices.DirtyListener.RegisterOnDirtyDatabase(OnPettableDatabaseChange);
        PetServices.DirtyListener.RegisterOnClearEntry(OnPettableEntryClear);
        PetServices.DirtyListener.RegisterOnDirtyEntry(OnPettableEntryChange);
        PetServices.DirtyListener.RegisterOnDirtyName(OnNameDatabaseChange);
        PetServices.DirtyListener.RegisterOnPlayerCharacterDirty(OnPlayerDirty);

        DalamudServices.Hooking.InitializeFromAttributes(this);
    }

    public    abstract void Init();
    protected abstract void OnDispose();

    protected virtual void OnNameDatabaseChange(INamesDatabase nameDatabase)            
        => Refresh();

    protected virtual void OnPettableDatabaseChange(IPettableDatabase pettableDatabase) 
        => Refresh();

    protected virtual void OnPettableEntryChange(IPettableDatabaseEntry pettableEntry)  
        => Refresh();

    protected virtual void OnPettableEntryClear(IPettableDatabaseEntry pettableEntry)   
        => Refresh();

    protected virtual void OnPlayerDirty(IPettableUser user)                           
        => Refresh();

    public virtual void Refresh() { }

    public void Dispose()
    {
        PetServices.DirtyListener.UnregisterOnDirtyDatabase(OnPettableDatabaseChange);
        PetServices.DirtyListener.UnregisterOnClearEntry(OnPettableEntryClear);
        PetServices.DirtyListener.UnregisterOnDirtyEntry(OnPettableEntryChange);
        PetServices.DirtyListener.UnregisterOnDirtyName(OnNameDatabaseChange);
        PetServices.DirtyListener.UnregisterOnPlayerCharacterDirty(OnPlayerDirty);

        OnDispose();
    }
}
