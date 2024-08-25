using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe abstract class BasePettablePet : IPettablePet
{
    public bool Marked { get; set; } = false;

    public nint PetPointer { get; private set; }
    public int SkeletonID { get; init; }
    public ulong ObjectID { get; init; }
    public ushort Index { get; init; }
    public string Name { get; init; } = "";
    public string? CustomName { get; private set; }
    public IPetSheetData? PetData { get; private set; }
    public ulong Lifetime { get; private set; }
    public IPettableUser? Owner { get; private set; }

    readonly IPettableDatabaseEntry Entry;
    readonly ISharingDictionary SharingDictionary;
    readonly bool AsBattlePet = false;

    public BasePettablePet(Character* pet, IPettableUser owner, ISharingDictionary sharingDictionary, IPettableDatabaseEntry entry, IPetServices petServices, bool asBattlePet = false)
    {
        Entry = entry;
        AsBattlePet = asBattlePet;
        SharingDictionary = sharingDictionary;

        PetPointer = (nint)pet;

        Marked = true;
        Owner = owner;
        SkeletonID = pet->CharacterData.ModelCharaId;
        if (asBattlePet) SkeletonID = -SkeletonID;
        Index = pet->GameObject.ObjectIndex;
        Name = pet->GameObject.NameString;
        ObjectID = pet->GetGameObjectId();
        PetData = petServices.PetSheets.GetPet(SkeletonID);
        Recalculate();
    }

    public void Recalculate()
    {
         CustomName = Entry.GetName(SkeletonID);
         SharingDictionary.Set(ObjectID, CustomName);
    }

    public void Dispose()
    {
        SharingDictionary.Set(ObjectID, null);
    }
}
