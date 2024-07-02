namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDatabase
{
    bool ContainsLegacy { get; }
    IPettableDatabaseEntry[] DatabaseEntries { get; }

    /// <summary>
    /// Get's the database entry if it exists. Returns null if the entry does NOT exist.
    /// </summary>
    /// <param name="name">Player Name</param>
    /// <returns>The Data Base Entry</returns>
    IPettableDatabaseEntry? GetEntry(string name);
    /// <summary>
    /// Get's the database entry if it exists. In the case it doesn't it creates a new one!
    /// </summary>
    /// <param name="name">Player Content ID</param>
    /// <returns>The Data Base Entry</returns>
    IPettableDatabaseEntry GetEntry(ulong contentID);

    void CheckLegacyStatus();
}
