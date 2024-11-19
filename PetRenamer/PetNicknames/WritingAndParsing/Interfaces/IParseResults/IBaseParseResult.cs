using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IBaseParseResult : IDataParseResult
{
    string UserName { get; }
    ushort Homeworld { get; }

    int[] IDs { get; }
    string[] Names { get; }
    Vector3?[] EdgeColous { get; }
    Vector3?[] TextColours { get; }
}
