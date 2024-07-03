using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDatabaseEntry
{
    ulong ContentID { get; }
    string Name { get; }
    ushort Homeworld { get; }

    bool Dirty { get; }
    bool IsActive { get; }

    public INamesDatabase ActiveDatabase { get; }
    public INamesDatabase[] AllDatabases { get; }

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
    void SetName(int skeletonID, string name);
}
