using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class ClearParseResult : IClearParseResult
{
    public ulong ContentID { get; }

    public ClearParseResult(ulong contentID)
    {
        ContentID = contentID;
    }
}
