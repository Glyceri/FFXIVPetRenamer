using Dalamud.Hooking;
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
    private readonly Hook<Companion.Delegates.OnInitialize>? OnInitializeCompanionHook;
    private readonly Hook<Companion.Delegates.Terminate>? OnTerminateCompanionHook;
    private readonly Hook<BattleChara.Delegates.OnInitialize> OnInitializeBattleCharaHook;
    private readonly Hook<BattleChara.Delegates.Terminate> OnTerminateBattleCharaHook;
    private readonly Hook<BattleChara.Delegates.Dtor> OnDestroyBattleCharaHook;

    private readonly IPettableDatabase Database;
    private readonly ILegacyDatabase LegacyDatabase;
    private readonly ISharingDictionary SharingDictionary;
    private readonly IPettableDirtyCaller DirtyCaller;
    private readonly IIslandHook IslandHook;

    private readonly List<nint> temporaryPets = new List<nint>();

    public CharacterManagerHook(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, IPettableDatabase database, ILegacyDatabase legacyDatabase, ISharingDictionary sharingDictionary, IPettableDirtyCaller dirtyCaller, IIslandHook islandHook) : base(services, userList, petServices, dirtyListener)
    {
        Database            = database;
        LegacyDatabase      = legacyDatabase;
        SharingDictionary   = sharingDictionary;
        DirtyCaller         = dirtyCaller;
        IslandHook          = islandHook;

        OnInitializeCompanionHook   = DalamudServices.Hooking.HookFromAddress<Companion.Delegates.OnInitialize>     ((nint)Companion.StaticVirtualTablePointer->OnInitialize,       InitializeCompanion);
        OnTerminateCompanionHook    = DalamudServices.Hooking.HookFromAddress<Companion.Delegates.Terminate>        ((nint)Companion.StaticVirtualTablePointer->Terminate,          TerminateCompanion);
        OnInitializeBattleCharaHook = DalamudServices.Hooking.HookFromAddress<BattleChara.Delegates.OnInitialize>   ((nint)BattleChara.StaticVirtualTablePointer->OnInitialize,     InitializeBattleChara);
        OnTerminateBattleCharaHook  = DalamudServices.Hooking.HookFromAddress<BattleChara.Delegates.Terminate>      ((nint)BattleChara.StaticVirtualTablePointer->Terminate,        TerminateBattleChara);
        OnDestroyBattleCharaHook    = DalamudServices.Hooking.HookFromAddress<BattleChara.Delegates.Dtor>           ((nint)BattleChara.StaticVirtualTablePointer->Dtor,             DestroyBattleChara);
    }

    public override void Init()
    {
        OnInitializeCompanionHook?.Enable();
        OnTerminateCompanionHook?.Enable();
        OnInitializeBattleCharaHook?.Enable();
        OnTerminateBattleCharaHook?.Enable();
        OnDestroyBattleCharaHook?.Enable();

        FloodInitialList();
    }

    private void FloodInitialList()
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

    private void InitializeCompanion(Companion* companion)
    {
        try
        {
            OnInitializeCompanionHook!.OriginalDisposeSafe(companion);
        }
        catch (Exception e)
        {
            PetServices.PetLog.LogException(e);
        }

        DalamudServices.Framework.Run(() => HandleAsCreatedCompanion(companion));
    }

    private void TerminateCompanion(Companion* companion)
    {
        HandleAsDeletedCompanion(companion);

        try
        {
            OnTerminateCompanionHook!.OriginalDisposeSafe(companion);
        }
        catch (Exception e)
        {
            PetServices.PetLog.LogException(e);
        }
    }

    private void InitializeBattleChara(BattleChara* bChara)
    {
        try
        {
            OnInitializeBattleCharaHook!.OriginalDisposeSafe(bChara);
        }
        catch (Exception e)
        {
            PetServices.PetLog.LogException(e);
        }

        DalamudServices.Framework.Run(() => HandleAsCreated(bChara));
    }

    private void TerminateBattleChara(BattleChara* bChara)
    {
        HandleAsDeleted(bChara);

        try
        {
            OnTerminateBattleCharaHook!.OriginalDisposeSafe(bChara);
        }
        catch (Exception e)
        {
            PetServices.PetLog.LogException(e);
        }
    }

    private GameObject* DestroyBattleChara(BattleChara* bChara, byte freeMemory)
    {
        HandleAsDeleted(bChara);

        try
        {
            return OnDestroyBattleCharaHook!.OriginalDisposeSafe(bChara, freeMemory);
        }
        catch (Exception e)
        {
            PetServices.PetLog.LogException(e);
        }

        return null;
    }

    private void HandleAsCreatedCompanion(Companion* companion) => GetOwner(companion)?.SetCompanion(companion);
    private void HandleAsDeletedCompanion(Companion* companion) => GetOwner(companion)?.RemoveCompanion(companion);

    private IPettableUser? GetOwner(Companion* companion)
    {
        if (companion == null) return null;

        return UserList.GetUserFromOwnerID(companion->CompanionOwnerId);
    }

    private void HandleAsCreated(BattleChara* newBattleChara)
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

    private bool HandleAsIsland(BattleChara* newBattleChara)
    {
        if (!IslandHook.IsOnIsland)                                                             return false;

        if (newBattleChara->SubKind         != 10)                                              return false;
        if (newBattleChara->HomeWorld       != ushort.MaxValue)                                 return false;
        if (newBattleChara->CurrentWorld    != ushort.MaxValue)                                 return false;
        if (newBattleChara->OwnerId         != 0xE0000000)                                      return false;
        if (PetServices.PetSheets.GetPet(newBattleChara->ModelContainer.ModelCharaId) == null)  return false;

        IPettableUser? user = UserList.PettableUsers[PettableUserList.IslandIndex];
        if (user == null)
        {
            return false;
        }

        if (user is not IIslandUser islandUser)
        {
            return false;
        }

        islandUser.SetBattlePet(newBattleChara);

        return true;
    }

    private void HandleAsDeleted(BattleChara* newBattleChara)
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

    private void AddTempPetsToUser(IPettableUser user)
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

    private IPettableUser? CreateUser(BattleChara* newBattleChara)
    {
        int actualIndex = CreateActualIndex(newBattleChara->ObjectIndex);
        if (actualIndex < 0 || actualIndex >= 100) return null;

        PettableUser newUser = new PettableUser(SharingDictionary, Database, LegacyDatabase, PetServices, DirtyListener, DirtyCaller, newBattleChara);

        UserList.PettableUsers[actualIndex] = newUser;

        AddTempPetsToUser(newUser);

        if (newBattleChara->CompanionData.CompanionObject != null)
        {
            newUser.SetCompanion(newBattleChara->CompanionData.CompanionObject);
        }

        return newUser;
    }

    private int CreateActualIndex(ushort index) => (int)MathF.Floor(index * 0.5f);

    protected override void OnDispose()
    {
        OnInitializeCompanionHook?.Dispose();
        OnTerminateCompanionHook?.Dispose();
        OnInitializeBattleCharaHook?.Dispose();
        OnTerminateBattleCharaHook?.Dispose();
        OnDestroyBattleCharaHook?.Dispose();
    }
}
