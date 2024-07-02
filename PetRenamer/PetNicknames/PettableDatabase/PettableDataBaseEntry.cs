using PetRenamer.PetNicknames.PettableDatabase.Interfaces;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDataBaseEntry : IPettableDatabaseEntry
{
    public ulong ContentID { get; private set; }
    public string Name { get; } = "";

    public int[] IDs { get; }
    public string[] Names { get; }

    public bool IsLegacy { get; private set; }

    public PettableDataBaseEntry(ulong contentID, string name, int[] ids, string[] names, bool isLegacy = false)
    {
        ContentID = contentID;
        Name = name;
        IDs = ids;
        Names = names;
        IsLegacy = isLegacy;
    }

    public void RemoveLegacyStatusWith(ulong ContentID)
    {
        if (ContentID == 0 || ContentID == ulong.MaxValue) return;
        this.ContentID = ContentID;
        IsLegacy = false;
    }
}
