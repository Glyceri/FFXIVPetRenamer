using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;
using static PetRenamer.PetNicknames.PettableUsers.Interfaces.IPettableUser;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableUser : IPettableUser
{
    public string Name { get; private set; } = "";
    public ulong ContentID { get; private set; }
    public ushort Homeworld { get; private set; }
    public ulong ObjectID { get; private set; }
    public List<IPettablePet> PettablePets { get; } = new List<IPettablePet>();
    public nint Address { get; private set; }
    public BattleChara* BattleChara { get; }
    public bool IsActive => DataBaseEntry.IsActive;

    IPetServices PetServices { get; init; }
    public IPettableDatabaseEntry DataBaseEntry { get; private set; }
    public nint User { get; private set; }
    public uint ShortObjectID { get; private set; }
    public uint CurrentCastID { get; private set; }
    public bool IsLocalPlayer { get; private set; }

    public bool IsDirty { get; private set; }

    uint lastCast;

    public PettableUser(IPettableDatabase dataBase, IPetServices petServices, Pointer<BattleChara> battleChara)
    {
        BattleChara = battleChara.Value;
        Address = (nint)BattleChara;
        IsLocalPlayer = BattleChara->ObjectIndex == 0;
        Name = BattleChara->NameString;
        ContentID = BattleChara->ContentId;
        Homeworld = BattleChara->HomeWorld;

        ObjectID = BattleChara->GetGameObjectId();
        ShortObjectID = BattleChara->GetGameObjectId().ObjectId;
        DataBaseEntry = dataBase.GetEntry(ContentID);
        DataBaseEntry.UpdateEntry(this);
        PetServices = petServices;
        User = (nint)BattleChara;
        if (IsLocalPlayer) DataBaseEntry.UpdateContentID(ContentID, true);
    }

    public void Set(Pointer<BattleChara> pointer)
    {
        Reset();
        User = (nint)pointer.Value;
        CurrentCastID = BattleChara->CastInfo.ActionId;
        if (lastCast != CurrentCastID) OnLastCastChanged(CurrentCastID);
        if (!DataBaseEntry.IsActive) return;
        if (pointer.Value == null) return;
        Companion* c = pointer.Value->CompanionData.CompanionObject;
        if (c == null) return;

        IPettablePet? storedPet = FindPet(ref c->Character);
        if (storedPet != null) storedPet.Update((nint)c);
        else CreateNewPet(new PettableCompanion(c, this, DataBaseEntry, PetServices));
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

    IPettablePet? FindPet(ref Character character)
    {
        for (int i = 0; i < PettablePets.Count; i++)
        {
            IPettablePet pet = PettablePets[i];
            if (pet.Compare(ref character)) return pet;
        }
        return null;
    }

    void Reset()
    {
        IsDirty = false;

        if (DataBaseEntry.IsDirty)
        {
            foreach(IPettablePet pet in PettablePets)
            {
                pet.Recalculate();
            }
        }

        for (int i = PettablePets.Count - 1; i >= 0; i--)
        {
            IPettablePet pet = PettablePets[i];

            if (!pet.Touched)
            {
                IsDirty = true;
                PettablePets.RemoveAt(i);
                continue;
            }

            pet.Touched = false;
        }
    }

    public void CalculateBattlepets(ref List<Pointer<BattleChara>> pets)
    {
        if (!DataBaseEntry.IsActive) return;

        for (int i = pets.Count - 1; i >= 0; i--)
        {
            Pointer<BattleChara> bChara = pets[i];
            if (bChara == null) continue;
            if (bChara.Value->OwnerId != ShortObjectID) continue;

            pets.RemoveAt(i);

            IPettablePet? storedPet = FindPet(ref bChara.Value->Character);
            if (storedPet != null) storedPet.Update((nint)bChara.Value);
            else CreateNewPet(new PettableBattlePet(bChara.Value, this, DataBaseEntry, PetServices));
        }
    }

    void CreateNewPet(IPettablePet pet)
    {
        IsDirty = true;
        PettablePets.Add(pet);
    }

    public IPettablePet? GetPet(nint pet)
    {
        if (!IsActive) return null;
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
        if (!IsActive) return null;
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
        ulong lastLifetime = ulong.MaxValue;
        IPettablePet? lastPet = null;
        if (!IsActive) return null;
        int petCount = PettablePets.Count;
        for (int i = 0; i < petCount; i++)
        {
            IPettablePet pPet = PettablePets[i];
            if (filter != PetFilter.None)
            {
                if (filter != PetFilter.Minion && pPet is PettableCompanion) continue;
                if (filter != PetFilter.BattlePet && filter != PetFilter.Chocobo && pPet is PettableBattlePet) continue;
                if (filter == PetFilter.BattlePet && !PetServices.PetSheets.IsValidBattlePet(pPet.SkeletonID)) continue;
                if (filter == PetFilter.Chocobo && PetServices.PetSheets.IsValidBattlePet(pPet.SkeletonID)) continue;
            }

            if (pPet.Lifetime < lastLifetime)
            {
                lastLifetime = pPet.Lifetime;
                lastPet = pPet;
            }
        }
        return lastPet;
    }

    bool CastCheck(uint castID) => lastCast == castID && CurrentCastID != castID;

    public string? GetCustomName(IPetSheetData sheetData) => DataBaseEntry.GetName(sheetData.Model);

    public void RefreshCast()
    {
        if (BattleChara == null) return;
        CurrentCastID = BattleChara->CastInfo.ActionId;
    }
}
