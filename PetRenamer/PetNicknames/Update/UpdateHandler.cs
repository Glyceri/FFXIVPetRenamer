using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.Update.Updatables;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Update;

internal class UpdateHandler : IDisposable
{
    DalamudServices DalamudServices { get; init; }
    IPetServices PetServices { get; init; }
    IPettableUserList PettableUserList { get; init; }
    IPettableDatabase PettableDatabase { get; init; }
    IPettableDatabase LegacyPettableDatabase { get; init; }
    IPetLog PetLog { get; init; }

    readonly LodestoneNetworker LodestoneNetworker;

    List<IUpdatable> _updatables = new List<IUpdatable>();

    public UpdateHandler(DalamudServices dalamudServices, IPettableUserList pettableUserList, IPettableDatabase legacyDatabase, IPettableDatabase pettableDatabase, IPetServices petServices, LodestoneNetworker lodestoneNetworker)
    {
        DalamudServices = dalamudServices;
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
