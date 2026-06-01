using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IDataChecker
{
    bool CheckModernData(ulong contentId, IModernParseResult modernParseResult);
    bool CheckData(IBaseParseResult baseParseResult);
}