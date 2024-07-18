namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IClearParseResult : IDataParseResult
{
    ulong ContentID { get; }
}
