using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettablePet
{
    public bool Touched { get; set; }

    public nint PetPointer { get; }
    public int SkeletonID { get; }
    public ulong ObjectID { get; }
    public uint OldObjectID { get; }
    public byte PetType { get; }
    public ushort Index { get; }
    public string Name { get; }
    public string? CustomName { get; }
    public string? CustomSoftName { get; }
    public bool Dirty { get; }
    public PetSheetData? PetData { get; }

    void Update(nint pointer);
    bool Compare(ref Character character);
    void Destroy();
}
