using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettablePet : IPettableEntity, IDisposable
{
    public IPettableUser? Owner      { get; }
    public int            SkeletonID { get; }
    public ulong          ObjectID   { get; }
    public ushort         Index      { get; }
    public string         Name       { get; }
    public string?        CustomName { get; }
    public Vector3?       EdgeColour { get; }
    public Vector3?       TextColour { get; }
    public IPetSheetData? PetData    { get; }

    public void Recalculate();

    public void GetDrawColours(out Vector3? edgeColour, out Vector3? textColour);
}
