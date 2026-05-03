using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettablePet : IPettableEntity, IDisposable
{
    IPettableUser? Owner      { get; }
    PetSkeleton    SkeletonID { get; }
    ulong          ObjectID   { get; }
    ushort         Index      { get; }
    string         Name       { get; }
    string?        CustomName { get; }
    IPetSheetData? PetData    { get; }
    bool           IsActive   { get; }
    
    void Recalculate();
    void GetDrawColours(Configuration.ColourConfig colourConfig, out Vector3? edgeColour, out Vector3? textColour);
}
