using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableIslandPet : IIslandPet
{
    public nint           Address    { get; private set; }
    public PetSkeleton    SkeletonId { get; private set; }
    public ulong          ObjectId   { get; private set; }
    public ushort         Index      { get; private set; }
    public string         Name       { get; private set; } = string.Empty;
    public string?        CustomName { get; private set; }
    public IPetSheetData? PetData    { get; private set; }
    public IPettableUser? Owner      { get; private set; }
    public BattleChara*   BattlePet  { get; private set; }

    private readonly IPettableDatabaseEntry Entry;
    private readonly IPetServices           PetServices;

    public PettableIslandPet(BattleChara* pet, IPettableUser owner, IPettableDatabaseEntry entry, IPetServices petServices)
    {
        PetServices = petServices;
        Entry       = entry;

        Address     = (nint)pet;
        Owner       = owner;
        SkeletonId  = new PetSkeleton((uint)pet->Character.ModelContainer.ModelCharaId, SkeletonType.Minion);
        Index       = pet->Character.GameObject.ObjectIndex;
        Name        = pet->Character.GameObject.NameString;
        ObjectId    = pet->GetGameObjectId();
        CustomName  = entry.GetName(SkeletonId);
        PetData     = petServices.PetSheets.GetPet(SkeletonId);
    }

    public void Recalculate()
    {
        CustomName = Entry.GetName(SkeletonId);
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

    public bool IsActive
        => (Owner?.IsActive ?? false);

    public void Dispose() { }
}
