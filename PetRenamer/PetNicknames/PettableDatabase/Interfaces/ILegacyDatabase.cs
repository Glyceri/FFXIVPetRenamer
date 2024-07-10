using PetRenamer.Core.Serialization;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface ILegacyDatabase : IPettableDatabase
{
    SerializableUserV3[] SerializeLegacyDatabase();
}
