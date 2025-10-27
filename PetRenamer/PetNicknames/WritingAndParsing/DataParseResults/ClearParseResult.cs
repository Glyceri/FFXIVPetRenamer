using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class ClearParseResult : IClearParseResult
{
    public string Name      { get; }
    public ushort Homeworld { get; }

    public ClearParseResult(string name, ushort homeworld)
    {
        Name      = name;
        Homeworld = homeworld;
    }
}
