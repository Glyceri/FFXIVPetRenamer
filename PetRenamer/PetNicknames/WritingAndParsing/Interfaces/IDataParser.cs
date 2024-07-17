using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.Parsing.Interfaces;

internal interface IDataParser
{
    IDataParseResult ParseData(string data);
    bool ApplyParseData(IDataParseResult result, bool isFromIPC);
}
