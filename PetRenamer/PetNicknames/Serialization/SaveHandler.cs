using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Serialization;

internal class SaveHandler : IUpdatable, IDisposable
{
    private const double THROTTLE_DELAY = 8;

    public bool Enabled { get; set; } = true;

    private readonly Configuration          Configuration;
    private readonly IPettableUserList      UserList;
    private readonly IIpcProvider           IpcProvider;
    private readonly IPettableDirtyListener DirtyListener;
    private readonly IPetServices           PetServices;

    private double  throttleCounter     = THROTTLE_DELAY;
    private bool    hasPassedThrottle   = false;
    private bool    readyToSave         = false;

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

    public void OnUpdate(IFramework framework)
    {
        if (hasPassedThrottle)
        {
            if (readyToSave)
            {
                ReleaseSave();
            }

            return;
        }

        throttleCounter += framework.UpdateDelta.TotalSeconds;

        if (throttleCounter >= THROTTLE_DELAY)
        {
            hasPassedThrottle = true;
        }
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
        readyToSave = true;
    }

    private void ReleaseSave()
    {
        readyToSave         = false;
        hasPassedThrottle   = false;

        throttleCounter     = 0;

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
