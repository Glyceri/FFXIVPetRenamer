using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettablePet : IDisposable
{
    IPettableUser? Owner { get; }

    public bool Marked { get; set; }

    public nint PetPointer { get; }
    public int SkeletonID { get; }
    public ulong ObjectID { get; }
    public ushort Index { get; }
    public string Name { get; }
    public string? CustomName { get; }
    public IPetSheetData? PetData { get; }

    void Recalculate();
}
