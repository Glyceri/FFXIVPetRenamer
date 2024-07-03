namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDatabaseEntry
{
    ulong ContentID { get; }
    string Name { get; }
    int[] IDs { get; }
    string[] Names { get; }

    bool IsLegacy { get; }

    int Length();
    void RemoveLegacyStatusWith(ulong ContentID);
}
