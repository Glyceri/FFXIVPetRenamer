using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PN.S;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;
using System;
using Dalamud.Utility;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDatabase : IPettableDatabase
{
    protected List<IPettableDatabaseEntry> _entries = new List<IPettableDatabaseEntry>();

    public bool ContainsLegacy { get; private set; } = false;
    public IPettableDatabaseEntry[] DatabaseEntries { get => _entries.ToArray(); }

    readonly IPetServices PetServices;
    readonly IPettableDirtyCaller DirtyCaller;

    public PettableDatabase(in IPetServices petServices, in IPettableDirtyCaller dirtyCaller, object? _)
    {
        PetServices = petServices;
        DirtyCaller = dirtyCaller;
    }

    public PettableDatabase(in IPetServices petServices, in IPettableDirtyCaller dirtyCaller)
    {
        PetServices = petServices;
        DirtyCaller = dirtyCaller;

        List<IPettableDatabaseEntry> newEntries = new List<IPettableDatabaseEntry>();
        SerializableUserV4[]? users = petServices.Configuration.SerializableUsersV4;
        if (users == null) return;
        foreach(SerializableUserV4 user in users)
        {
            SerializableNameData[] datas = user.SerializableNameDatas;
            if (datas.Length == 0) continue;

            string addedOn = user.AddedOn;
            if (addedOn.IsNullOrWhitespace())
            {
                addedOn = "DateTime.Unkown";
            }

            string version = user.Version;
            if (version.IsNullOrWhitespace())
            {
                version = "Version.Unkown";
            }

            newEntries.Add(new PettableDataBaseEntry(in PetServices, in DirtyCaller, user.ContentID, user.Name, user.Homeworld, datas[0].IDS, datas[0].Names, user.SoftSkeletonData, addedOn, version, true));
        }
        _entries = newEntries;
    }

    public IPettableDatabaseEntry GetEntry(string name, ushort homeworld)
    {
        int entriesCount = _entries.Count;
        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = _entries[i];
            if (entry.Name != name || entry.Homeworld != homeworld) continue;
            return entry;
        }

        IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(in PetServices, in DirtyCaller, 0, name, homeworld, [], [], PluginConstants.BaseSkeletons, DateTime.Now.ToString("yyyyMMdd"), PetRenamerPlugin.PuginVersion.ToString(), false);
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
            return _entries[i];
        }

        IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(in PetServices, in DirtyCaller, contentID, "[UNKOWN]", 0, [], [], PluginConstants.BaseSkeletons, DateTime.Now.ToString("yyyyMMdd"), PetRenamerPlugin.PuginVersion.ToString(), false);
        _entries.Add(newEntry);
        return newEntry;
    }

    public bool RemoveEntry(IPettableDatabaseEntry entry)
    {
        bool hasRemoved = false;
        for (int i = _entries.Count - 1; i >= 0; i--)
        {
            if (_entries[i].ContentID != entry.ContentID) continue;

            _entries.RemoveAt(i);
            SetDirty();
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
            if (entry.IsIPC) continue;
            users.Add(entry.SerializeEntry());
        }
        return users.ToArray();
    }

    public void ApplyParseResult(IModernParseResult parseResult, bool isFromIPC)
    {
        IPettableDatabaseEntry entry = GetEntry(parseResult.ContentID);
        entry.UpdateEntry(parseResult, isFromIPC);
        if (!isFromIPC) SetDirty();
    }

    protected void SetDirty()
    {
        DirtyCaller.DirtyDatabase(this);
    }
}
