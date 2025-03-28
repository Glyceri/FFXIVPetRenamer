﻿using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableIslandPet : IIslandPet
{
    public nint PetPointer { get; private set; }
    public int SkeletonID { get; init; }
    public ulong ObjectID { get; init; }
    public ushort Index { get; init; }
    public string Name { get; init; } = "";
    public string? CustomName { get; private set; }
    public IPetSheetData? PetData { get; private set; }
    public IPettableUser? Owner { get; private set; }
    public BattleChara* BattlePet { get; }
    public Vector3? EdgeColour { get; private set; }
    public Vector3? TextColour { get; private set; }

    readonly IPettableDatabaseEntry Entry;
    readonly IPetServices PetServices;

    public PettableIslandPet(BattleChara* pet, IPettableUser owner, IPettableDatabaseEntry entry, IPetServices petServices)
    {
        PetServices = petServices;
        Entry = entry;

        PetPointer = (nint)pet;

        Owner = owner;
        SkeletonID = pet->Character.ModelContainer.ModelCharaId;
        Index = pet->Character.GameObject.ObjectIndex;
        Name = pet->Character.GameObject.NameString;
        ObjectID = pet->GetGameObjectId();
        CustomName = entry.GetName(SkeletonID);
        PetData = petServices.PetSheets.GetPet(SkeletonID);
    }

    public void Recalculate()
    {
        CustomName = Entry.GetName(SkeletonID);
        EdgeColour = Entry.GetEdgeColour(SkeletonID);
        TextColour = Entry.GetTextColour(SkeletonID);
    }

    public void Dispose() { }

    public void GetDrawColours(out Vector3? edgeColour, out Vector3? textColour)
    {
        edgeColour = null;
        textColour = null;

        int colourSetting = PetServices.Configuration.showColours;

        if (colourSetting >= 2) return;
        if (colourSetting == 1 && (Owner?.IsLocalPlayer ?? false)) return;

        edgeColour = EdgeColour;
        textColour = TextColour;
    }
}
