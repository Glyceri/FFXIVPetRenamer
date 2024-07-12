namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IDataParserElement
{
    IDataParseResult Parse(string data);
}
