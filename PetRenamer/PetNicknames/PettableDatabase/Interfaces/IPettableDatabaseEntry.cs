﻿using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using System.Collections.Immutable;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDatabaseEntry
{
    ulong ContentID { get; }
    string Name { get; }
    ushort Homeworld { get; }
    string HomeworldName { get; }

    bool IsActive { get; }
    bool Destroyed { get; }
    bool IsIPC { get; }

    ImmutableArray<int> SoftSkeletons { get; }

    bool IsDirty { get; }
    bool IsDirtyForUI { get; }

    INamesDatabase ActiveDatabase { get; }
    INamesDatabase[] AllDatabases { get; }

    int Length();
    void UpdateContentID(ulong contentID);
    void UpdateEntry(IPettableUser pettableUser);
    /// <summary>
    /// Moves this entry into the new database.
    /// </summary>
    /// <param name="database">The database to move this entry into.</param>
    /// <returns>If the move succeeded.</returns>
    bool MoveToDataBase(IPettableDatabase database);
    string? GetName(int skeletonID);
    int? GetSoftSkeleton(int softIndex);
    void SetSoftSkeleton(int index, int softSkeleton);
    void SetName(int skeletonID, string name);
    void NotifySeenDirty();
    void MarkDirtyUIAsNotified();
    void Destroy();

    void UpdateEntry(IModernParseResult parseResult, bool asIPC);
    void UpdateEntryBase(IBaseParseResult parseResult, bool asIPC)

    SerializableUserV4 SerializeEntry();
}
