using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableIslandPet : IIslandPet
{
    public bool Marked { get; set; } = false;

    public nint PetPointer { get; private set; }
    public int SkeletonID { get; init; }
    public ulong ObjectID { get; init; }
    public ushort Index { get; init; }
    public string Name { get; init; } = "";
    public string? CustomName { get; private set; }
    public IPetSheetData? PetData { get; private set; }
    public uint OldObjectID { get; init; }
    public byte PetType { get; init; }
    public ulong Lifetime { get; private set; }
    public IPettableUser? Owner { get; private set; }
    public BattleChara* BattlePet { get; }

    readonly IPettableDatabaseEntry Entry;

    public PettableIslandPet(BattleChara* pet, in IPettableUser owner, in IPettableDatabaseEntry entry, in IPetServices petServices)
    {
        Entry = entry;

        PetPointer = (nint)pet;

        Marked = true;
        Owner = owner;
        SkeletonID = pet->Character.CharacterData.ModelCharaId;
        Index = pet->Character.GameObject.ObjectIndex;
        Name = pet->Character.GameObject.NameString;
        ObjectID = pet->GetGameObjectId();
        PetType = pet->GetGameObjectId().Type;
        OldObjectID = pet->EntityId;
        CustomName = entry.GetName(SkeletonID);
        PetData = petServices.PetSheets.GetPet(SkeletonID);
    }

    public bool Compare(Character character)
    {
        int skeletonID = character.CharacterData.ModelCharaId;
        ushort index = character.GameObject.ObjectIndex;
        uint objectID = character.EntityId;

        return skeletonID == SkeletonID && index == Index && OldObjectID == objectID;
    }

    public void Recalculate() => CustomName = Entry.GetName(SkeletonID);

    public void Update(nint pointer)
    {
        Lifetime++;
        Marked = true;
        PetPointer = pointer;
    }
}
