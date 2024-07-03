using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDataBaseEntry : IPettableDatabaseEntry
{
    public ulong ContentID { get; private set; }
    public string Name { get; private set; } = "";

    public bool IsActive { get; private set; }
    public ushort Homeworld { get; private set; }

    public INamesDatabase ActiveDatabase { get; private set; }
    public INamesDatabase[] AllDatabases { get => [ActiveDatabase]; }
    public bool Dirty { get => ActiveDatabase.IsDirty; }

    public PettableDataBaseEntry(ulong contentID, string name, ushort homeworld, int[] ids, string[] names, bool isActive)
    {
        ContentID = contentID;
        Name = name;
        ActiveDatabase = new PettableNameDatabase(ids, names);
        IsActive = isActive;
        Homeworld = homeworld;
    }

    public void UpdateEntry(IPettableUser pettableUser)
    {
        Homeworld = pettableUser.Homeworld;
        Name = pettableUser.Name;
    }

    public int Length() => ActiveDatabase.IDs.Length;

    public bool MoveToDataBase(IPettableDatabase database)
    {
        IPettableDatabaseEntry entry = database.GetEntry(ContentID);
        if (entry is not PettableDataBaseEntry pEntry) return false;
        pEntry.ActiveDatabase = this.ActiveDatabase;
        pEntry.Name = this.Name;
        pEntry.Homeworld = this.Homeworld;
        pEntry.ContentID = this.ContentID;
        pEntry.IsActive = true;
        return true;
    }

    public void UpdateContentID(ulong contentID)
    {
        this.ContentID = contentID;
        IsActive = true;
    }

    public string? GetName(int skeletonID) => ActiveDatabase.GetName(skeletonID);
    public void SetName(int skeletonID, string? name) => ActiveDatabase.SetName(skeletonID, name);

    public void NotifySeenDirty() => ActiveDatabase.MarkDirtyAsNoticed();
}
