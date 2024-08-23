using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking;

internal abstract class HookableElement : IHookableElement
{
    public readonly DalamudServices DalamudServices;
    public readonly IPettableUserList UserList;
    public readonly IPetServices PetServices;
    public readonly IPettableDirtyListener DirtyListener;

    public HookableElement(in DalamudServices services, in IPettableUserList userList, in IPetServices petServices, in IPettableDirtyListener dirtyListener)
    {
        DalamudServices = services;
        UserList = userList;
        PetServices = petServices;
        DirtyListener = dirtyListener;

        DirtyListener.RegisterOnDirtyDatabase(OnPettableDatabaseChange);
        DirtyListener.RegisterOnClearEntry(OnPettableEntryClear);
        DirtyListener.RegisterOnDirtyEntry(OnPettableEntryChange);
        DirtyListener.RegisterOnDirtyName(OnNameDatabaseChange);

        DalamudServices.Hooking.InitializeFromAttributes(this);
    }

    public abstract void Init();
    protected abstract void OnDispose();

    protected virtual void OnNameDatabaseChange(INamesDatabase nameDatabase) => Refresh();
    protected virtual void OnPettableDatabaseChange(IPettableDatabase pettableDatabase) => Refresh();
    protected virtual void OnPettableEntryChange(IPettableDatabaseEntry pettableEntry) => Refresh();
    protected virtual void OnPettableEntryClear(IPettableDatabaseEntry pettableEntry) => Refresh();
    protected virtual void Refresh() { }

    public void Dispose()
    {
        DirtyListener.UnregisterOnDirtyDatabase(OnPettableDatabaseChange);
        DirtyListener.UnregisterOnClearEntry(OnPettableEntryClear);
        DirtyListener.UnregisterOnDirtyEntry(OnPettableEntryChange);
        DirtyListener.UnregisterOnDirtyName(OnNameDatabaseChange);

        OnDispose();
    }
}
