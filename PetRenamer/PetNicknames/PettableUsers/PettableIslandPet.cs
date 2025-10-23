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
    public PetSkeleton    SkeletonID { get; private set; }
    public ulong          ObjectID   { get; private set; }
    public ushort         Index      { get; private set; }
    public string         Name       { get; private set; } = string.Empty;
    public string?        CustomName { get; private set; }
    public IPetSheetData? PetData    { get; private set; }
    public IPettableUser? Owner      { get; private set; }
    public BattleChara*   BattlePet  { get; private set; }
    public Vector3?       EdgeColour { get; private set; }
    public Vector3?       TextColour { get; private set; }

    private readonly IPettableDatabaseEntry Entry;
    private readonly IPetServices           PetServices;

    public PettableIslandPet(BattleChara* pet, IPettableUser owner, IPettableDatabaseEntry entry, IPetServices petServices)
    {
        PetServices = petServices;
        Entry       = entry;

        Address     = (nint)pet;
        Owner       = owner;
        SkeletonID  = new PetSkeleton((uint)pet->Character.ModelContainer.ModelCharaId, SkeletonType.Minion);
        Index       = pet->Character.GameObject.ObjectIndex;
        Name        = pet->Character.GameObject.NameString;
        ObjectID    = pet->GetGameObjectId();
        CustomName  = entry.GetName(SkeletonID);
        PetData     = petServices.PetSheets.GetPet(SkeletonID);
    }

    public void Recalculate()
    {
        CustomName = Entry.GetName(SkeletonID);
        EdgeColour = Entry.GetEdgeColour(SkeletonID);
        TextColour = Entry.GetTextColour(SkeletonID);
    }

    public void GetDrawColours(out Vector3? edgeColour, out Vector3? textColour)
    {
        edgeColour = null;
        textColour = null;

        int colourSetting = PetServices.Configuration.showColours;

        if (colourSetting >= 2)
        {
            return;
        }

        if (colourSetting == 1 && (Owner?.IsLocalPlayer ?? false))
        {
            return;
        }

        edgeColour = EdgeColour;
        textColour = TextColour;
    }

    public void Dispose() { }
}
