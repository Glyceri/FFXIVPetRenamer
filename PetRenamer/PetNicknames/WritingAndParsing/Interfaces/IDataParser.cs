using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IDataParser
{
    IDataParseResult ParseData(string data);
    bool ApplyParseData(IDataParseResult result, bool isFromIPC);
}
