using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDataBaseEntry : IPettableDatabaseEntry
{
    public bool IsActive { get; private set; }

    public ulong ContentID { get; private set; }
    public string Name { get; private set; } = "";
    public ushort Homeworld { get; private set; }
    public string HomeworldName { get; private set; }

    public int[] SoftSkeletons { get; private set; }

    public INamesDatabase ActiveDatabase { get; private set; }
    public INamesDatabase[] AllDatabases { get => [ActiveDatabase]; }

    bool _IsDirty;
    public bool IsDirty { get => _IsDirty || ActiveDatabase.IsDirty; }
    
    public bool Destroyed { get; private set; } = false;
    public bool OldUser { get; private set; } = false;

    readonly IPetServices PetServices;

    public PettableDataBaseEntry(in IPetServices petServices, ulong contentID, string name, ushort homeworld, int[] ids, string[] names, int[] softSkeletons, bool isActive)
    {
        PetServices = petServices;
        ContentID = contentID;
        Name = name;
        ActiveDatabase = new PettableNameDatabase(ids, names);
        IsActive = isActive;
        Homeworld = homeworld;
        SoftSkeletons = softSkeletons;
        HomeworldName = petServices.PetSheets.GetWorldName(Homeworld) ?? "[No Homeworld Found]";
    }

    public void UpdateEntry(IPettableUser pettableUser)
    {
        Homeworld = pettableUser.Homeworld;
        Name = pettableUser.Name;
        HomeworldName = pettableUser.HomeworldName;

        if (IsActive) return;
        if (!pettableUser.IsLocalPlayer) return;
        _IsDirty = true;
    }

    public int Length() => ActiveDatabase.IDs.Length;

    public bool MoveToDataBase(IPettableDatabase database)
    {
        IPettableDatabaseEntry entry = database.GetEntry(ContentID);
        if (entry is not PettableDataBaseEntry pEntry) return false;
        pEntry.ActiveDatabase = this.ActiveDatabase;
        pEntry.Name = this.Name;
        pEntry.Homeworld = this.Homeworld;
        pEntry.HomeworldName = PetServices.PetSheets.GetWorldName(Homeworld) ?? "[No Homeworld Found]";
        pEntry.ContentID = this.ContentID;
        pEntry.IsActive = true;
        pEntry.SoftSkeletons = this.SoftSkeletons;
        pEntry._IsDirty = true;
        pEntry.OldUser = true;
        return true;
    }

    public void UpdateContentID(ulong contentID)
    {
        this.ContentID = contentID;
        IsActive = true;
    }

    public string? GetName(int skeletonID) => ActiveDatabase.GetName(skeletonID);
    public void SetName(int skeletonID, string? name) => ActiveDatabase.SetName(skeletonID, name);

    public void NotifySeenDirty()
    {
        _IsDirty = false;
        ActiveDatabase.MarkDirtyAsNoticed();
    }

    public string? GetSoftName(int softIndex) => GetName(GetSoftSkeleton(softIndex) ?? 0);
    public int? GetSoftSkeleton(int softIndex)
    {
        if (softIndex < 0 || softIndex >= SoftSkeletons.Length) return null;
        return SoftSkeletons[softIndex];
    }

    public void Destroy() => Destroyed = true;

    public SerializableUserV4 SerializeEntry() => new SerializableUserV4(this);
}
