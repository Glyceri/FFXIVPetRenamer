using PetRenamer.Core.Serialization;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class LegacyPettableDatabase : PettableDatabase, ILegacyDatabase
{
    public LegacyPettableDatabase(IPetServices petServices) 
        : base(petServices) { }

    protected override void Setup()
    {
        SerializableUserV3[]? serializableUsers = PetServices.Configuration.serializableUsersV3;

        Entries.Clear();

        if (serializableUsers == null)
        {
            return;
        }

        if (serializableUsers.Length == 0)
        {
            return;
        }

        foreach (SerializableUserV3 userV3 in serializableUsers)
        {
            IPettableDatabaseEntry newEntry = new PettableDataBaseEntry(PetServices, 0, userV3.username, userV3.homeworld, PetSkeletonHelper.AsPetSkeletons(userV3.ids), userV3.names, new Vector3?[userV3.ids.Length], new Vector3?[userV3.ids.Length], PetSkeletonHelper.AsPetSkeletons(userV3.softSkeletons), false, true);
            
            Entries.Add(newEntry);
        }
    }

    public void ApplyParseResult(IBaseParseResult parseResult, ParseSource parseSource)
    {
        IPettableDatabaseEntry? entry = GetEntry(parseResult.UserName, parseResult.Homeworld, true);

        if (entry == null)
        {
            return;
        }

        entry.UpdateEntryBase(parseResult, parseSource);

        SetDirty();
    }

    public SerializableUserV3[] SerializeLegacyDatabase()
    {
        List<SerializableUserV3> users   = [];
        IPettableDatabaseEntry[] entries = DatabaseEntries;

        int entriesCount = entries.Length;

        for (int i = 0; i < entriesCount; i++)
        {
            IPettableDatabaseEntry entry        = entries[i];
            INamesDatabase         nameDatabase = entry.ActiveDatabase;

            if (nameDatabase.Length == 0)
            {
                continue;
            }

            PetSkeleton[] tempSoftSkeletonArray = entry.SoftSkeletons.ToArray();

            PetSkeletonHelper.AsLegacyArray(tempSoftSkeletonArray, out int[] legacySoftSkeletonArray);
            PetSkeletonHelper.AsLegacyArray(nameDatabase.Ids,      out int[] legacyIdsArray);

            users.Add(new SerializableUserV3(legacyIdsArray, nameDatabase.Names, entry.Name, entry.Homeworld, legacySoftSkeletonArray, legacySoftSkeletonArray));
        }

        return [.. users];
    }
}