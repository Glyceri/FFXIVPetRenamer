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
    readonly ISharingDictionary SharingDictionary;
    readonly IPetServices PetServices;
    readonly IPettableUserList PettableUserList;
    readonly IPettableDatabase PettableDatabase;
    readonly ILegacyDatabase LegacyPettableDatabase;
    readonly IPettableDirtyListener DirtyListener;
    readonly IImageDatabase ImageDatabase;
    readonly IIpcProvider IpcProvider;
    readonly IIslandHook IslandHook;
    readonly LodestoneNetworker LodestoneNetworker;
    readonly List<IUpdatable> _updatables = new List<IUpdatable>();

    public UpdateHandler(DalamudServices dalamudServices, ISharingDictionary sharingDictionary, IPettableUserList pettableUserList, ILegacyDatabase legacyDatabase, IPettableDatabase pettableDatabase, IPetServices petServices, LodestoneNetworker lodestoneNetworker, IImageDatabase imageDatabase, IPettableDirtyListener dirtyListener, IIpcProvider ipcProvider, IIslandHook islandHook)
    {
        DalamudServices = dalamudServices;
        SharingDictionary = sharingDictionary;
        PetServices = petServices;
        PettableUserList = pettableUserList;
        PettableDatabase = pettableDatabase;
        LegacyPettableDatabase = legacyDatabase;
        LodestoneNetworker = lodestoneNetworker;
        ImageDatabase = imageDatabase;
        DirtyListener = dirtyListener;
        IpcProvider = ipcProvider;
        IslandHook = islandHook;

        DalamudServices.Framework.Update += OnUpdate;
        Setup();
    }

    void Setup()
    {
        _updatables.Add(new PettableUserHandler(DalamudServices, SharingDictionary, PettableUserList, PettableDatabase, LegacyPettableDatabase, PetServices, DirtyListener, IslandHook));
        _updatables.Add(new LodestoneQueueHelper(LodestoneNetworker, ImageDatabase));
        _updatables.Add(new IPCPreparer(PettableUserList, IpcProvider));
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
