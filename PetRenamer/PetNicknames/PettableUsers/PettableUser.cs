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
    public bool Touched { get; set; }
    public List<IPettablePet> PettablePets { get; } = new List<IPettablePet>();
    public BattleChara* BattleChara { get; }
    public bool IsActive => DataBaseEntry.IsActive;

    IPetServices PetServices { get; init; }
    IPetLog PetLog { get; init; }
    public IPettableDatabaseEntry DataBaseEntry { get; private set; }
    public nint User { get; private set; }
    public uint ShortObjectID { get; private set; }
    public uint CurrentCastID { get; private set; }
    public bool IsLocalPlayer { get; private set; }
    public bool Destroyed { get; private set; }
    public bool IsDirty { get; private set; }
    public string HomeworldName { get; private set; }

    uint lastCast;

    public PettableUser(IPetLog petLog, IPettableDatabase dataBase, IPetServices petServices, Pointer<BattleChara> battleChara)
    {
        PetLog = petLog;
        BattleChara = battleChara.Value;
        IsLocalPlayer = BattleChara->ObjectIndex == 0;
        Name = BattleChara->NameString;
        ContentID = BattleChara->ContentId;
        Homeworld = BattleChara->HomeWorld;
        HomeworldName = petServices.PetSheets.GetWorldName(Homeworld)?? "[No Homeworld Found]";
        ObjectID = BattleChara->GetGameObjectId();
        ShortObjectID = BattleChara->GetGameObjectId().ObjectId;
        Touched = true;
        DataBaseEntry = dataBase.GetEntry(ContentID);
        DataBaseEntry.UpdateEntry(this);
        PetServices = petServices;
        User = (nint)BattleChara;
        if (IsLocalPlayer) DataBaseEntry.UpdateContentID(ContentID);
    }

    public void Destroy() => Destroyed = true;

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
        else CreateNewPet(new PettableCompanion(c, DataBaseEntry, PetServices));
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

        if (sIndex < 0 || sIndex >= DataBaseEntry.SoftSkeletons.Length) return;

        int oldSkeleton = DataBaseEntry.SoftSkeletons[sIndex];
        int newSkeleton = youngestPet.SkeletonID;

        if (oldSkeleton == newSkeleton) return;

        DataBaseEntry.SoftSkeletons[sIndex] = newSkeleton;

        if (!IsLocalPlayer) return;

        PetServices.Configuration.Save();
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
        if (DataBaseEntry.IsDirty)
        {
            DataBaseEntry.NotifySeenDirty();
            for (int i = PettablePets.Count - 1; i >= 0; i--)
            {
                PettablePets[i].Destroy();
            }
            PettablePets.Clear();
            return;
        }

        if (IsDirty) NotifyOfDirty();

        for (int i = PettablePets.Count - 1; i >= 0; i--)
        {
            IPettablePet pet = PettablePets[i];

            if (!pet.Touched)
            {
                pet.Destroy();
                PettablePets.RemoveAt(i);
                IsDirty = true;
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
            else CreateNewPet(new PettableBattlePet(bChara.Value, DataBaseEntry, PetServices));
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

    public void NotifyOfDirty()
    {
        IsDirty = false;
        DataBaseEntry.NotifySeenDirty();
    }
}
