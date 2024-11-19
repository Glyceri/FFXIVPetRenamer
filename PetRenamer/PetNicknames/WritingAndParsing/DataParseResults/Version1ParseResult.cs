using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class Version1ParseResult : IBaseParseResult
{
    public string UserName { get; init; }
    public ushort Homeworld { get; init; }

    public int[] IDs { get; init; } = Array.Empty<int>();
    public string[] Names { get; init; } = Array.Empty<string>();
    public Vector3?[] EdgeColous { get; init; } = Array.Empty<Vector3?>();
    public Vector3?[] TextColours { get; init; } = Array.Empty<Vector3?>();

    public Version1ParseResult(string userName, ushort homeworld)
    {
        UserName = userName;
        Homeworld = homeworld;
    }

    public Version1ParseResult(string userName, ushort homeworld, int[] ids, string[] names) : this(userName, homeworld)
    {
        IDs = ids;
        Names = names;
        EdgeColous = new Vector3?[ids.Length];
        TextColours = new Vector3?[ids.Length];
    }
}
