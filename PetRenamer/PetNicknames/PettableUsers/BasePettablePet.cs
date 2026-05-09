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
        SkeletonId          = new PetSkeleton(pet->ModelContainer.ModelCharaId, skeletonType);
        PetData             = petServices.PetSheets.GetPet(SkeletonId);
        
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.LogVerbose($"Just created a new pet at Address: {Address}, Index: {Index}, Name: {Name}, and the ObjectID: {ObjectId}");
        }

        Recalculate();
    }

    public bool IsActive
        => (Owner?.IsActive ?? false);

    public void Recalculate()
    {
        CustomName = Entry.GetName(SkeletonId);

        Configuration.ColourMode colourMode = PetServices.Configuration.SelectedColourMode;
        
        Vector3? edgeColour = null;
        Vector3? textColour = null;
        
        if ((colourMode == Configuration.ColourMode.All) || (colourMode == Configuration.ColourMode.Personal && (Owner?.IsLocalPlayer ?? false)))
        {
            edgeColour = Entry.GetEdgeColour(SkeletonId);
            textColour = Entry.GetTextColour(SkeletonId);
        }
        
        SharingDictionary.Set(ObjectId, CustomName, Address, edgeColour, textColour);
    }

    public void Dispose()
    {
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.LogVerbose($"Just removed the Pet: {Name}, Address: {Address}, Index: {Index}, and the ObjectID: {ObjectId}");
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

        Owner.GetDrawColours(PetData, colourConfig, out edgeColour, out textColour);
    }
}
