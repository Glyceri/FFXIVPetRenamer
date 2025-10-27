namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IClearParseResult : IDataParseResult
{
    public ushort Homeworld { get; }
    public string Name      { get; }
}
