using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
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
    public GameObjectId   ObjectId   { get; }
    public IPetSheetData? PetData    { get; }
    public IPettableUser? Owner      { get; }

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
        ObjectId            = pet->GetGameObjectId();
        SkeletonId          = new PetSkeleton(pet->ModelContainer.ModelCharaId, skeletonType);
        PetData             = petServices.PetSheets.GetPet(SkeletonId);
        
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.LogVerbose($"Just created a new pet at Address: {Address}, and the ObjectID: {ObjectId}");
        }

        PetServices.DirtyCaller.DirtyPet(this);
        
        Recalculate();
    }

    public bool IsActive
        => (Owner?.IsActive ?? false);

    public void Recalculate()
    {
        Configuration.ColourMode colourMode = PetServices.Configuration.SelectedColourMode;
        
        Vector3? edgeColour = null;
        Vector3? textColour = null;
        
        if ((colourMode == Configuration.ColourMode.All) || (colourMode == Configuration.ColourMode.Personal && (Owner?.IsLocalPlayer ?? false)))
        {
            edgeColour = Entry.GetEdgeColour(SkeletonId);
            textColour = Entry.GetTextColour(SkeletonId);
        }
        
        SharingDictionary.Set(ObjectId, Entry.GetName(SkeletonId), Address, edgeColour, textColour);
    }

    public void Dispose()
    {
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.LogVerbose($"Just removed the pet at Address: {Address}, and the ObjectID: {ObjectId}");
        }

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

        Owner.GetDrawColours(PetData.Model, colourConfig, out edgeColour, out textColour);
    }
}
