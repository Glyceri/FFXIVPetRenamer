using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PN.S;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface INamesDatabase
{
    public PetSkeleton[] IDs { get; }
    public string[] Names { get; }
    public Vector3?[] EdgeColours { get; }
    public Vector3?[] TextColours { get; }
    public int Length { get; }
    string? GetName(PetSkeleton ID);
    Vector3? GetEdgeColour(PetSkeleton ID);
    Vector3? GetTextColour(PetSkeleton ID);
    void SetName(PetSkeleton ID, string? name, Vector3? edgeColour, Vector3? textColour);

    void Update(PetSkeleton[] IDs, string[] names, Vector3?[] edgeColours, Vector3?[] textColours, IPettableDirtyCaller dirtyCaller);

    SerializableNameDataV3 SerializeData();
}
