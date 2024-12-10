using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface ILegacyDatabase : IPettableDatabase
{
#pragma warning disable CS0618 // Type or member is obsolete (This is supposed to handle obsolete objects)
    SerializableUserV3[] SerializeLegacyDatabase();
#pragma warning restore CS0618 // Type or member is obsolete
    void ApplyParseResult(IBaseParseResult parseResult, bool isFromIPC);
}
