using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
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
    public string? CustomName { get; }
    public bool Dirty { get; private set; } = true;

    public PettableCompanion(Companion* c, IPettableDatabaseEntry entry)
    {
        if (c == null) return;
        Touched = true;
        Companion = c;

        SkeletonID = c->Character.CharacterData.ModelCharaId;
        Index = c->Character.GameObject.ObjectIndex;
        Name = c->Character.GameObject.NameString;
        ObjectID = c->Character.EntityId;
        CustomName = entry.GetName(SkeletonID);
    }

    public void Update(nint pointer)
    {
        Touched = true; 
        Companion = (Companion*)pointer;
        Dirty = false;
    }

    public bool Compare(ref Character character)
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
