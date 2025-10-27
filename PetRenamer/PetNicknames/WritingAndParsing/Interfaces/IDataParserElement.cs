using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IDataParserElement
{
    public IDataParseResult Parse(string data);
}
