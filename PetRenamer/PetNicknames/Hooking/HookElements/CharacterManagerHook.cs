using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
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
    public delegate BattleChara* CharacterManager_CreateCharacterAtFirstEmptyIndexDelegate(CharacterManager* characterManager, uint entityID);
    public delegate BattleChara* CharacterManager_CreateCharacterAtIndexDelegate(CharacterManager* characterManager, uint entityID, int index, uint layoutID);
    public delegate void CharacterManager_DeleteCharacterAtIndexDelegate(CharacterManager* characterManager, int index);
    public delegate nint CharacterManager_DeleteAllCharactersDelegate(CharacterManager* characterManager);
    public delegate Companion* Companion_OnInitializeDelegate(Companion* companion);
    public delegate Companion* Companion_TerminateDelegate(Companion* companion);

    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 56 57 41 56 48 83 EC 20 33 DB 48 8D 71 50", DetourName = nameof(CharacterManager_CreateCharacterAtFirstEmptyIndexDetour))]
    readonly Hook<CharacterManager_CreateCharacterAtFirstEmptyIndexDelegate>? CreateCharacterAtFirstEmptyIndexHook = null;

    [Signature("E8 ?? ?? ?? ?? 48 85 C0 74 50 8B 13", DetourName = nameof(CharacterManager_CreateCharacterAtIndexDetour))]
    readonly Hook<CharacterManager_CreateCharacterAtIndexDelegate>? CreateCharacterAtIndexHook = null;

    [Signature("83 FA 64 0F 8D ?? ?? ?? ?? 48 89 74 24 ??", DetourName = nameof(CharacterManager_DeleteCharacterAtIndexDetour))]
    readonly Hook<CharacterManager_DeleteCharacterAtIndexDelegate>? DeleteCharacterAtIndexHook = null;

    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 41 56 41 57 48 83 EC 20 45 33 FF 48 8D 79 50", DetourName = nameof(CharacterManager_DeleteAllCharactersDetour))]
    readonly Hook<CharacterManager_DeleteAllCharactersDelegate>? DeleteAllCharactersHook = null;

    [Signature("48 89 5C 24 ?? 57 48 83 EC 20 33 FF 48 8B D9 48 89 B9 ?? ?? ?? ?? 66 89 B9 ?? ?? ?? ??", DetourName = nameof(InitializeCompanion))]
    readonly Hook<Companion_OnInitializeDelegate>? OnInitializeHook = null;

    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 33 ED 48 8D 99 ?? ?? ?? ?? 48 89 A9 ?? ?? ?? ??", DetourName = nameof(TerminateCopmanion))]
    readonly Hook<Companion_TerminateDelegate>? CompanionTerminateHook = null;

    readonly IPettableDatabase Database;
    readonly ILegacyDatabase LegacyDatabase;
    readonly ISharingDictionary SharingDictionary;

    readonly List<IntPtr> temporaryPets = new List<IntPtr>();

    public CharacterManagerHook(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, IPettableDatabase database, ILegacyDatabase legacyDatabase, ISharingDictionary sharingDictionary) : base(services, userList, petServices, dirtyListener) 
    { 
        Database = database;
        LegacyDatabase = legacyDatabase;
        SharingDictionary = sharingDictionary;
    }

    public override void Init()
    {
        CreateCharacterAtFirstEmptyIndexHook?.Enable();
        CreateCharacterAtIndexHook?.Enable();
        DeleteCharacterAtIndexHook?.Enable();
        DeleteAllCharactersHook?.Enable();
        OnInitializeHook?.Enable();
        CompanionTerminateHook?.Enable();

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
        Companion* initializedCompanion = OnInitializeHook!.Original(companion);

        DalamudServices.Framework.Run(() => HandleAsCreatedCompanion(companion));

        return initializedCompanion;
    }

    Companion* TerminateCopmanion(Companion* companion)
    {
        HandleAsDeletedCompanion(companion);

        return CompanionTerminateHook!.Original(companion);
    }

    BattleChara* CharacterManager_CreateCharacterAtFirstEmptyIndexDetour(CharacterManager* characterManager, uint entityID)
    {
        BattleChara* newBattleChara = CreateCharacterAtFirstEmptyIndexHook!.Original(characterManager, entityID);

        DalamudServices.Framework.Run(() => HandleAsCreated(newBattleChara));

        return newBattleChara;
    }

    BattleChara* CharacterManager_CreateCharacterAtIndexDetour(CharacterManager* characterManager, uint entityID, int index, uint layoutID)
    {
        BattleChara* newBattleChara = CreateCharacterAtIndexHook!.Original(characterManager, entityID, index, layoutID);

        DalamudServices.Framework.Run(() => HandleAsCreated(newBattleChara));

        return newBattleChara;
    }

    void CharacterManager_DeleteCharacterAtIndexDetour(CharacterManager* characterManager, int index)
    {
        HandleAsDeleted(index);
        DeleteCharacterAtIndexHook!.Original(characterManager, index);
    }

    nint CharacterManager_DeleteAllCharactersDetour(CharacterManager* characterManager)
    {
        HandleAllDeleted();

        return DeleteAllCharactersHook!.Original(characterManager);
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
                temporaryPets.Add((nint)newBattleChara);
            }
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
        IPettableUser? newUser = new PettableUser(SharingDictionary, Database, LegacyDatabase, PetServices, DirtyListener, newBattleChara);

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

    int CreateActualIndex(ushort index)
    {
        return (int)MathF.Floor(index * 0.5f);
    }

    void HandleAllDeleted()
    {
        for (int i = 0; i <= UserList.PettableUsers.Length; i++)
        {
            HandleAsDeleted(i);
        }
    }

    void HandleAsDeleted(int index)
    {
        if (index < 0 || index >= 100) return;

        IPettableUser? user = UserList.PettableUsers[index];
        if (user != null)
        {
            user?.Dispose(Database);
            UserList.PettableUsers[index] = null;
        }
        else
        {
            BattleChara* battleChara = CharacterManager.Instance()->BattleCharas[index];
            if (battleChara != null)
            {
                IPettablePet? pet = UserList.GetPet((nint)battleChara);
                if (pet is IPettableBattlePet battlePet)
                {
                    pet.Owner?.RemoveBattlePet(battleChara);
                }
                else
                {
                    temporaryPets.Remove((nint)battleChara);
                }
            }
        }
    }

    protected override void OnDispose()
    {
        CreateCharacterAtFirstEmptyIndexHook?.Dispose();
        CreateCharacterAtIndexHook?.Dispose();
        DeleteCharacterAtIndexHook?.Dispose();
        DeleteAllCharactersHook?.Dispose();
        OnInitializeHook?.Dispose();
        CompanionTerminateHook?.Dispose();
    }
}
