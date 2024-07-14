using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettablePet
{
    IPettableUser? Owner { get; }

    public bool Touched { get; set; }

    public nint PetPointer { get; }
    public int SkeletonID { get; }
    public ulong ObjectID { get; }
    public uint OldObjectID { get; }
    public byte PetType { get; }
    public ushort Index { get; }
    public string Name { get; }
    public string? CustomName { get; }
    public bool Dirty { get; }
    public IPetSheetData? PetData { get; }
    public ulong Lifetime { get; }

    void Recalculate();
    void Update(nint pointer);
    bool Compare(ref Character character);
}
