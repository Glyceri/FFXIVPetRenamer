using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IDataParser
{
    public IDataParseResult ParseData(string data);
    public bool ApplyParseData(IDataParseResult result, ParseSource parseSource);
}
