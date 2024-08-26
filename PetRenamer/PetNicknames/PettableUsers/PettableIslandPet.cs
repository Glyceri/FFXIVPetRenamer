using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableIslandPet : IIslandPet
{
    public nint PetPointer { get; private set; }
    public int SkeletonID { get; init; }
    public ulong ObjectID { get; init; }
    public ushort Index { get; init; }
    public string Name { get; init; } = "";
    public string? CustomName { get; private set; }
    public IPetSheetData? PetData { get; private set; }
    public IPettableUser? Owner { get; private set; }
    public BattleChara* BattlePet { get; }

    readonly IPettableDatabaseEntry Entry;

    public PettableIslandPet(BattleChara* pet, IPettableUser owner, IPettableDatabaseEntry entry, IPetServices petServices)
    {
        Entry = entry;

        PetPointer = (nint)pet;

        Owner = owner;
        SkeletonID = pet->Character.CharacterData.ModelCharaId;
        Index = pet->Character.GameObject.ObjectIndex;
        Name = pet->Character.GameObject.NameString;
        ObjectID = pet->GetGameObjectId();
        CustomName = entry.GetName(SkeletonID);
        PetData = petServices.PetSheets.GetPet(SkeletonID);
    }

    public void Recalculate() => CustomName = Entry.GetName(SkeletonID);
    public void Dispose() { }
}
