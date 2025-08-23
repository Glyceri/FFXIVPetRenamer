using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using PN.S;
using System.Collections.Immutable;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDatabaseEntry
{
    ulong ContentID { get; }
    string Name { get; }
    ushort Homeworld { get; }
    string HomeworldName { get; }

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
    Vector3? GetEdgeColour(int skeletonID);
    Vector3? GetTextColour(int skeletonID);
    int? GetSoftSkeleton(int softIndex);
    void SetSoftSkeleton(int index, int softSkeleton);
    void SetName(int skeletonID, string name, Vector3? edgeColour, Vector3? textColour);

    void Clear(ParseSource parseSource);
    void UpdateEntry(IModernParseResult parseResult, ParseSource parseSource);
    void UpdateEntryBase(IBaseParseResult parseResult, ParseSource parseSource);

    SerializableUserV5 SerializeEntry();
}
