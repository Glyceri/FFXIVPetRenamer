#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.

using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class LegacyPettableDatabase : PettableDatabase, ILegacyDatabase
{
    public LegacyPettableDatabase(in IPetServices PetServices) : base(in PetServices)
    {
        SerializableUserV3[]? serializableUsers = PetServices.Configuration.serializableUsersV3;
        _entries.Clear();
        if (serializableUsers == null) return;
        if (serializableUsers.Length == 0) return;

        foreach (SerializableUserV3 userV3 in serializableUsers)
        {
            PetServices.PetLog.Log(userV3.username);
            IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(in PetServices, ulong.MaxValue, userV3.username, userV3.homeworld, userV3.ids, userV3.names, userV3.softSkeletons, false);
            _entries.Add(newEntry);
        }
    }

    public SerializableUserV3[] SerializeLegacyDatabase()
    {
        List<SerializableUserV3> users = new List<SerializableUserV3>();
        IPettableDatabaseEntry[] entries = DatabaseEntries;
        int entriesCount = entries.Length;
        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = entries[i];
            // VV funny, but for oldschool users they are meant to NOT be active at ANY time.
            // if (!entry.IsActive) continue;
            INamesDatabase nameDatabase = entry.ActiveDatabase;
            users.Add(new SerializableUserV3(nameDatabase.IDs, nameDatabase.Names, entry.Name, entry.Homeworld, entry.SoftSkeletons, entry.SoftSkeletons));
        }
        return users.ToArray();
    }
}

#pragma warning restore CS0618