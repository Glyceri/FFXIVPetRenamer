using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Update.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal unsafe class UserFindUpdatable : IUpdatable
{
    public bool Enabled { get; set; } = true;

    DalamudServices DalamudServices { get; init; }
    PetServices PetServices { get; init; }
    IPetLog PetLog { get; init; }
    PettableUserHandler PettableUserHandler { get; init; }

    public UserFindUpdatable(DalamudServices dalamudServices, PetServices petServices, PettableUserHandler pettableUserHandler)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PetLog = PetServices.PetLog;
        PettableUserHandler = pettableUserHandler;
    }

    public void OnUpdate(IFramework framework)
    {
        Span<Pointer<BattleChara>> charaSpan = CharacterManager.Instance()->BattleCharas;

        List<Pointer<BattleChara>> battleCharas = new List<Pointer<BattleChara>>();

        int length = charaSpan.Length;
        for (int i = 0; i < length; i++)
        {
            Pointer<BattleChara> chara = charaSpan[i];
            if (chara.Value == null) continue;
            ulong contentID = chara.Value->ContentId;
            if (contentID == 0) continue;
            battleCharas.Add(chara);
        }

        PettableUserHandler.SetCurrentBatch(ref battleCharas);
    }
}
