using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class Version1ParseResult : IBaseParseResult
{
    public string UserName { get; init; }
    public ushort Homeworld { get; init; }

    public int[] IDs { get; init; } = Array.Empty<int>();
    public string[] Names { get; init; } = Array.Empty<string>();

    public Version1ParseResult(string userName, ushort homeworld)
    {
        UserName = userName;
        Homeworld = homeworld;
    }

    public Version1ParseResult(string userName, ushort homeworld, int[] ids, string[] names) : this(userName, homeworld)
    {
        IDs = ids;
        Names = names;
    }
}
