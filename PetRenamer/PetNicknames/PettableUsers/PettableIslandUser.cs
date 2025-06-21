using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
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

    public PettableIslandUser(IPetServices petServices, IPettableDatabaseEntry entry)
    {
        PetServices = petServices;

        DataBaseEntry = entry;
        Name = entry.Name;
        ContentID = entry.ContentID;
        Homeworld = entry.Homeworld;
    }

    public void SetBattlePet(BattleChara* pointer)
    {
        PettablePets.Add(new PettableIslandPet(pointer, this, DataBaseEntry, PetServices));
    }

    public void RemoveBattlePet(BattleChara* pointer)
    {
        if (pointer == null) return;

        for (int i = PettablePets.Count - 1; i >= 0; i--)
        {
            IPettablePet? pet = PettablePets[i];
            if (pet == null) continue;
            if (pet.ObjectID != pointer->GetGameObjectId()) continue;

            PettablePets.RemoveAt(i);
        }
    }

    public IPettablePet? GetPet(nint pet)
    {
        int petCount = PettablePets.Count;
        for (int i = 0; i < petCount; i++)
        {
            IPettablePet pPet = PettablePets[i];
            if (pPet.Address == pet) return pPet;
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


    public string? GetCustomName(IPetSheetData sheetData) => DataBaseEntry.GetName(sheetData.Model);

    public void OnLastCastChanged(uint cast) { } // Unused
    public void RefreshCast() { } // Unused
    public IPettablePet? GetYoungestPet(IPettableUser.PetFilter filter = IPettableUser.PetFilter.None) => null; // Unused
    public void Dispose(IPettableDatabase d) { } // Unused
    public void Update() { } // Unused FOR NOW

    public void SetCompanion(Companion* companion) { } // Unused
    public void RemoveCompanion(Companion* companion) { } // Unused
}
