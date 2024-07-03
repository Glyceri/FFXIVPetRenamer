using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
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
    IPetLog PetLog { get; init; }

    List<IUpdatable> _updatables = new List<IUpdatable>();

    public UpdateHandler(DalamudServices dalamudServices, IPettableUserList pettableUserList, IPettableDatabase pettableDatabase, IPetServices petServices)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PettableUserList = pettableUserList;
        PetLog = PetServices.PetLog;
        PettableDatabase = pettableDatabase;

        DalamudServices.Framework.Update += OnUpdate;
        Setup();
    }

    void Setup()
    {
        _updatables.Add(new PettableUserHandler(DalamudServices, PettableUserList, PettableDatabase, PetServices));
        _updatables.Add(new LegacyDatabaseHelper(DalamudServices, PettableDatabase, PetServices));
    }

    void OnUpdate(IFramework framework)
    {
        IPlayerCharacter player = DalamudServices.ClientState.LocalPlayer!;
        if (player == null) return;

        int updatableCount = _updatables.Count;
        for(int i = 0; i < updatableCount; i++)
        {
            IUpdatable updatable = _updatables[i];
            if (!updatable.Enabled) continue;
            updatable.OnUpdate(framework, player);
        }
    }

    public void Dispose()
    {
        DalamudServices.Framework.Update -= OnUpdate;
    }
}
