#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.

using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class LegacyPettableDatabase : PettableDatabase
{
    public LegacyPettableDatabase(Configuration configuration, IPetLog PetLog) : base(PetLog)
    {
        SerializableUserV3[]? serializableUsers = configuration.serializableUsersV3;
        if (serializableUsers == null) return;
        if (serializableUsers.Length == 0) return;
        foreach (SerializableUserV3 userV3 in serializableUsers)
        {
            IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(ulong.MaxValue, userV3.username, userV3.homeworld, userV3.ids, userV3.names, userV3.softSkeletons, false);
            _entries.Add(newEntry);
        }
    }
}

#pragma warning restore CS0618