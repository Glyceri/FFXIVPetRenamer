using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableIslandUser : IIslandUser
{
    public bool IsActive { get; } = true;
    public bool IsLocalPlayer { get; } = false;
    public bool IsDirty { get; } = false;
    public IPettableDatabaseEntry DataBaseEntry { get; }
    public List<IPettablePet> PettablePets { get; } = new List<IPettablePet>();
    public string Name { get; }
    public ulong ContentID { get; }
    public ushort Homeworld { get; }
    public ulong ObjectID { get; } = 0;
    public uint ShortObjectID { get; } = 0;
    public uint CurrentCastID { get; } = 0;
    public nint Address { get; } = 0;
    public unsafe BattleChara* BattleChara { get; } = null;

    readonly IPetServices PetServices;

    public PettableIslandUser(in IPetServices petServices, IPettableDatabaseEntry entry)
    {
        PetServices = petServices;

        DataBaseEntry = entry;
        Name = entry.Name;
        ContentID = entry.ContentID;
        Homeworld = entry.Homeworld;
    }

    public void Set(Pointer<BattleChara> pointer) => Reset();

    public void CalculateBattlepets(ref List<Pointer<BattleChara>> pets)
    {
        for (int i = pets.Count - 1; i >= 0; i--)
        {
            Pointer<BattleChara> bChara = pets[i];
            if (bChara == null) continue;

            pets.RemoveAt(i);

            IPettablePet? storedPet = FindPet(ref bChara.Value->Character);
            if (storedPet != null)
            {
                storedPet.Update((nint)bChara.Value);
            }
            else
            {
                PettablePets.Add(new PettableIslandPet(bChara.Value, this, DataBaseEntry, PetServices));
            }
        }
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
        for (int i = PettablePets.Count - 1; i >= 0; i--)
        {
            IPettablePet pet = PettablePets[i];

            if (!pet.Marked)
            {
                PettablePets.RemoveAt(i);
                continue;
            }

            pet.Marked = false;
        }
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


    public string? GetCustomName(IPetSheetData sheetData) => DataBaseEntry.GetName(sheetData.Model);

    public void OnLastCastChanged(uint cast) { } // Unused
    public void RefreshCast() { } // Unused
    public IPettablePet? GetYoungestPet(IPettableUser.PetFilter filter = IPettableUser.PetFilter.None) => null; // Unused
    public void Dispose() { } // Unused
}
