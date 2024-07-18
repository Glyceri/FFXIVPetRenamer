using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IDataParserElement
{
    IDataParseResult Parse(string data);
}
