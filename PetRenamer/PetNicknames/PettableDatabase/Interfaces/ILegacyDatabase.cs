using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface ILegacyDatabase : IPettableDatabase
{
    SerializableUserV3[] SerializeLegacyDatabase();
    void ApplyParseResult(IBaseParseResult parseResult, bool isFromIPC);
}
