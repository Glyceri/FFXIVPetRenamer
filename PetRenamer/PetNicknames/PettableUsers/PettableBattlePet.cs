using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

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

    public PettableBattlePet(BattleChara* battlePet)
    {
        if (battlePet == null) return;
        Touched = true;
        BattlePet = battlePet;

        SkeletonID = battlePet->Character.CharacterData.ModelCharaId;
        Index = battlePet->Character.GameObject.ObjectIndex;
        Name = battlePet->Character.GameObject.NameString;
        ObjectID = battlePet->Character.GetGameObjectId().ObjectId;
    }

    public void Update(nint pointer)
    {
        Touched = true;
        BattlePet = (BattleChara*)pointer;
    }

    public bool Compare(Character character)
    {
        int skeletonID = character.CharacterData.ModelCharaId;
        ushort index = character.GameObject.ObjectIndex;
        uint objectID = character.EntityId;

        return skeletonID == SkeletonID && index == Index && ObjectID == objectID;
    }

    public void Destroy()
    {
        
    }
}
