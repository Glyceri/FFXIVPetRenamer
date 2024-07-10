using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.Update.Updatables;

namespace PetRenamer.PetNicknames.Update;

internal class UpdateHandler : IDisposable
{
    readonly DalamudServices DalamudServices;
    readonly Configuration Configuration;
    readonly IPetServices PetServices;
    readonly IPettableUserList PettableUserList;
    readonly IPettableDatabase PettableDatabase;
    readonly IPettableDatabase LegacyPettableDatabase;
    readonly IPetLog PetLog;
    readonly LodestoneNetworker LodestoneNetworker;
    readonly List<IUpdatable> _updatables = new List<IUpdatable>();

    public UpdateHandler(in DalamudServices dalamudServices, in Configuration configuration, in IPettableUserList pettableUserList, in IPettableDatabase legacyDatabase, in IPettableDatabase pettableDatabase, in IPetServices petServices, in LodestoneNetworker lodestoneNetworker)
    {
        DalamudServices = dalamudServices;
        Configuration = configuration;
        PetServices = petServices;
        PettableUserList = pettableUserList;
        PetLog = PetServices.PetLog;
        PettableDatabase = pettableDatabase;
        LegacyPettableDatabase = legacyDatabase;
        LodestoneNetworker = lodestoneNetworker;

        DalamudServices.Framework.Update += OnUpdate;
        Setup();
    }

    void Setup()
    {
        _updatables.Add(new LegacyDatabaseHelper(DalamudServices, LegacyPettableDatabase, PettableDatabase, PetServices));
        _updatables.Add(new SaveHelper(Configuration, PettableDatabase));
        _updatables.Add(new PettableUserHandler(DalamudServices, PettableUserList, PettableDatabase, PetServices));
        _updatables.Add(new LodestoneQueueHelper(in LodestoneNetworker));
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
