using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableIslandPet : IIslandPet
{
    public nint           Address     { get; }
    public PetSkeleton    SkeletonId  { get; }
    public GameObjectId   ObjectId    { get; }
    public IPetSheetData? PetData     { get; }
    public IPettableUser? Owner       { get; }
    public BattleChara*   BattleChara { get; }

    public PettableIslandPet(BattleChara* pet, IPettableUser owner, IPetServices petServices)
    {
        BattleChara = pet;
        Address     = (nint)pet;
        Owner       = owner;
        SkeletonId  = new PetSkeleton((uint)pet->Character.ModelContainer.ModelCharaId, SkeletonType.Minion);
        ObjectId    = pet->GetGameObjectId();
        PetData     = petServices.PetSheets.GetPet(SkeletonId);
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

    public bool IsActive
        => (Owner?.IsActive ?? false);

    public void Recalculate() 
        { } // Unused

    public void Dispose() 
        { } // Unused
}
