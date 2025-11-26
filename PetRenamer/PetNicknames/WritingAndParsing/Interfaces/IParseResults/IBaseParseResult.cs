using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IBaseParseResult : IDataParseResult
{
    public string        UserName    { get; }
    public ushort        Homeworld   { get; }

    public PetSkeleton[] IDs         { get; }
    public string[]      Names       { get; }
    public Vector3?[]    EdgeColous  { get; }
    public Vector3?[]    TextColours { get; }
}
