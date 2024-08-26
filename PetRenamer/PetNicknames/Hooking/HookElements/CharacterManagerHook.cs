using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class CharacterManagerHook : HookableElement
{
    delegate Companion* Companion_OnInitializeDelegate(Companion* companion);
    delegate Companion* Companion_TerminateDelegate(Companion* companion);
    delegate BattleChara* BattleChara_OnInitializeDelegate(BattleChara* battleChara);
    delegate BattleChara* BattleChara_TerminateDelegate(BattleChara* battleChara);
    delegate BattleChara* BattleChara_Destroy(BattleChara* battleChara, bool freeMemory);

    [Signature("48 89 5C 24 ?? 57 48 83 EC 20 33 FF 48 8B D9 48 89 B9 ?? ?? ?? ?? 66 89 B9 ?? ?? ?? ??", DetourName = nameof(InitializeCompanion))]
    readonly Hook<Companion_OnInitializeDelegate>? OnInitializeCompanionHook = null;

    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 33 ED 48 8D 99 ?? ?? ?? ?? 48 89 A9 ?? ?? ?? ??", DetourName = nameof(TerminateCompanion))]
    readonly Hook<Companion_TerminateDelegate>? OnTerminateCompanionHook = null;

    [Signature("48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B D7", DetourName = nameof(InitializeBattleChara))]
    readonly Hook<BattleChara_OnInitializeDelegate>? OnInitializeBattleCharaHook = null;

    [Signature("40 53 48 83 EC 20 8B 91 ?? ?? ?? ?? 48 8B D9 E8 ?? ?? ?? ?? 48 8D 8B ?? ?? ?? ??", DetourName = nameof(TerminateBattleChara))]
    readonly Hook<BattleChara_TerminateDelegate>? OnTerminateBattleCharaHook = null;

    readonly IPettableDatabase Database;
    readonly ILegacyDatabase LegacyDatabase;
    readonly ISharingDictionary SharingDictionary;
    readonly IPettableDirtyCaller DirtyCaller;
    readonly IIslandHook IslandHook;

    readonly List<IntPtr> temporaryPets = new List<IntPtr>();

    public CharacterManagerHook(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, IPettableDatabase database, ILegacyDatabase legacyDatabase, ISharingDictionary sharingDictionary, IPettableDirtyCaller dirtyCaller, IIslandHook islandHook) : base(services, userList, petServices, dirtyListener) 
    { 
        Database = database;
        LegacyDatabase = legacyDatabase;
        SharingDictionary = sharingDictionary;
        DirtyCaller = dirtyCaller;
        IslandHook = islandHook;
    }

    public override void Init()
    {
        OnInitializeCompanionHook?.Enable();
        OnTerminateCompanionHook?.Enable();
        OnInitializeBattleCharaHook?.Enable();
        OnTerminateBattleCharaHook?.Enable();

        FloodInitialList();
    }

    void FloodInitialList()
    {
        for (int i = 0; i < 100; i++)
        {
            BattleChara* bChara = CharacterManager.Instance()->BattleCharas[i];
            if (bChara == null) continue;

            ObjectKind charaKind = bChara->GetObjectKind();
            if (charaKind != ObjectKind.Pc && charaKind != ObjectKind.BattleNpc) continue;
            
            HandleAsCreated(bChara);
        }
    }

    Companion* InitializeCompanion(Companion* companion)
    {
        Companion* initializedCompanion = OnInitializeCompanionHook!.Original(companion);

        DalamudServices.Framework.Run(() => HandleAsCreatedCompanion(companion));

        return initializedCompanion;
    }

    Companion* TerminateCompanion(Companion* companion)
    {
        HandleAsDeletedCompanion(companion);

        return OnTerminateCompanionHook!.Original(companion);
    }

    BattleChara* InitializeBattleChara(BattleChara* bChara)
    {
        BattleChara* initializedBattleChara = OnInitializeBattleCharaHook!.Original(bChara);

        DalamudServices.Framework.Run(() => HandleAsCreated(bChara));

        return initializedBattleChara;
    }

    BattleChara* TerminateBattleChara(BattleChara* bChara) 
    {
        HandleAsDeleted(bChara);

        return OnTerminateBattleCharaHook!.Original(bChara);
    }

    void HandleAsCreatedCompanion(Companion* companion) => GetOwner(companion)?.SetCompanion(companion);
    void HandleAsDeletedCompanion(Companion* companion) => GetOwner(companion)?.RemoveCompanion(companion);

    IPettableUser? GetOwner(Companion* companion)
    {
        if (companion == null) return null;

        return UserList.GetUserFromOwnerID(companion->CompanionOwnerId);
    }

    void HandleAsCreated(BattleChara* newBattleChara)
    {
        if (newBattleChara == null) return;

        ObjectKind actualObjectKind = newBattleChara->ObjectKind;

        if (actualObjectKind == ObjectKind.Pc)
        {
            CreateUser(newBattleChara);
        }

        if (actualObjectKind == ObjectKind.BattleNpc)
        {
            uint owner = newBattleChara->OwnerId;

            bool gotOwner = false;

            for (int i = 0; i < UserList.PettableUsers.Length; i++)
            {
                IPettableUser? user = UserList.PettableUsers[i];
                if (user == null) continue;
                if (user.ShortObjectID != owner) continue;

                user.SetBattlePet(newBattleChara);
                gotOwner = true;
                break;
            }

            if (!gotOwner)
            {
                if (!HandleAsIsland(newBattleChara))
                {
                    temporaryPets.Add((nint)newBattleChara);
                }
            }
        }
    }

    bool HandleAsIsland(BattleChara* newBattleChara)
    {
        if (!IslandHook.IsOnIsland) return false;

        if (newBattleChara->SubKind != 10) return false;
        if (newBattleChara->HomeWorld != ushort.MaxValue) return false;
        if (newBattleChara->CurrentWorld != ushort.MaxValue) return false;
        if (newBattleChara->OwnerId != 0xE0000000) return false;
        if (PetServices.PetSheets.GetPet(newBattleChara->ModelCharaId) == null) return false;

        IPettableUser? user = UserList.PettableUsers[PettableUserList.IslandIndex];
        if (user == null) return false;

        if (user is not IIslandUser islandUser) return false;

        islandUser.SetBattlePet(newBattleChara);
        return true;
    }

    void HandleAsDeleted(BattleChara* newBattleChara)
    {
        if (newBattleChara == null) return;

        nint addressChara = (nint)newBattleChara;

        ObjectKind actualObjectKind = newBattleChara->ObjectKind;

        if (actualObjectKind == ObjectKind.Pc)
        {
            for (int i = 0; i < UserList.PettableUsers.Length; i++)
            {
                IPettableUser? user = UserList.PettableUsers[i];
                if (user == null) continue;
                if (user.BattleChara != newBattleChara) continue;

                user?.Dispose(Database);
                UserList.PettableUsers[i] = null;
                break;
            }
        }

        if (actualObjectKind == ObjectKind.BattleNpc)
        {
            temporaryPets.Remove(addressChara);

            IPettableUser? user = UserList.GetUser(addressChara);
            if (user == null) return;
            
            user.RemoveBattlePet(newBattleChara);
        }
    }

    void AddTempPetsToUser(IPettableUser user)
    {
        uint userID = user.ShortObjectID;

        for (int i = temporaryPets.Count - 1; i >= 0; i--)
        {
            nint tempPetPtr = temporaryPets[i];
            if (tempPetPtr == 0) continue;

            BattleChara* tempPet = (BattleChara*)tempPetPtr;
            if (tempPet == null) continue;
            if (tempPet->OwnerId != userID) continue;

            user.SetBattlePet(tempPet);
            temporaryPets.RemoveAt(i);
        }
    }

    IPettableUser? CreateUser(BattleChara* newBattleChara)
    {
        IPettableUser? newUser = new PettableUser(SharingDictionary, Database, LegacyDatabase, PetServices, DirtyListener, DirtyCaller, newBattleChara);

        int actualIndex = CreateActualIndex(newBattleChara->ObjectIndex);
        if (actualIndex < 0 || actualIndex >= 100) return null;

        UserList.PettableUsers[actualIndex] = newUser;

        AddTempPetsToUser(newUser);

        if (newBattleChara->CompanionData.CompanionObject != null) 
        {
            newUser.SetCompanion(newBattleChara->CompanionData.CompanionObject);
        }

        return newUser;
    }

    int CreateActualIndex(ushort index) => (int)MathF.Floor(index * 0.5f);
    
    protected override void OnDispose()
    {
        OnInitializeCompanionHook?.Dispose();
        OnTerminateCompanionHook?.Dispose();
        OnInitializeBattleCharaHook?.Dispose();
        OnTerminateBattleCharaHook?.Dispose();
    }
}
