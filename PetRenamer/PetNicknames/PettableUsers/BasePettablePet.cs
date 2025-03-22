using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe abstract class BasePettablePet : IPettablePet
{
    public nint PetPointer { get; private set; }
    public int SkeletonID { get; init; }
    public ulong ObjectID { get; init; }
    public ushort Index { get; init; }
    public string Name { get; init; } = "";
    public string? CustomName { get; private set; }
    public IPetSheetData? PetData { get; private set; }
    public ulong Lifetime { get; private set; }
    public IPettableUser? Owner { get; private set; }
    public Vector3? EdgeColour { get; private set; }
    public Vector3? TextColour { get; private set; }

    readonly IPetServices PetServices;
    readonly IPettableDatabaseEntry Entry;
    readonly ISharingDictionary SharingDictionary;
    readonly bool AsBattlePet = false;

    public BasePettablePet(Character* pet, IPettableUser owner, ISharingDictionary sharingDictionary, IPettableDatabaseEntry entry, IPetServices petServices, bool asBattlePet = false)
    {
        PetServices = petServices;
        Entry = entry;
        AsBattlePet = asBattlePet;
        SharingDictionary = sharingDictionary;

        PetPointer = (nint)pet;

        Owner = owner;
        SkeletonID = pet->ModelContainer.ModelCharaId;
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
        EdgeColour = Entry.GetEdgeColour(SkeletonID);
        TextColour = Entry.GetTextColour(SkeletonID);

        SharingDictionary.Set(ObjectID, CustomName);
    }

    public void Dispose()
    {
        SharingDictionary.Set(ObjectID, null);
    }

    public void GetDrawColours(out Vector3? edgeColour, out Vector3? textColour)
    {
        edgeColour = null;
        textColour = null;

        int colourSetting = PetServices.Configuration.showColours;

        if (colourSetting >= 2) return;
        if (colourSetting == 1 && (!Owner?.IsLocalPlayer ?? false)) return;

        edgeColour = EdgeColour;
        textColour = TextColour;
    }
}
