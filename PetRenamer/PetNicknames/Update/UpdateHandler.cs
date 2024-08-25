using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using PetNicknames.PetNicknames.Update.Updatables;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
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
    readonly List<IUpdatable> _updatables = new List<IUpdatable>();

    public UpdateHandler(DalamudServices dalamudServices, IPettableUserList pettableUserList, LodestoneNetworker lodestoneNetworker, IIpcProvider ipcProvider, IImageDatabase imageDatabase)
    {
        DalamudServices = dalamudServices;
        PettableUserList = pettableUserList;
        LodestoneNetworker = lodestoneNetworker;
        ImageDatabase = imageDatabase;
        IpcProvider = ipcProvider;

        DalamudServices.Framework.Update += OnUpdate;
        Setup();
    }

    void Setup()
    {
        _updatables.Add(new PettableUserHandler(PettableUserList));
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
