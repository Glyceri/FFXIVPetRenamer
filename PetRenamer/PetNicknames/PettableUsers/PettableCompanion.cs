using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableCompanion : IPettableCompanion
{
    public Companion* Companion { get => (Companion*)PetPointer; private set => PetPointer = (nint)value; }
    public bool Touched { get; set; } = false;

    public nint PetPointer { get; private set; }
    public int SkeletonID { get; init; }
    public ulong ObjectID { get; init; }
    public uint OldObjectID { get; init; }
    public string Name { get; init; } = "";
    public ushort Index { get; init; }
    public string? CustomName { get; private set; }
    public bool Dirty { get; private set; } = true;
    public IPetSheetData? PetData { get; private set; }
    public byte PetType { get; private set; }
    public ulong Lifetime { get; private set; }
    public IPettableUser? Owner { get; private set; }

    readonly IPettableDatabaseEntry Entry;

    public PettableCompanion(Companion* c, IPettableUser owner, IPettableDatabaseEntry entry, IPetServices petServices)
    {
        Entry = entry;

        if (c == null) return;
        Touched = true;
        Companion = c;
        Owner = owner;
        SkeletonID = c->Character.CharacterData.ModelCharaId;
        Index = c->Character.GameObject.ObjectIndex;
        Name = c->Character.GameObject.NameString;
        ObjectID = c->Character.GetGameObjectId();
        OldObjectID = c->Character.EntityId;
        PetType = c->Character.GetGameObjectId().Type;
        CustomName = entry.GetName(SkeletonID);
        PetData = petServices.PetSheets.GetPet(SkeletonID);
    }

    public void Update(nint pointer)
    {
        Lifetime++;
        Touched = true; 
        Companion = (Companion*)pointer;
        Dirty = false;
    }

    public bool Compare(ref Character character)
    {
        int skeletonID = character.CharacterData.ModelCharaId;
        ushort index = character.GameObject.ObjectIndex;
        uint objectID = character.EntityId;

        return skeletonID == SkeletonID && index == Index && OldObjectID == objectID;
    }

    public void Recalculate()
    {
        CustomName = Entry?.GetName(SkeletonID);
    }
}
