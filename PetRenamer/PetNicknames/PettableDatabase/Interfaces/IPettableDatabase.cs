﻿using PN.S;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDatabase
{
    IPettableDatabaseEntry[] DatabaseEntries { get; }

    /// <summary>
    /// Get's the database entry if it exists. Returns null if the entry does NOT exist.
    /// </summary>
    /// <param name="name">Player Name</param>
    /// <returns>The Data Base Entry</returns>
    IPettableDatabaseEntry? GetEntry(string name, ushort homeworld, bool create);
    /// <summary>
    /// Get's the database entry if it exists.
    /// </summary>
    /// <param name="name">Player Content ID</param>
    /// <returns>The Data Base Entry</returns>
    IPettableDatabaseEntry? GetEntryNoCreate(ulong contentID);
    /// <summary>
    /// Get's the database entry if it exists. In the case it doesn't it creates a new one!
    /// </summary>
    /// <param name="name">Player Content ID</param>
    /// <returns>The Data Base Entry</returns>
    IPettableDatabaseEntry GetEntry(ulong contentID);
    /// <summary>
    /// Removes the entry from the database.
    /// </summary>
    /// <param name="entry">Entry to remove.</param>
    /// <returns>Whether the remove succeeded.</returns>
    void RemoveEntry(IPettableDatabaseEntry entry);

    SerializableUserV5[] SerializeDatabase();

    void SetDirty();
    void ApplyParseResult(IModernParseResult parseResult, bool isFromIPC);
}
