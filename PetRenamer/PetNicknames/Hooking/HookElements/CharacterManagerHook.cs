using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Runtime.CompilerServices;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class CharacterManagerHook : HookableElement
{
    private const uint PlayerMaxInObjectTable = 100;
    private const byte IslandPetSubKind       = 10;
    
    private readonly Hook<Companion.Delegates.OnInitialize>?    OnInitializeCompanionHook;
    private readonly Hook<Companion.Delegates.Terminate>?       OnTerminateCompanionHook;
    private readonly Hook<BattleChara.Delegates.OnInitialize>?  OnInitializeBattleCharaHook;
    private readonly Hook<BattleChara.Delegates.Terminate>?     OnTerminateBattleCharaHook;
    private readonly Hook<BattleChara.Delegates.Dtor>?          OnDestroyBattleCharaHook;

    private readonly IPettableDatabase                          Database;
    private readonly ILegacyDatabase                            LegacyDatabase;
    private readonly ISharingDictionary                         SharingDictionary;

    private readonly nint[] _temporaryPets = new nint[PlayerMaxInObjectTable];
    
    public CharacterManagerHook(DalamudServices services, IPetServices petServices, IPettableDatabase database, ILegacyDatabase legacyDatabase, ISharingDictionary sharingDictionary) 
        : base(services, petServices)
    {
        Database          = database;
        LegacyDatabase    = legacyDatabase;
        SharingDictionary = sharingDictionary;

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
        PetServices.PetLog.LogInfo("Flooding Initial Object Table.");
        
        for (int i = 0; i < PlayerMaxInObjectTable; i++)
        {
            BattleChara* bChara = CharacterManager.Instance()->BattleCharas[i];

            if (bChara == null)
            {
                continue;
            }
            
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

        _ = DalamudServices.Framework.Run(() =>
        {
            HandleAsCreatedCompanion(companion);
        });
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

        _ = DalamudServices.Framework.Run(() => 
        {
            HandleAsCreated(bChara);
        });
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleAsCreatedCompanion(Companion* companion)
        => GetOwner(companion)?.SetCompanion(companion);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleAsDeletedCompanion(Companion* companion)
        => GetOwner(companion)?.RemoveCompanion();

    private IPettableUser? GetOwner(Companion* companion)
    {
        if (companion == null)
        {
            return null;
        }

        return PetServices.UserList.GetUserFromObjectId(companion->CompanionOwnerId);
    }

    private void HandleAsCreated(BattleChara* newBattleChara)
    {
        if (newBattleChara == null)
        {
            return;
        }

        ObjectKind actualObjectKind = newBattleChara->GetObjectKind();

        if (actualObjectKind == ObjectKind.Pc)
        {
            CreateUser(newBattleChara);
        }

        if (actualObjectKind == ObjectKind.BattleNpc)
        {
            bool ownerFound = false;
            
            uint owner = newBattleChara->OwnerId;
            
            foreach (IPettableUser? user in PetServices.UserList)
            {
                if (user == null)
                {
                    continue;
                }

                if (user.ObjectId.Id == PluginConstants.InvalidId)
                {
                    continue;
                }
                
                if (user.ObjectId.ObjectId != owner)
                {
                    continue;
                }

                user.SetBattlePet(newBattleChara);

                ownerFound = true;
                
                break;
            }
            
            if (!ownerFound)
            {
                AssignNewBattleChara(newBattleChara);
            }
        }
        
        if (actualObjectKind == ObjectKind.EventNpc)
        {
            HandleAsIsland(newBattleChara);
        }
    }

    private void HandleAsIsland(BattleChara* newBattleChara)
    {
        if (!MJIManager.Instance()->IsPlayerInSanctuary)
        {
            return;
        }
        
        if (newBattleChara->SubKind != IslandPetSubKind)
        {
            return;
        }

        if (newBattleChara->HomeWorld != ushort.MaxValue)
        {
            return;
        }

        if (newBattleChara->CurrentWorld != ushort.MaxValue)
        {
            return;
        }

        if (newBattleChara->OwnerId != PluginConstants.InvalidId)
        {
            return;
        }

        PetSkeleton temporaryPetSkeleton = new PetSkeleton((uint)newBattleChara->ModelContainer.ModelCharaId, SkeletonType.Minion);

        if (PetServices.PetSheets.GetPet(temporaryPetSkeleton) == null)
        {
            return;
        }

        IPettableUser? user = PetServices.UserList[IUserList.IslandIndex];

        if (user == null)
        {
            return;
        }

        if (user is not IIslandUser islandUser)
        {
            return;
        }

        islandUser.SetBattlePet(newBattleChara);
    }

    private void HandleAsDeleted(BattleChara* newBattleChara)
    {
        if (newBattleChara == null)
        {
            return;
        }

        switch (newBattleChara->ObjectKind)
        {
            case ObjectKind.Pc:        HandleDeleteAsPc(newBattleChara);        break;
            case ObjectKind.BattleNpc: HandleDeleteAsBattleNpc(newBattleChara); break;
        }
        
        CleanBattleChara(newBattleChara);
    }
    
    private void CleanBattleChara(BattleChara* newBattleChara)
    {
        int floorIndex = CreateActualIndex(newBattleChara->ObjectIndex);
        
        _temporaryPets[floorIndex] = nint.Zero;
    }
    
    private void AssignNewBattleChara(BattleChara* newBattleChara)
    {
        int floorIndex = CreateActualIndex(newBattleChara->ObjectIndex);
        
        _temporaryPets[floorIndex] = (nint)newBattleChara;
    }
    
    private void HandleDeleteAsPc(BattleChara* newBattleChara)
    {
        int index = -1;
            
        foreach (IPettableUser? user in PetServices.UserList)
        {
            index++;
                
            if (user == null)
            {
                continue;
            }

            if (user.BattleChara != newBattleChara)
            {
                continue;
            }

            user.Dispose(Database);
                
            PetServices.UserList[index] = null;
                
            break;
        }
    }
    
    private void HandleDeleteAsBattleNpc(BattleChara* newBattleChara)
    {
        nint addressChara = (nint)newBattleChara;
        
        IPettableUser? user = PetServices.UserList.GetUser(addressChara, UserListFindType.PetMeansOwner);

        if (user == null)
        {
            return;
        }

        user.RemoveBattlePet(newBattleChara);
    }

    private void AddTempPetsToUser(IPettableUser user)
    {
        for (int i = (int)PlayerMaxInObjectTable - 1; i >= 0; i--)
        {
            nint tempPetPtr = _temporaryPets[i];

            if (tempPetPtr == nint.Zero)
            {
                continue;
            }

            BattleChara* tempPet = (BattleChara*)tempPetPtr;

            if (tempPet == null)
            {
                continue;
            }

            if (tempPet->OwnerId != user.ObjectId.ObjectId)
            {
                continue;
            }
            
            user.SetBattlePet(tempPet);
            
            _temporaryPets[i] = nint.Zero;
        }
    }

    private void CreateUser(BattleChara* newBattleChara)
    {
        int actualIndex = CreateActualIndex(newBattleChara->ObjectIndex);

        if (actualIndex < 0 || actualIndex >= PlayerMaxInObjectTable)
        {
            return;
        }

        PettableUser newUser = new PettableUser(PetServices, SharingDictionary, Database, LegacyDatabase, newBattleChara);

        PetServices.UserList[actualIndex] = newUser;

        AddTempPetsToUser(newUser);

        if (newBattleChara->CompanionData.CompanionObject == null)
        {
            return;
        }
        
        newUser.SetCompanion(newBattleChara->CompanionData.CompanionObject);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CreateActualIndex(ushort index)
        => (int)Math.Floor(index * 0.5f);
    
    protected override void OnDispose()
    {
        OnInitializeCompanionHook?.Dispose();
        OnTerminateCompanionHook?.Dispose();
        OnInitializeBattleCharaHook?.Dispose();
        OnTerminateBattleCharaHook?.Dispose();
        OnDestroyBattleCharaHook?.Dispose();
    }
}
