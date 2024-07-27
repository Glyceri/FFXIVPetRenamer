namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IClearParseResult : IDataParseResult
{
    ushort Homeworld { get; }
    string Name { get; }
}
