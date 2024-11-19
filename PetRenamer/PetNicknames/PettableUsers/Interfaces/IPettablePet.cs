using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettablePet : IDisposable
{
    IPettableUser? Owner { get; }

    nint PetPointer { get; }
    int SkeletonID { get; }
    ulong ObjectID { get; }
    ushort Index { get; }
    string Name { get; }
    string? CustomName { get; }
    Vector3? EdgeColour { get; }
    Vector3? TextColour { get; }
    IPetSheetData? PetData { get; }

    void Recalculate();

    void GetDrawColours(out Vector3? edgeColour, out Vector3? textColour);
}
