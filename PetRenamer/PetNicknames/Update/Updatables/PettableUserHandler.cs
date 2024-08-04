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
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System.Collections.Generic;
using PetRenamer.PetNicknames.IPC.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal unsafe class PettableUserHandler : IUpdatable
{
    public bool Enabled { get; set; } = true;

    readonly DalamudServices DalamudServices;
    readonly ISharingDictionary SharingDictionary;
    readonly IPetServices PetServices;
    readonly IPettableUserList PettableUserList;
    readonly IPetLog PetLog;
    readonly IPettableDatabase PettableDatabase;
    readonly ILegacyDatabase LegacyDatabase;
    readonly IPettableDirtyListener DirtyListener;

    public PettableUserHandler(in DalamudServices dalamudServices, in ISharingDictionary sharingDictionary, in IPettableUserList pettableUserList, in IPettableDatabase pettableDatabase, in ILegacyDatabase legacyDatabase, in IPetServices petServices, in IPettableDirtyListener dirtyListener)
    {
        DalamudServices = dalamudServices;
        SharingDictionary = sharingDictionary;
        PetServices = petServices;
        PettableUserList = pettableUserList;
        PetLog = PetServices.PetLog;
        PettableDatabase = pettableDatabase;
        LegacyDatabase = legacyDatabase;
        DirtyListener = dirtyListener;
    }

    List<Pointer<BattleChara>> availablePets = new List<Pointer<BattleChara>>();

    public void OnUpdate(IFramework framework)
    {
        SharingDictionary.Clear();

        Span<Pointer<BattleChara>> charaSpan = CharacterManager.Instance()->BattleCharas;
        int charaSpanLength = charaSpan.Length;

        availablePets.Clear();

        for (int i = 0; i < charaSpanLength; i++)
        {
            Pointer<BattleChara> battleChara = charaSpan[i];
            IPettableUser? pettableUser = PettableUserList.PettableUsers[i];

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
                // Destroy the user
                PettableUserList.PettableUsers[i]?.Dispose();
                PettableUserList.PettableUsers[i] = null;
            }

            if (pettableUser == null && battleChara != null && currentObjectKind == ObjectKind.Pc)
            {
                // Create a user
                IPettableUser newUser = new PettableUser(in SharingDictionary, in PettableDatabase, in LegacyDatabase, in PetServices, in DirtyListener, battleChara);
                PettableUserList.PettableUsers[i] = newUser;
                continue;
            }

            if (currentObjectKind == ObjectKind.BattleNpc) availablePets.Add(battleChara);

            // Update the user
            pettableUser?.Set(battleChara);
        }

        IPettableUser?[] users = PettableUserList.PettableUsers;
        int size = users.Length;
        for (int i = 0; i < size; i++)
        {
            users[i]?.CalculateBattlepets(ref availablePets);
        }
    }
}
