using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Structs;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IDataParser
{
    IDataParseResult ParseData(string data);
    bool             ApplyParseData(IDataParseResult result, ParseContext parseContext);
}
