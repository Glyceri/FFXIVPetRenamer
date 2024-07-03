using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDataBaseEntry : IPettableDatabaseEntry
{
    public ulong ContentID { get; private set; }
    public string Name { get; private set; } = "";

    public int[] IDs { get; private set; }
    public string[] Names { get; private set; }

    public bool IsActive { get; private set; }
    public ushort Homeworld { get; private set; }

    public PettableDataBaseEntry(ulong contentID, string name, ushort homeworld, int[] ids, string[] names, bool isActive)
    {
        ContentID = contentID;
        Name = name;
        IDs = ids;
        Names = names;
        IsActive = isActive;
        Homeworld = homeworld;
    }

    public void UpdateEntry(IPettableUser pettableUser)
    {
        this.Homeworld = pettableUser.Homeworld;
        this.Name = pettableUser.Name;
    }

    public int Length() => IDs.Length;

    public bool MoveToDataBase(IPettableDatabase database)
    {
        IPettableDatabaseEntry entry = database.GetEntry(ContentID);
        if (entry is not PettableDataBaseEntry pEntry) return false;
        pEntry.IDs = this.IDs;
        pEntry.Name = this.Name;
        pEntry.Homeworld = this.Homeworld;
        pEntry.Names = this.Names;
        pEntry.ContentID = this.ContentID;
        pEntry.IsActive = true;
        return true;
    }

    public void UpdateContentID(ulong contentID)
    {
        this.ContentID = contentID;
        IsActive = true;
    }
}
