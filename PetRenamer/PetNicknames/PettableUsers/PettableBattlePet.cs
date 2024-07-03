using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Data.Parsing.Layer;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableBattlePet : IPettableBattlePet
{
    public BattleChara* BattlePet { get => (BattleChara*)PetPointer; private set => PetPointer = (nint)value; }
    public bool Touched { get; set; } = false;

    public nint PetPointer { get; private set; }
    public int SkeletonID { get; init; }
    public uint ObjectID { get; init; }
    public ushort Index { get; init; }
    public string Name { get; init; } = "";
    public string? CustomName { get; }
    public bool Dirty { get; private set; } = true;
    public string? CustomSoftName { get; }
    public PetSheetData? PetData { get; private set; }
    public uint OldObjectID { get; init; }
    public byte PetType { get; init; }

    public PettableBattlePet(BattleChara* battlePet, IPettableDatabaseEntry entry, IPetServices petServices)
    {
        if (battlePet == null) return;
        Touched = true;
        BattlePet = battlePet;

        SkeletonID = -battlePet->Character.CharacterData.ModelCharaId;
        Index = battlePet->Character.GameObject.ObjectIndex;
        Name = battlePet->Character.GameObject.NameString;
        ObjectID = battlePet->Character.GetGameObjectId().ObjectId;
        PetType = battlePet->Character.GetGameObjectId().Type;
        OldObjectID = battlePet->Character.EntityId;
        CustomName = entry.GetName(SkeletonID);
        PetData = petServices.PetSheets.GetPet(SkeletonID);
    }

    public void Update(nint pointer)
    {
        Touched = true;
        BattlePet = (BattleChara*)pointer;
        Dirty = false;
    }

    public bool Compare(ref Character character)
    {
        int skeletonID = character.CharacterData.ModelCharaId;
        ushort index = character.GameObject.ObjectIndex;
        uint objectID = character.EntityId;

        return -skeletonID == SkeletonID && index == Index && ObjectID == objectID;
    }

    public void Destroy()
    {
        
    }
}
