#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.

using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class LegacyPettableDatabase : PettableDatabase, ILegacyDatabase
{
    public LegacyPettableDatabase(IPetServices petServices, IPettableDirtyCaller dirtyCaller) : base(petServices, dirtyCaller) { }

    protected override void Setup()
    {
        SerializableUserV3[]? serializableUsers = PetServices.Configuration.serializableUsersV3;
        _entries.Clear();
        if (serializableUsers == null) return;
        if (serializableUsers.Length == 0) return;

        foreach (SerializableUserV3 userV3 in serializableUsers)
        {
            IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(PetServices, DirtyCaller, 0, userV3.username, userV3.homeworld, userV3.ids, userV3.names, new Vector3?[userV3.ids.Length], new Vector3?[userV3.ids.Length], userV3.softSkeletons, false, true);
            _entries.Add(newEntry);
        }
    }

    public void ApplyParseResult(IBaseParseResult parseResult, ParseSource parseSource)
    {
        IPettableDatabaseEntry? entry = GetEntry(parseResult.UserName, parseResult.Homeworld, true);

        if (entry == null)
        {
            return;
        }

        bool isFromIPC = parseSource == ParseSource.IPC;

        entry.UpdateEntryBase(parseResult, isFromIPC);

        SetDirty();
    }

    public SerializableUserV3[] SerializeLegacyDatabase()
    {
        List<SerializableUserV3> users = new List<SerializableUserV3>();
        IPettableDatabaseEntry[] entries = DatabaseEntries;
        int entriesCount = entries.Length;
        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = entries[i];
            INamesDatabase nameDatabase = entry.ActiveDatabase;
            if (nameDatabase.Length == 0) continue;

            int[] tempSoftSkeletonArray = entry.SoftSkeletons.ToArray();
            users.Add(new SerializableUserV3(nameDatabase.IDs, nameDatabase.Names, entry.Name, entry.Homeworld, tempSoftSkeletonArray, tempSoftSkeletonArray));
        }
        return users.ToArray();
    }
}

#pragma warning restore CS0618