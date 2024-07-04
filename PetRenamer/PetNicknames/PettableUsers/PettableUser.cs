using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

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

    public PettableUser(IPetLog petLog, IPettableDatabase dataBase, IPetServices petServices, Pointer<BattleChara> battleChara)
    {
        this.PetLog = petLog;
        BattleChara = battleChara.Value;
        Name = BattleChara->NameString;
        ContentID = BattleChara->ContentId;
        Homeworld = BattleChara->HomeWorld;
        ObjectID = BattleChara->GetGameObjectId();
        ShortObjectID = BattleChara->GetGameObjectId().ObjectId;
        Touched = true;
        DataBaseEntry = dataBase.GetEntry(ContentID);
        DataBaseEntry.UpdateEntry(this);
        PetServices = petServices;
        User = (nint)BattleChara;
    }

    public void Destroy()
    {
        
    }

    public void Set(Pointer<BattleChara> pointer)
    {
        Reset();
        User = (nint)pointer.Value;
        if (!DataBaseEntry.IsActive) return;
        if (pointer.Value == null) return;
        Companion* c = pointer.Value->CompanionData.CompanionObject;
        if (c == null) return;

        IPettablePet? storedPet = FindPet(ref c->Character);
        if (storedPet != null) storedPet.Update((nint)c);
        else CreateNewPet(new PettableCompanion(c, DataBaseEntry, PetServices));
    }

    IPettablePet? FindPet(ref Character character)
    {
        for(int i = 0; i < PettablePets.Count; i++)
        {
            IPettablePet pet = PettablePets[i];
            if (pet.Compare(ref character)) return pet;
        }
        return null;
    }

    void Reset()
    {
        if (DataBaseEntry.Dirty)
        {
            DataBaseEntry.NotifySeenDirty();
            for (int i = PettablePets.Count - 1; i >= 0; i--)
            {
                PettablePets[i].Destroy();
            }
            PettablePets.Clear();
            return;
        }

        for (int i = PettablePets.Count - 1; i >= 0; i--) 
        {
            IPettablePet pet = PettablePets[i];

            if (!pet.Touched) 
            {
                pet.Destroy();
                PettablePets.RemoveAt(i); 
                continue; 
            }

            pet.Touched = false;
        }
    }

    public void CalculateBattlepets(ref List<Pointer<BattleChara>> pets)
    {
        if (!DataBaseEntry.IsActive) return;

        for(int i = pets.Count - 1; i >= 0; i--)
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
        foreach (IPettablePet pPet in PettablePets)
        {
            if (pPet.ObjectID == (ulong)gameObjectId) return pPet;
        }
        return null;
    }
}
