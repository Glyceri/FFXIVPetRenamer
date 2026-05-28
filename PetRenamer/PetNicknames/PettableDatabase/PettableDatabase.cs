using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using PN.S;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDatabase : IPettableDatabase
{
    protected List<IPettableDatabaseEntry> Entries = [];

    protected readonly IPetServices PetServices;

    public PettableDatabase(IPetServices petServices)
    {
        PetServices = petServices;

        Setup();
    }

    public IPettableDatabaseEntry[] DatabaseEntries 
        => [.. Entries];

    protected virtual void Setup()
    {
        List<IPettableDatabaseEntry> newEntries = [];

        SerializableUserV6[]? users = PetServices.Configuration.SerializableUsersV6;

        if (users == null)
        {
            return;
        }

        foreach (SerializableUserV6 user in users)
        {
            SerializableNameDataV3[] datas = user.SerializableNameDatas;

            if (datas.Length == 0)
            {
                continue;
            }

            newEntries.Add(new PettableDataBaseEntry(PetServices, user.ContentID, user.Name, user.Homeworld, PetSkeletonHelper.AsPetSkeletons(datas[0].Ids, datas[0].SkeletonTypes), datas[0].Names, datas[0].EdgeColours, datas[0].TextColours, PetSkeletonHelper.AsPetSkeletons(user.SoftSkeletonData, user.SoftSkeletonTypes), true));
        }
        Entries = newEntries;
    }

    public IPettableDatabaseEntry? GetEntry(string name, ushort homeworld, bool create)
    {
        int entriesCount = Entries.Count;

        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = Entries[i];

            if (entry.Name != name || entry.Homeworld != homeworld)
            {
                continue;
            }

            return entry;
        }

        if (!create)
        {
            return null;
        }

        IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(PetServices, 0, name, homeworld, [], [], [], [], PluginConstants.BaseSkeletons, false);

        Entries.Add(newEntry);

        return newEntry;
    }


    public IPettableDatabaseEntry? GetEntryNoCreate(ulong contentId)
    {
        int entriesCount = Entries.Count;

        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry = Entries[i];

            if (entry.ContentId != contentId)
            {
                continue;
            }

            return Entries[i];
        }

        return null;
    }

    public IPettableDatabaseEntry GetEntry(ulong contentId)
    {
        IPettableDatabaseEntry? entry = GetEntryNoCreate(contentId);

        if (entry != null)
        {
            return entry;
        }

        IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(PetServices, contentId, "[UNKNOWN]", 0, [], [], [], [], PluginConstants.BaseSkeletons, false);

        Entries.Add(newEntry);

        return newEntry;
    }

    public void RemoveEntry(IPettableDatabaseEntry entry, ParseSource parseSource)
    {
        for (int i = Entries.Count - 1; i >= 0; i--)
        {
            IPettableDatabaseEntry currentEntry = Entries[i];

            if (currentEntry.ContentId != entry.ContentId)
            {
                continue;
            }

            currentEntry.Clear(parseSource);

            Entries.RemoveAt(i);
        }
    }

    public SerializableUserV6[] SerializeDatabase()
    {
        List<SerializableUserV6> users = [];

        int entryCount = Entries.Count;

        for (int i = 0; i < entryCount; i++)
        {
            IPettableDatabaseEntry entry = Entries[i];

            if (!entry.IsActive)
            {
                continue;
            }

            if (entry.IsIPC)
            {
                continue;
            }

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
        PetServices.DirtyCaller.DirtyDatabase(this);
    }
}
