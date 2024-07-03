using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettablePet
{
    public bool Touched { get; set; }

    public nint PetPointer { get; }
    public int SkeletonID { get; }
    public uint ObjectID { get; }
    public ushort Index { get; }
    public string Name { get; }

    void Update(nint pointer);
    bool Compare(ref Character character);
    void Destroy();
}
