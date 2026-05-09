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

internal abstract unsafe class BasePettablePet : IPettablePet
{
    public nint           Address    { get; }
    public PetSkeleton    SkeletonId { get; }
    public ulong          ObjectId   { get; }
    public ushort         Index      { get; }
    public string         Name       { get; }
    public IPetSheetData? PetData    { get; }
    public IPettableUser? Owner      { get; }

    public string?        CustomName { get; private set; }

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

        Index               = pet->GameObject.ObjectIndex;
        Name                = pet->GameObject.NameString;
        ObjectId            = pet->GetGameObjectId();
        PetData             = petServices.PetSheets.GetPet(SkeletonId);

        
        uint skeletonId     = (uint)pet->ModelContainer.ModelCharaId;
        
        if (skeletonType == SkeletonType.BattlePet && skeletonId == 1)
        {
            SkeletonId      = new PetSkeleton(skeletonId, SkeletonType.Chocobo);
        }
        else
        {
            SkeletonId      = new PetSkeleton((uint)pet->ModelContainer.ModelCharaId, skeletonType);
        }
        
#if DEBUG
        PetServices.PetLog.LogVerbose($"Just created a new pet at Address: {Address}, Index: {Index}, Name: {Name}, and the ObjectID: {ObjectId}");
#endif

        Recalculate();
    }

    public bool IsActive
        => (Owner?.IsActive ?? false);

    public void Recalculate()
    {
        CustomName = Entry.GetName(SkeletonId);

        SharingDictionary.Set(ObjectId, CustomName);
    }

    public void Dispose()
    {
#if DEBUG
        PetServices.PetLog.LogVerbose($"Just removed the Pet: {Name}, Address: {Address}, Index: {Index}, and the ObjectID: {ObjectId}");
#endif

        SharingDictionary.Set(ObjectId, null);
    }

    public void GetDrawColours(Configuration.ColourConfig colourConfig, out Vector3? edgeColour, out Vector3? textColour)
    {
        edgeColour = null;
        textColour = null;
        
        if (Owner == null || PetData == null)
        {
            return;
        }

        Owner.GetDrawColours(PetData, colourConfig, out edgeColour, out textColour);
    }
}
