using Dalamud.Game.ClientState.Objects.SubKinds;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

namespace PetRenamer.PetNicknames.Parsing.Interfaces;

internal interface IDataParser
{
    IDataParseResult ParseData(string data);
    void ApplyParseData(IPlayerCharacter player, IDataParseResult result, bool isFromIPC);
}
