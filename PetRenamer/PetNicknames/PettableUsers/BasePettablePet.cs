using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe abstract class BasePettablePet : IPettablePet
{
    public bool Touched { get; set; } = false;

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

    readonly IPettableDatabaseEntry Entry;
    readonly ISharingDictionary SharingDictionary;
    readonly bool AsBattlePet = false;

    public BasePettablePet(Character* pet, in IPettableUser owner, in ISharingDictionary sharingDictionary, in IPettableDatabaseEntry entry, in IPetServices petServices, bool asBattlePet = false)
    {
        Entry = entry;
        AsBattlePet = asBattlePet;
        SharingDictionary = sharingDictionary;

        PetPointer = (nint)pet;

        Touched = true;
        Owner = owner;
        SkeletonID = pet->CharacterData.ModelCharaId;
        if (asBattlePet) SkeletonID = -SkeletonID;
        Index = pet->GameObject.ObjectIndex;
        Name = pet->GameObject.NameString;
        ObjectID = pet->GetGameObjectId();
        PetType = pet->GetGameObjectId().Type;
        OldObjectID = pet->EntityId;
        CustomName = entry.GetName(SkeletonID);
        PetData = petServices.PetSheets.GetPet(SkeletonID);
    }

    public void Update(nint pointer)
    {
        if (CustomName != null) SharingDictionary.Set(ObjectID, CustomName);

        Lifetime++;
        Touched = true;
        PetPointer = pointer;
    }

    public bool Compare(ref Character character)
    {
        int skeletonID = character.CharacterData.ModelCharaId;
        ushort index = character.GameObject.ObjectIndex;
        uint objectID = character.EntityId;

        if (AsBattlePet) skeletonID = -skeletonID;

        return skeletonID == SkeletonID && index == Index && OldObjectID == objectID;
    }

    public void Recalculate()
    {
         CustomName = Entry.GetName(SkeletonID);
    }
}
