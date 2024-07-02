using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDatabase : IPettableDatabase
{
    List<IPettableDatabaseEntry> _entries = new List<IPettableDatabaseEntry>();

    public bool ContainsLegacy { get; private set; } = false;
    public IPettableDatabaseEntry[] DatabaseEntries { get => _entries.ToArray(); }

    IPetLog petLog;

    public PettableDatabase(Configuration configuration, IPetLog PetLog)
    {
        petLog = PetLog;
        SerializableUserV3[]? serializableUsers = configuration.serializableUsersV3;
        if (serializableUsers == null)      return;
        if (serializableUsers.Length == 0)  return; 
        ContainsLegacy = true;
        foreach(SerializableUserV3 userV3 in serializableUsers)
        {
            IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(ulong.MaxValue, userV3.username, userV3.ids, userV3.names, true);
            _entries.Add(newEntry);
        }
    }

    public IPettableDatabaseEntry? GetEntry(string name)
    {
        int entriesCount = _entries.Count;
        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = _entries[i];
            petLog.Log(entry.Name + " : " + name);
            if (entry.Name != name) continue;
            return entry;
        }

        IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(ulong.MaxValue, name, [], [], true);
        ContainsLegacy = true;
        _entries.Add(newEntry);
        return newEntry;
    }

    public IPettableDatabaseEntry GetEntry(ulong contentID)
    {
        int entriesCount = _entries.Count;
        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = _entries[i];
            if (entry.ContentID != contentID) continue;
            return entry;
        }

        IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(contentID, "[UNKOWN]", [], [], false);
        _entries.Add(newEntry);
        return newEntry;
    }

    public void CheckLegacyStatus()
    {
        bool hasLegacy = false;
        foreach(IPettableDatabaseEntry entry in _entries)
        {
            if (!entry.IsLegacy) continue;
            hasLegacy = true;
            break;
        }
        ContainsLegacy = hasLegacy;
    }
}
