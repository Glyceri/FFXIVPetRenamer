using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;
using static PetRenamer.PetNicknames.PettableUsers.Interfaces.IPettableUser;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableUser : IPettableUser
{
    public string Name { get; } = string.Empty;
    public ulong ContentID { get; }
    public ushort Homeworld { get; }
    public ulong ObjectID { get; }
    public List<IPettablePet> PettablePets { get; } = new List<IPettablePet>();
    public nint Address { get; private set; }
    public BattleChara* BattleChara { get; }
    public bool IsActive => DataBaseEntry.IsActive;

    public IPettableDatabaseEntry DataBaseEntry { get; }
    public uint ShortObjectID { get; }
    public uint CurrentCastID { get; private set; }
    public bool IsLocalPlayer { get; }

    uint lastCast;

    readonly IPetServices PetServices;
    readonly IPettableDirtyListener DirtyListener;
    readonly IPettableDirtyCaller DirtyCaller;
    readonly ISharingDictionary SharingDictionary;

    public PettableUser(ISharingDictionary sharingDictionary, IPettableDatabase dataBase, ILegacyDatabase legacyDatabase, IPetServices petServices, IPettableDirtyListener dirtyListener, IPettableDirtyCaller dirtyCaller, BattleChara* battleChara)
    {
        PetServices = petServices;
        DirtyListener = dirtyListener;
        SharingDictionary = sharingDictionary;
        DirtyCaller = dirtyCaller;

        DirtyListener.RegisterOnClearEntry(OnDirty);
        DirtyListener.RegisterOnDirtyEntry(OnDirty);
        DirtyListener.RegisterOnDirtyName(OnDirty);

        BattleChara = battleChara;
        Address = (nint)BattleChara;
        IsLocalPlayer = BattleChara->ObjectIndex == 0;
        Name = BattleChara->NameString;
        ContentID = BattleChara->ContentId;
        Homeworld = BattleChara->HomeWorld;

        ObjectID = BattleChara->GetGameObjectId();
        ShortObjectID = BattleChara->GetGameObjectId().ObjectId;

        IPettableDatabaseEntry? legacyEntry = legacyDatabase.GetEntry(Name, Homeworld, false);
        if (legacyEntry != null)
        {
            legacyEntry.UpdateContentID(ContentID, true);
            legacyDatabase.RemoveEntry(legacyEntry);
            legacyEntry.MoveToDataBase(dataBase);
            legacyDatabase.SetDirty();
        }

        DataBaseEntry = dataBase.GetEntry(ContentID);
        DataBaseEntry.UpdateEntry(this);

        if (IsLocalPlayer) DataBaseEntry.UpdateContentID(ContentID, true);
    }

    public void Update()
    {
        CurrentCastID = BattleChara->CastInfo.ActionId;
        if (lastCast != CurrentCastID) OnLastCastChanged(CurrentCastID);
    }

    public void OnLastCastChanged(uint cast)
    {
        if (!IsActive) return;
        CurrentCastID = cast;
        if (lastCast == CurrentCastID) return;

        int? softIndex = PetServices.PetSheets.CastToSoftIndex(lastCast);
        lastCast = CurrentCastID;
        if (CurrentCastID != 0) return;
        if (softIndex == null) return;

        int sIndex = softIndex.Value;
        IPettablePet? youngestPet = GetYoungestPet(PetFilter.BattlePet);
        if (youngestPet == null) return;

        DataBaseEntry.SetSoftSkeleton(sIndex, youngestPet.SkeletonID);
    }

    IPettablePet? FindPet()
    {
        for (int i = 0; i < PettablePets.Count; i++)
        {
            IPettablePet pet = PettablePets[i];
            if (pet.ObjectID != ObjectID) continue;

            return pet;
        }
        return null;
    }

    void OnDirty(INamesDatabase database)
    {
        if (database != DataBaseEntry.ActiveDatabase) return;
        Recalculate();
    }

    void OnDirty(IPettableDatabaseEntry entry)
    {
        if (entry != DataBaseEntry) return;
        Recalculate();
    }

    void Recalculate()
    {
        foreach (IPettablePet pet in PettablePets)
        {
            pet.Recalculate();
        }
    }

    void CreateNewPet(IPettablePet pet, int index = -1)
    {
        if (index == -1)
        {
            PettablePets.Add(pet);
        }
        else
        {
            PettablePets.Insert(index, pet);
        }
        DirtyCaller.DirtyPlayer(this);
    }

    public IPettablePet? GetPet(nint pet)
    {
        int petCount = PettablePets.Count;
        for (int i = 0; i < petCount; i++)
        {
            IPettablePet pPet = PettablePets[i];
            if (pPet.PetPointer == pet) return pPet;
        }
        return null;
    }

    public IPettablePet? GetPet(GameObjectId gameObjectId)
    {
        int petCount = PettablePets.Count;
        for (int i = 0; i < petCount; i++)
        {
            IPettablePet pPet = PettablePets[i];
            if (pPet.ObjectID == (ulong)gameObjectId) return pPet;
        }
        return null;
    }

    public IPettablePet? GetYoungestPet(PetFilter filter = PetFilter.None)
    {
        // The last pet  in the list is always the youngest
        for (int i = PettablePets.Count - 1; i >= 0; i--)
        {
            IPettablePet pPet = PettablePets[i];
            if (filter != PetFilter.None)
            {
                if (filter != PetFilter.Minion && pPet is PettableCompanion) continue;
                if (filter != PetFilter.BattlePet && filter != PetFilter.Chocobo && pPet is PettableBattlePet) continue;
                if (filter == PetFilter.BattlePet && !PetServices.PetSheets.IsValidBattlePet(pPet.SkeletonID)) continue;
                if (filter == PetFilter.Chocobo && PetServices.PetSheets.IsValidBattlePet(pPet.SkeletonID)) continue;
            }

            return pPet;
        }
        return null;
    }

    public string? GetCustomName(IPetSheetData sheetData) => DataBaseEntry.GetName(sheetData.Model);

    public void RefreshCast()
    {
        if (BattleChara == null) return;

        uint castID = BattleChara->CastInfo.ActionId;
        if (castID == 0) return;

        CurrentCastID = castID;
    }

    public void Dispose(IPettableDatabase database)
    {
        DirtyListener.UnregisterOnClearEntry(OnDirty);
        DirtyListener.UnregisterOnDirtyEntry(OnDirty);
        DirtyListener.UnregisterOnDirtyName(OnDirty);

        if (DataBaseEntry.IsIPC)
        {
            DataBaseEntry.Clear(true);
        }

        if (!IsActive)
        {
            database.RemoveEntry(DataBaseEntry);
        }
    }

    public void SetBattlePet(BattleChara* pointer)
    {
        CreateNewPet(new PettableBattlePet(pointer, this, SharingDictionary, DataBaseEntry, PetServices));
    }

    public void RemoveBattlePet(BattleChara* pointer)
    {
        if (pointer == null) return;

        for (int i = PettablePets.Count - 1; i >= 0; i--)
        {
            IPettablePet? pet = PettablePets[i];
            if (pet == null) continue;
            if (pet.ObjectID != pointer->GetGameObjectId()) continue;

            pet?.Dispose();
            PettablePets.RemoveAt(i);
        }
    }

    public void SetCompanion(Companion* companion)
    {
        RemoveCompanion(companion);
        CreateNewPet(new PettableCompanion(companion, this, SharingDictionary, DataBaseEntry, PetServices), 0);
    }

    public void RemoveCompanion(Companion* companion)
    {
        if (PettablePets.Count == 0) return;
        if (PettablePets[0] is not PettableCompanion pCompanion) return;

        pCompanion.Dispose();
        PettablePets.RemoveAt(0);
    }
}
