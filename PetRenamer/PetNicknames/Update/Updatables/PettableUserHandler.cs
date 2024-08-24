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
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal unsafe class PettableUserHandler : IUpdatable
{
    public bool Enabled { get; set; } = true;

    bool isDirty = false;

    readonly DalamudServices DalamudServices;
    readonly ISharingDictionary SharingDictionary;
    readonly IPetServices PetServices;
    readonly IPettableUserList PettableUserList;
    readonly IPetLog PetLog;
    readonly IPettableDatabase PettableDatabase;
    readonly ILegacyDatabase LegacyDatabase;
    readonly IPettableDirtyListener DirtyListener;
    readonly IIslandHook IslandHook;

    public PettableUserHandler(DalamudServices dalamudServices, ISharingDictionary sharingDictionary, IPettableUserList pettableUserList, IPettableDatabase pettableDatabase, ILegacyDatabase legacyDatabase, IPetServices petServices, IPettableDirtyListener dirtyListener, IIslandHook islandHook)
    {
        DalamudServices = dalamudServices;
        SharingDictionary = sharingDictionary;
        PetServices = petServices;
        PettableUserList = pettableUserList;
        PetLog = PetServices.PetLog;
        PettableDatabase = pettableDatabase;
        LegacyDatabase = legacyDatabase;
        DirtyListener = dirtyListener;
        IslandHook = islandHook;

        DirtyListener.RegisterOnClearEntry(OnDirty);
        DirtyListener.RegisterOnDirtyDatabase(OnDirty);
        DirtyListener.RegisterOnDirtyEntry(OnDirty);
        DirtyListener.RegisterOnDirtyName(OnDirty);
    }

    List<Pointer<BattleChara>> availablePets = new List<Pointer<BattleChara>>();

    public void OnUpdate(IFramework framework)
    {
        SharingDictionary.Clear();

        HandleIsland();

        availablePets.Clear();

        // The CharacterManager.Instance()->BattleCharas is ALWAYS 100
        for (int i = 0; i < 100; i++)
        {
            Pointer<BattleChara> battleChara = CharacterManager.Instance()->BattleCharas[i];

            IPettableUser? pettableUser = PettableUserList.PettableUsers[i];

            ObjectKind currentObjectKind = ObjectKind.None;
            ulong pettableContentID = ulong.MaxValue;
            ulong contentID = ulong.MaxValue;
            uint ownerID = 0xE0000000;
            if (battleChara != null)
            {
                contentID = battleChara.Value->ContentId;
                currentObjectKind = battleChara.Value->GetObjectKind();
                ownerID = battleChara.Value->OwnerId;
            }
            if (pettableUser != null) pettableContentID = pettableUser.ContentID;

            if (contentID == ulong.MaxValue || contentID == 0 || pettableContentID != contentID)
            {
                // Destroy the user
                PettableUserList.PettableUsers[i]?.Dispose(PettableDatabase);
                PettableUserList.PettableUsers[i] = null;
            }

            if (pettableUser == null && battleChara != null && currentObjectKind == ObjectKind.Pc)
            {
                // Create a user
                IPettableUser newUser = new PettableUser(SharingDictionary, PettableDatabase, LegacyDatabase, PetServices, DirtyListener, battleChara);
                PettableUserList.PettableUsers[i] = newUser;
                continue;
            }

            if (currentObjectKind != ObjectKind.Pc && ownerID != 0xE0000000 && battleChara != null)
            {
                availablePets.Add(battleChara);
            }

            // Update the user
            pettableUser?.Set(battleChara);
        }

        for (int i = 0; i < PettableUsers.PettableUserList.PettableUserArraySize; i++)
        {
            IPettableUser? user = PettableUserList.PettableUsers[i];
            if (user == null) continue;

            for (int f = availablePets.Count - 1; f >= 0; f--)
            {
                Pointer<BattleChara> pointer = availablePets[f];
                if (pointer.Value->OwnerId != user.ShortObjectID) continue;

                user.SetBattlePet(pointer);
                break;
            }
        }
    }

    void OnDirty(IPettableDatabase database) => SetDirty();
    void OnDirty(INamesDatabase database) => SetDirty();
    void OnDirty(IPettableDatabaseEntry entry) => SetDirty();
    void SetDirty() => isDirty = true;
    void ClearDirty() => isDirty = false;

    void HandleIsland()
    {
        if (!PetServices.Configuration.showOnIslandPets) return;

        IslandHook.Update();
        UpdateIslandUser();

        if (!IslandHook.IslandStatusChanged && !isDirty) return;

        ClearDirty();

        if (IslandHook.IsOnIsland) HandleOnIsland();
        else HandleNotOnIsland();
    }

    void UpdateIslandUser()
    {
        IPettableUser? user = PettableUserList.PettableUsers[PettableUsers.PettableUserList.IslandIndex];
        if (user == null) return;

        user.Set(null);
    }

    void HandleNotOnIsland() => ClearIslandUser();
    void HandleOnIsland()
    {
        if (IslandHook.VisitingFor == null || IslandHook.VisitingHomeworld == null) return;

        IPettableDatabaseEntry? entry = PettableDatabase.GetEntry(IslandHook.VisitingFor, (ushort)IslandHook.VisitingHomeworld, false);
        if (entry == null) return;

        CreateIslandUser(entry);
    }

    void ClearIslandUser()
    {
        PettableUserList.PettableUsers[PettableUsers.PettableUserList.IslandIndex]?.Dispose(PettableDatabase);
        PettableUserList.PettableUsers[PettableUsers.PettableUserList.IslandIndex] = null;
    }

    void CreateIslandUser(IPettableDatabaseEntry entry)
    {
        ClearIslandUser();
        PettableUserList.PettableUsers[PettableUsers.PettableUserList.IslandIndex] = new PettableIslandUser(PetServices, entry);
    }
}
