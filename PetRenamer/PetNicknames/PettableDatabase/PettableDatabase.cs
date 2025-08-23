using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PN.S;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDatabase : IPettableDatabase
{
    protected List<IPettableDatabaseEntry> _entries = new List<IPettableDatabaseEntry>();

    public IPettableDatabaseEntry[] DatabaseEntries { get => [.. _entries]; }

    protected readonly IPetServices PetServices;
    protected readonly IPettableDirtyCaller DirtyCaller;

    public PettableDatabase(IPetServices petServices, IPettableDirtyCaller dirtyCaller)
    {
        PetServices = petServices;
        DirtyCaller = dirtyCaller;

        Setup();
    }

    protected virtual void Setup()
    {
        List<IPettableDatabaseEntry> newEntries = new List<IPettableDatabaseEntry>();
        SerializableUserV5[]? users = PetServices.Configuration.SerializableUsersV5;
        if (users == null) return;
        foreach (SerializableUserV5 user in users)
        {
            SerializableNameDataV2[] datas = user.SerializableNameDatas;
            if (datas.Length == 0) continue;

            newEntries.Add(new PettableDataBaseEntry(PetServices, DirtyCaller, user.ContentID, user.Name, user.Homeworld, datas[0].IDS, datas[0].Names, datas[0].EdgeColours, datas[0].TextColours, user.SoftSkeletonData, true));
        }
        _entries = newEntries;
    }

    public IPettableDatabaseEntry? GetEntry(string name, ushort homeworld, bool create)
    {
        int entriesCount = _entries.Count;
        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = _entries[i];
            if (entry.Name != name || entry.Homeworld != homeworld) continue;
            return entry;
        }

        if (!create)
        {
            return null;
        }

        IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(PetServices, DirtyCaller, 0, name, homeworld, [], [], [], [], PluginConstants.BaseSkeletons, false);
        _entries.Add(newEntry);
        return newEntry;
    }


    public IPettableDatabaseEntry? GetEntryNoCreate(ulong contentID)
    {
        int entriesCount = _entries.Count;

        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = _entries[i];
            if (entry.ContentID != contentID) continue;
            return _entries[i];
        }

        return null;
    }

    public IPettableDatabaseEntry GetEntry(ulong contentID)
    {
        IPettableDatabaseEntry? entry = GetEntryNoCreate(contentID);
        if (entry != null) return entry;

        IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(PetServices, DirtyCaller, contentID, "[UNKNOWN]", 0, [], [], [], [], PluginConstants.BaseSkeletons, false);
        _entries.Add(newEntry);
        return newEntry;
    }

    public void RemoveEntry(IPettableDatabaseEntry entry)
    {
        for (int i = _entries.Count - 1; i >= 0; i--)
        {
            IPettableDatabaseEntry currentEntry = _entries[i];

            if (currentEntry.ContentID != entry.ContentID)
            {
                continue;
            }

            currentEntry.Clear(ParseSource.None);

            _entries.RemoveAt(i);
        }
    }

    public SerializableUserV5[] SerializeDatabase()
    {
        List<SerializableUserV5> users = new List<SerializableUserV5>();
        int entryCount = _entries.Count;
        for (int i = 0; i < entryCount; i++)
        {
            IPettableDatabaseEntry entry = _entries[i];
            if (!entry.IsActive) continue;
            if (entry.IsIPC) continue;
            users.Add(entry.SerializeEntry());
        }
        return users.ToArray();
    }

    public void ApplyParseResult(IModernParseResult parseResult, ParseSource parseSource)
    {
        bool isFromIPC = parseSource == ParseSource.IPC;

        IPettableDatabaseEntry entry = GetEntry(parseResult.ContentID);

        entry.UpdateEntry(parseResult, parseSource);

        if (isFromIPC)
        {
            return;
        }

        SetDirty();
    }

    public void SetDirty()
    {
        DirtyCaller.DirtyDatabase(this);
    }
}
