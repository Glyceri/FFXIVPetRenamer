using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.Update.Updatables;

namespace PetRenamer.PetNicknames.Update;

internal class UpdateHandler : IDisposable
{
    private readonly DalamudServices        DalamudServices;
    private readonly IImageDatabase         ImageDatabase;
    private readonly IIpcProvider           IpcProvider;
    private readonly LodestoneNetworker     LodestoneNetworker;
    private readonly IPetServices           PetServices;
    private readonly SaveHandler            SaveHandler;    

    private readonly List<IUpdatable>       _updatables = [];

    public UpdateHandler(
        DalamudServices        dalamudServices, 
        IPetServices           petServices,
        LodestoneNetworker     lodestoneNetworker, 
        IIpcProvider           ipcProvider, 
        IImageDatabase         imageDatabase, 
        SaveHandler            saveHandler)
    {
        DalamudServices     = dalamudServices;
        LodestoneNetworker  = lodestoneNetworker;
        ImageDatabase       = imageDatabase;
        IpcProvider         = ipcProvider;
        PetServices         = petServices;
        SaveHandler         = saveHandler;

        DalamudServices.Framework.Update += OnUpdate;

        Setup();
    }
    
    public void Dispose()
    {
        DalamudServices.Framework.Update -= OnUpdate;
    }

    private void Setup()
    {
        _updatables.Add(new PettableUserHandler(PetServices));
        _updatables.Add(new LodestoneQueueHelper(LodestoneNetworker, ImageDatabase));
        _updatables.Add(new IpcPreparer(PetServices, IpcProvider));
        _updatables.Add(IpcProvider);
        _updatables.Add(SaveHandler);
        _updatables.Add(PetServices.TargetManager);
    }

    private void OnUpdate(IFramework framework)
    {
        int updatableCount = _updatables.Count;

        for(int i = 0; i < updatableCount; i++)
        {
            IUpdatable updatable = _updatables[i];

            if (!updatable.Enabled)
            {
                continue;
            }

            updatable.OnUpdate(framework);
        }
    }
}
