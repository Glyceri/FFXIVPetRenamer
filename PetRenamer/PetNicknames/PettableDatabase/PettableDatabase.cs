﻿using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.Services.Interface;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDatabase : IPettableDatabase
{
    protected List<IPettableDatabaseEntry> _entries = new List<IPettableDatabaseEntry>();

    public bool ContainsLegacy { get; private set; } = false;
    public IPettableDatabaseEntry[] DatabaseEntries { get => _entries.ToArray(); }

    readonly IPetServices PetServices;

    public PettableDatabase(in IPetServices petServices)
    {
        PetServices = petServices;

        List<IPettableDatabaseEntry> newEntries = new List<IPettableDatabaseEntry>();
        SerializableUserV4[]? users = petServices.Configuration.SerializableUsersV4;
        if (users == null) return;
        foreach(SerializableUserV4 user in users)
        {
            SerializableNameData[] datas = user.SerializableNameDatas;
            if (datas.Length == 0) continue;
            newEntries.Add(new PettableDataBaseEntry(in PetServices, user.ContentID, user.Name, user.Homeworld, datas[0].IDS, datas[0].Names, user.SoftSkeletonData, true));
        }
        _entries = newEntries;
    }

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

    public SerializableUserV4[] SerializeDatabase()
    {
        List<SerializableUserV4> users = new List<SerializableUserV4>();
        int entryCount = _entries.Count;
        for (int i = 0; i < entryCount; i++)
        {
            IPettableDatabaseEntry entry = _entries[i];
            if (!entry.IsActive) continue;
            users.Add(entry.SerializeEntry());
        }
        return users.ToArray();
    }
}
