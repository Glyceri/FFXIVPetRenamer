using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using System;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using Dalamud.Game.ClientState.Objects.SubKinds;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal unsafe class PettableUserHandler : IUpdatable
{
    public bool Enabled { get; set; } = true;

    DalamudServices DalamudServices { get; init; }
    IPetServices PetServices { get; init; }
    IPettableUserList PettableUserList { get; init; }
    IPetLog PetLog { get; init; }
    IPettableDatabase PettableDatabase { get; init; }

    public PettableUserHandler(DalamudServices dalamudServices, IPettableUserList pettableUserList, IPettableDatabase pettableDatabase, IPetServices petServices)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PettableUserList = pettableUserList;
        PetLog = PetServices.PetLog;
        PettableDatabase = pettableDatabase;
    }

    public void OnUpdate(IFramework framework, IPlayerCharacter playerCharacter)
    {
        Span<Pointer<BattleChara>> charaSpan = CharacterManager.Instance()->BattleCharas;

        List<Pointer<BattleChara>> potentialBattlePets = new List<Pointer<BattleChara>>();

        for (int i = 0; i < charaSpan.Length; i++)
        {
            Pointer<BattleChara> battleChara = charaSpan[i];
            IPettableUser? pettableUser = PettableUserList.pettableUsers[i];

            ulong pettableContentID = ulong.MaxValue;
            ulong contentID = ulong.MaxValue;
            if (battleChara != null) contentID = battleChara.Value->ContentId;
            if (pettableUser != null) pettableContentID = pettableUser.ContentID;

            if (contentID == ulong.MaxValue || contentID == 0 || pettableContentID != contentID)
            {
                pettableUser?.Destroy();
                PettableUserList.pettableUsers[i] = null;
            }

            if (pettableUser == null && battleChara != null && battleChara.Value->GetObjectKind() == FFXIVClientStructs.FFXIV.Client.Game.Object.ObjectKind.Pc)
            {
                IPettableUser newUser = new PettableUser(PetLog, PettableDatabase, battleChara);
                PettableUserList.pettableUsers[i] = newUser;
                continue;
            }

            pettableUser?.Set(battleChara);
        }
    }
}
