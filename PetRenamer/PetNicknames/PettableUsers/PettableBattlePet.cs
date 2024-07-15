using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableBattlePet : IPettableBattlePet
{
    public BattleChara* BattlePet { get => (BattleChara*)PetPointer; private set => PetPointer = (nint)value; }
    public bool Touched { get; set; } = false;

    public nint PetPointer { get; private set; }
    public int SkeletonID { get; init; }
    public ulong ObjectID { get; init; }
    public ushort Index { get; init; }
    public string Name { get; init; } = "";
    public string? CustomName { get; private set; }
    bool _dirty = true;
    public bool Dirty { get => _dirty || Entry.IsDirtyForUI;  }
    public IPetSheetData? PetData { get; private set; }
    public uint OldObjectID { get; init; }
    public byte PetType { get; init; }
    public ulong Lifetime { get; private set; }
    public IPettableUser? Owner { get; private set; }

    readonly IPettableDatabaseEntry Entry;

    public PettableBattlePet(BattleChara* battlePet, IPettableUser owner, IPettableDatabaseEntry entry, IPetServices petServices)
    {
        Entry = entry;
        Touched = true;
        BattlePet = battlePet;
        Owner = owner;
        SkeletonID = -battlePet->Character.CharacterData.ModelCharaId;
        Index = battlePet->Character.GameObject.ObjectIndex;
        Name = battlePet->Character.GameObject.NameString;
        ObjectID = battlePet->Character.GetGameObjectId();
        PetType = battlePet->Character.GetGameObjectId().Type;
        OldObjectID = battlePet->Character.EntityId;
        CustomName = entry.GetName(SkeletonID);
        PetData = petServices.PetSheets.GetPet(SkeletonID);
    }

    public void Update(nint pointer)
    {
        Lifetime++;
        Touched = true;
        BattlePet = (BattleChara*)pointer;
        _dirty = false;
    }

    public bool Compare(ref Character character)
    {
        int skeletonID = character.CharacterData.ModelCharaId;
        ushort index = character.GameObject.ObjectIndex;
        uint objectID = character.EntityId;

        return -skeletonID == SkeletonID && index == Index && OldObjectID == objectID;
    }

    public void Recalculate()
    {
        CustomName = Entry?.GetName(SkeletonID);
    }
}
