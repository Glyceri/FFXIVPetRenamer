using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PN.S;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Immutable;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDatabaseEntry
{
    ulong ContentID { get; }
    string Name { get; }
    ushort Homeworld { get; }
    string HomeworldName { get; }
    string AddedOn { get; }
    string Version { get; }

    bool IsActive { get; }
    bool IsIPC { get; }
    bool IsLegacy { get; }

    ImmutableArray<int> SoftSkeletons { get; }

    INamesDatabase ActiveDatabase { get; }
    INamesDatabase[] AllDatabases { get; }

    void UpdateContentID(ulong contentID, bool removeIPCStatus = false);
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
    void Clear();

    void UpdateEntry(IModernParseResult parseResult, bool asIPC);
    void UpdateEntryBase(IBaseParseResult parseResult, bool asIPC);

    SerializableUserV4 SerializeEntry();
}
