using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe abstract class BasePettablePet : IPettablePet
{
    public nint           Address    { get; }
    public PetSkeleton    SkeletonID { get; }
    public ulong          ObjectID   { get; }
    public ushort         Index      { get; }
    public string         Name       { get; }
    public IPetSheetData? PetData    { get; }
    public IPettableUser? Owner      { get; }

    public string?        CustomName { get; private set; }
    public Vector3?       EdgeColour { get; private set; }
    public Vector3?       TextColour { get; private set; }

    private readonly IPetServices           PetServices;
    private readonly IPettableDatabaseEntry Entry;
    private readonly ISharingDictionary     SharingDictionary;

    public BasePettablePet(Character* pet, IPettableUser owner, ISharingDictionary sharingDictionary, IPettableDatabaseEntry entry, IPetServices petServices, SkeletonType skeletonType)
    {
        PetServices         = petServices;
        Entry               = entry;
        SharingDictionary   = sharingDictionary;

        Address             = (nint)pet;

        Owner               = owner;
        SkeletonID          = new PetSkeleton((uint)pet->ModelContainer.ModelCharaId, skeletonType);

        Index               = pet->GameObject.ObjectIndex;
        Name                = pet->GameObject.NameString;
        ObjectID            = pet->GetGameObjectId();
        PetData             = petServices.PetSheets.GetPet(SkeletonID);

#if DEBUG
        PetServices.PetLog.LogVerbose($"Just created a new pet at Address: {Address}, Index: {Index}, Name: {Name}, and the ObjectID: {ObjectID}");
#endif

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
#if DEBUG
        PetServices.PetLog.LogVerbose($"Just removed the Pet: {Name}, Address: {Address}, Index: {Index}, and the ObjectID: {ObjectID}");
#endif

        SharingDictionary.Set(ObjectID, null);
    }

    public void GetDrawColours(out Vector3? edgeColour, out Vector3? textColour)
    {
        edgeColour = null;
        textColour = null;

        // This should NEVER be the case
        if (Owner == null)
        {
            return;
        }

        int colourSetting = PetServices.Configuration.showColours;

        if (colourSetting >= 2)
        {
            return;
        }

        if (colourSetting == 1 && !Owner.IsLocalPlayer)
        {
            return;
        }

        edgeColour = EdgeColour;
        textColour = TextColour;
    }
}
