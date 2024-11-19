using PN.S;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface INamesDatabase
{
    public int[] IDs { get; }
    public string[] Names { get; }
    public Vector3?[] EdgeColours { get; }
    public Vector3?[] TextColours { get; }
    public int Length { get; }
    string? GetName(int ID);
    Vector3? GetEdgeColour(int ID);
    Vector3? GetTextColour(int ID);
    void SetName(int ID, string? name, Vector3? edgeColour, Vector3? textColour);

    void Update(int[] IDs, string[] names, Vector3?[] edgeColours, Vector3?[] textColours, IPettableDirtyCaller dirtyCaller);

    SerializableNameDataV2 SerializeData();
}
