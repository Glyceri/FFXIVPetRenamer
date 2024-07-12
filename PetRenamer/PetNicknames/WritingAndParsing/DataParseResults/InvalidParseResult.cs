using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class InvalidParseResult : IDataParseResult
{
    public readonly string Reason;

    public InvalidParseResult(string reason) 
    {
        Reason = reason;
    }
}
