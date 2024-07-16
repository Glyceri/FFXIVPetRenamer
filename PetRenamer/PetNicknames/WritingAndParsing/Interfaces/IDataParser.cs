using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.Parsing.Interfaces;

internal interface IDataParser
{
    IDataParseResult ParseData(string data);
    void ApplyParseData(ulong player, IDataParseResult result, bool isFromIPC);
}
