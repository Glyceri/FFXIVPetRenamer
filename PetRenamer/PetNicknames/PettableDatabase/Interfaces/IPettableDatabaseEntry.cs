using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDatabaseEntry
{
    ulong ContentID { get; }
    string Name { get; }
    ushort Homeworld { get; }
    string HomeworldName { get; }

    bool Dirty { get; }
    bool IsActive { get; }
    bool Destroyed { get; }

    int[] SoftSkeletons { get; }

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
    string? GetSoftName(int softIndex);
    int? GetSoftSkeleton(int softIndex);
    void SetName(int skeletonID, string name);
    void NotifySeenDirty();
    void Destroy();
}
