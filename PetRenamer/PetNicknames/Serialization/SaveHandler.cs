﻿using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Serialization;

internal class SaveHandler : IDisposable
{
    readonly Configuration Configuration;
    readonly IPettableUserList UserList;
    readonly IIpcProvider IpcProvider;
    readonly IPettableDirtyListener DirtyListener;

    public SaveHandler(in Configuration configuration, in IPettableUserList userList, in IIpcProvider ipcProvider, in IPettableDirtyListener dirtyListener)
    {
        Configuration = configuration;
        UserList = userList;
        IpcProvider = ipcProvider;
        DirtyListener = dirtyListener;

        DirtyListener.RegisterOnDirtyName(OnDirtyName);
        DirtyListener.RegisterOnDirtyDatabase(OnDirtyDatabase);
        DirtyListener.RegisterOnDirtyEntry(OnDirtyEntry);
        DirtyListener.RegisterOnClearEntry(OnDirtyEntry);
    }

    void OnDirtyName(INamesDatabase database)
    {
        Save();

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;

        if (user.DataBaseEntry.ActiveDatabase != database) return;

        NotifyIPC();
    }

    void OnDirtyEntry(IPettableDatabaseEntry entry)
    {
        Save();

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;

        if (user.DataBaseEntry != entry) return;

        NotifyIPC();
    }

    void OnDirtyDatabase(IPettableDatabase database)
    {
        Save();
    }

    void Save()
    {
        Configuration.Save();
    }

    void NotifyIPC()
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
