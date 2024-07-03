using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableCompanion : IPettableCompanion
{
    public Companion* Companion { get => (Companion*)PetPointer; private set => PetPointer = (nint)value; }
    public bool Touched { get; set; } = false;

    public nint PetPointer { get; private set; }
    public int SkeletonID { get; init; }
    public uint ObjectID { get; init; }
    public string Name { get; init; } = "";
    public ushort Index { get; init; }

    public PettableCompanion(Companion* c)
    {
        if (c == null) return;
        Touched = true;
        Companion = c;

        SkeletonID = c->Character.CharacterData.ModelCharaId;
        Index = c->Character.GameObject.ObjectIndex;
        Name = c->Character.GameObject.NameString;
        ObjectID = c->Character.EntityId;
    }

    public void Update(nint pointer)
    {
        Touched = true; 
        Companion = (Companion*)pointer;
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
