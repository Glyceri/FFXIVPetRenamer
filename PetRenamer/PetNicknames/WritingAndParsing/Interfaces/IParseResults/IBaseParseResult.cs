using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IBaseParseResult : IDataParseResult
{
    string        UserName    { get; }
    ushort        Homeworld   { get; }

    PetSkeleton[] IDs         { get; }
    string[]      Names       { get; }
    Vector3?[]    EdgeColous  { get; }
    Vector3?[]    TextColours { get; }
}
