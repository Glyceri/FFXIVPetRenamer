using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.Serialization;

internal class SaveHandler : IDisposable
{
    private readonly Configuration          Configuration;
    private readonly IPettableUserList      UserList;
    private readonly IIpcProvider           IpcProvider;
    private readonly IPettableDirtyListener DirtyListener;
    private readonly IPetServices           PetServices;

    public SaveHandler(IPetServices petServices, IPettableUserList userList, IIpcProvider ipcProvider, IPettableDirtyListener dirtyListener)
    {
        PetServices     = petServices;
        Configuration   = PetServices.Configuration;
        UserList        = userList;
        IpcProvider     = ipcProvider;
        DirtyListener   = dirtyListener;

        DirtyListener.RegisterOnDirtyName(OnDirtyName);
        DirtyListener.RegisterOnDirtyDatabase(OnDirtyDatabase);
        DirtyListener.RegisterOnDirtyEntry(OnDirtyEntry);
        DirtyListener.RegisterOnClearEntry(OnDirtyEntry);
    }

    private void OnDirtyName(INamesDatabase database)
    {
        Save();

        IPettableUser? user = UserList.LocalPlayer;

        if (user == null)
        {
            return;
        }

        if (user.DataBaseEntry.ActiveDatabase != database)
        {
            return;
        }

        NotifyIPC();
    }

    private void OnDirtyEntry(IPettableDatabaseEntry entry)
    {
        Save();

        IPettableUser? user = UserList.LocalPlayer;

        if (user == null)
        {
            return;
        }

        if (user.DataBaseEntry != entry)
        {
            return;
        }

        NotifyIPC();
    }

    private void OnDirtyDatabase(IPettableDatabase database)
    {
        Save();
    }

    private void Save()
    {
        Configuration.Save();
    }

    private void NotifyIPC()
    {
        IpcProvider.NotifyDataChanged();
    }

    public void Dispose()
    {
        DirtyListener.UnregisterOnDirtyName(OnDirtyName);
        DirtyListener.UnregisterOnDirtyDatabase(OnDirtyDatabase);
        DirtyListener.UnregisterOnDirtyEntry(OnDirtyEntry);
        DirtyListener.UnregisterOnClearEntry(OnDirtyEntry);
    }
}
