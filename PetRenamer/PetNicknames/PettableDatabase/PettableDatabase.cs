using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDatabase(in IPetServices petSerivces) : IPettableDatabase
{
    protected List<IPettableDatabaseEntry> _entries = new List<IPettableDatabaseEntry>();

    public bool ContainsLegacy { get; private set; } = false;
    public IPettableDatabaseEntry[] DatabaseEntries { get => _entries.ToArray(); }

    readonly IPetServices PetServices = petSerivces;

    public IPettableDatabaseEntry? GetEntry(string name)
    {
        int entriesCount = _entries.Count;
        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = _entries[i];
            if (entry.Name != name) continue;
            return entry;
        }

        return null;
    }

    public IPettableDatabaseEntry GetEntry(ulong contentID)
    {
        int entriesCount = _entries.Count;
        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = _entries[i];
            if (entry.ContentID != contentID) continue;
            return _entries[i];
        }

        IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(in PetServices, contentID, "[UNKOWN]", 0, [], [], PluginConstants.BaseSkeletons, false);
        _entries.Add(newEntry);
        return newEntry;
    }

    public bool RemoveEntry(ulong contentID)
    {
        bool hasRemoved = false;
        for (int i = _entries.Count - 1; i >= 0; i--)
        {
            if (_entries[i].ContentID != contentID) continue;
            _entries.RemoveAt(i);
            hasRemoved = true;
        }
        return hasRemoved;
    }

    public bool RemoveEntry(IPettableDatabaseEntry entry)
    {
        bool hasRemoved = false;
        for (int i = _entries.Count - 1; i >= 0; i--)
        {
            if (_entries[i].ContentID != entry.ContentID) continue;
            _entries.RemoveAt(i);
            hasRemoved = true;
        }
        return hasRemoved;
    }
}
