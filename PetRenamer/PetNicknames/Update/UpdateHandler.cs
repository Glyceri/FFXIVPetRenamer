using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using PetNicknames.PetNicknames.Update.Updatables;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.Update.Updatables;

namespace PetRenamer.PetNicknames.Update;

internal class UpdateHandler : IDisposable
{
    readonly DalamudServices DalamudServices;
    readonly IPettableUserList PettableUserList;
    readonly IImageDatabase ImageDatabase;
    readonly IIpcProvider IpcProvider;
    readonly LodestoneNetworker LodestoneNetworker;
    readonly IPetServices PetServices;
    readonly IIslandHook IslandHook;
    readonly IPettableDirtyListener DirtyListener;
    readonly IPettableDatabase Database;

    readonly List<IUpdatable> _updatables = new List<IUpdatable>();

    public UpdateHandler(
        DalamudServices dalamudServices, 
        IPettableUserList pettableUserList, 
        LodestoneNetworker lodestoneNetworker, 
        IIpcProvider ipcProvider, 
        IImageDatabase imageDatabase, 
        IPetServices petServices,
        IIslandHook islandHook,
        IPettableDirtyListener dirtyListener,
        IPettableDatabase database)
    {
        DalamudServices = dalamudServices;
        PettableUserList = pettableUserList;
        LodestoneNetworker = lodestoneNetworker;
        ImageDatabase = imageDatabase;
        IpcProvider = ipcProvider;
        PetServices = petServices;
        IslandHook = islandHook;
        DirtyListener = dirtyListener;
        Database = database;

        DalamudServices.Framework.Update += OnUpdate;
        Setup();
    }

    void Setup()
    {
        _updatables.Add(new PettableUserHandler(PettableUserList, PetServices, IslandHook, DirtyListener, Database));
        _updatables.Add(new LodestoneQueueHelper(LodestoneNetworker, ImageDatabase));
        _updatables.Add(new IPCPreparer(PettableUserList, IpcProvider));
        _updatables.Add(new IPCWatcher(PettableUserList, Database, PetServices));
    }

    void OnUpdate(IFramework framework)
    {
        int updatableCount = _updatables.Count;
        for(int i = 0; i < updatableCount; i++)
        {
            IUpdatable updatable = _updatables[i];
            if (!updatable.Enabled) continue;
            updatable.OnUpdate(framework);
        }
    }

    public void Dispose()
    {
        DalamudServices.Framework.Update -= OnUpdate;
    }
}
