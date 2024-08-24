using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettablePet
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
    public ulong Lifetime { get; }

    void Update(nint pointer);
    void Recalculate();
}
