using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.Update.Updatables;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Update;

internal class UpdateHandler : IDisposable
{
    DalamudServices DalamudServices { get; init; }
    PetServices PetServices { get; init; }
    IPetLog PetLog { get; init; }

    List<IUpdatable> _updatables = new List<IUpdatable>();

    public UpdateHandler(DalamudServices dalamudServices, PetServices petServices)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PetLog = PetServices.PetLog;

        DalamudServices.Framework.Update += OnUpdate;
        Setup();
    }

    void Setup()
    {
        PettableUserHandler pettableUserHandler = new PettableUserHandler(DalamudServices, PetServices);
        _updatables.Add(new UserFindUpdatable(DalamudServices, PetServices, pettableUserHandler));
        _updatables.Add(pettableUserHandler);
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
