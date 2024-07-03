using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableUser : IPettableUser
{
    public string Name { get; private set; } = "";
    public ulong ContentID { get; private set; }
    public ushort Homeworld { get; private set; }
    public uint ObjectID { get; private set; }
    public bool Touched { get; set; }
    public List<IPettablePet> PettablePets { get; } = new List<IPettablePet>();
    public BattleChara* BattleChara { get; }
    public bool IsActive => DataBaseEntry.IsActive;

    IPetLog PetLog { get; init; }
    public IPettableDatabaseEntry DataBaseEntry { get; private set; }
    
    public PettableUser(IPetLog petLog, IPettableDatabase dataBase, Pointer<BattleChara> battleChara)
    {
        this.PetLog = petLog;
        BattleChara* bChara = battleChara.Value;
        Name = bChara->NameString;
        ContentID = bChara->ContentId;
        Homeworld = bChara->HomeWorld;
        ObjectID = bChara->GetGameObjectId().ObjectId;
        Touched = true;
        DataBaseEntry = dataBase.GetEntry(ContentID);
        DataBaseEntry.UpdateEntry(this);
    }

    public void Destroy()
    {
        
    }

    public void Set(Pointer<BattleChara> pointer)
    {
        Reset();
        if (!DataBaseEntry.IsActive) return;
        if (pointer.Value == null) return;
        Companion* c = pointer.Value->CompanionData.CompanionObject;
        if (c == null) return;

        IPettablePet? storedPet = FindPet(ref c->Character);
        if (storedPet != null) storedPet.Update((nint)c);
        else CreateNewPet(new PettableCompanion(c, DataBaseEntry));
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
            if (bChara.Value->OwnerId != ObjectID) continue;

            pets.RemoveAt(i);

            IPettablePet? storedPet = FindPet(ref bChara.Value->Character);
            if (storedPet != null) storedPet.Update((nint)bChara.Value);
            else CreateNewPet(new PettableBattlePet(bChara.Value, DataBaseEntry));
        }
    }

    void CreateNewPet(IPettablePet pet)
    {
        PettablePets.Add(pet);
    }

    public IPettablePet? GetPet(nint pet)
    {
        foreach(IPettablePet pPet in PettablePets)
        {
            if (pPet.PetPointer == pet) return pPet;
        }
        return null;
    }
}
