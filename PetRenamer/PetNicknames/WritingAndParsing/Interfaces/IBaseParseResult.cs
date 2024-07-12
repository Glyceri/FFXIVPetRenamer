
namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IBaseParseResult : IDataParseResult
{
    string UserName { get; }
    ushort Homeworld { get; }

    int[] IDs { get; }
    string[] Names { get; }
}
