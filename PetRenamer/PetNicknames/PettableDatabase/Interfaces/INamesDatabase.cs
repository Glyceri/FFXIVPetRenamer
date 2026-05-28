using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface INamesDatabase
{
    PetSkeleton[] Ids         { get; }
    string[]      Names       { get; }
    Vector3?[]    EdgeColours { get; }
    Vector3?[]    TextColours { get; }
    int           Length      { get; }
    
    string?  GetName(PetSkeleton id);
    Vector3? GetEdgeColour(PetSkeleton id);
    Vector3? GetTextColour(PetSkeleton id);
    
    void SetName(PetSkeleton id, string? name, Vector3? edgeColour, Vector3? textColour);
    void Update(PetSkeleton[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours);
}
