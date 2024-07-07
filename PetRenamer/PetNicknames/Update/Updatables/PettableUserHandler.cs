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
using FFXIVClientStructs.FFXIV.Client.Game.Object;
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

    List<Pointer<BattleChara>> availablePets = new List<Pointer<BattleChara>>();

    public void OnUpdate(IFramework framework)
    {
        Span<Pointer<BattleChara>> charaSpan = CharacterManager.Instance()->BattleCharas;
        int charaSpanLength = charaSpan.Length;

        availablePets.Clear();

        for (int i = 0; i < charaSpanLength; i++)
        {
            Pointer<BattleChara> battleChara = charaSpan[i];
            IPettableUser? pettableUser = PettableUserList.pettableUsers[i];

            ObjectKind currentObjectKind = ObjectKind.None;
            ulong pettableContentID = ulong.MaxValue;
            ulong contentID = ulong.MaxValue;
            if (battleChara != null) 
            {
                contentID = battleChara.Value->ContentId; 
                currentObjectKind = battleChara.Value->GetObjectKind();
            }
            if (pettableUser != null) pettableContentID = pettableUser.ContentID;

            if (contentID == ulong.MaxValue || contentID == 0 || pettableContentID != contentID)
            {
                pettableUser?.Destroy();
                PettableUserList.pettableUsers[i] = null;
            }

            if (pettableUser == null && battleChara != null && currentObjectKind == ObjectKind.Pc)
            {
                IPettableUser newUser = new PettableUser(PetLog, PettableDatabase, PetServices, battleChara);
                PettableUserList.pettableUsers[i] = newUser;
                continue;
            }

            if (currentObjectKind == ObjectKind.BattleNpc) availablePets.Add(battleChara);

            pettableUser?.Set(battleChara);
        }

        IPettableUser?[] users = PettableUserList.pettableUsers;
        int size = users.Length;
        for (int i = 0; i < size; i++)
        {
            users[i]?.CalculateBattlepets(ref availablePets);
        }
    }
}
