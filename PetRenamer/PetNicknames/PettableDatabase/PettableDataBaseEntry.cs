using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PN.S;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using PetRenamer.PetNicknames.TranslatorSystem;

namespace PetRenamer.PetNicknames.PettableDatabase;

// All the [MethodImpl(MethodImplOptions.AggressiveInlining)]
// used to be inlined. In order for code to stay clearer I made them into methods.
// This is clarity only, they can be inlined

internal class PettableDataBaseEntry : IPettableDatabaseEntry
{
    public bool IsActive { get; private set; }

    public ulong ContentID { get; private set; }
    public string Name { get; private set; } = "";
    public ushort Homeworld { get; private set; }
    public string HomeworldName { get; private set; } = "";

    public ImmutableArray<int> SoftSkeletons { get; private set; } = new ImmutableArray<int>();

    public INamesDatabase ActiveDatabase { get; } = new PettableNameDatabase([], []);
    public INamesDatabase[] AllDatabases { get => [ActiveDatabase]; }

    bool _IsDirtyForUI;
    bool _IsDirty;
    public bool IsDirty { get => _IsDirty || ActiveDatabase.IsDirty;  }
    public bool IsDirtyForUI { get => _IsDirtyForUI || ActiveDatabase.IsDirtyForUI; }
    public bool IsCleared { get; private set; } = false;

    public bool IsIPC { get; private set; } = false;
    public bool IsLegacy { get; private set; } = false;

    readonly IPetServices PetServices;

    public PettableDataBaseEntry(in IPetServices petServices, ulong contentID, string name, ushort homeworld, int[] ids, string[] names, int[] softSkeletons, bool active, bool isLegacy = false)
    {
        PetServices = petServices;
        SetName(name);
        SetActiveDatabase(ids, names);
        SetSoftSkeletons(softSkeletons);
        SetHomeworld(homeworld);
        ContentID = contentID;
        IsActive = active;
        IsIPC = !IsActive;
        IsLegacy = isLegacy;
    }

    public void UpdateEntry(IPettableUser pettableUser)
    {
        SetName(pettableUser.Name);
        SetHomeworld(pettableUser.Homeworld);

        if (IsActive) return;
        if (!pettableUser.IsLocalPlayer) return;
        MarkDirty();
    }

    public bool MoveToDataBase(IPettableDatabase database)
    {
        IPettableDatabaseEntry entry = database.GetEntry(ContentID);
        if (entry is not PettableDataBaseEntry pEntry) return false;
        pEntry.SetActiveDatabase(ActiveDatabase.IDs, ActiveDatabase.Names);
        pEntry.SetName(Name);
        pEntry.SetHomeworld(Homeworld);
        pEntry.SetSoftSkeletons(SoftSkeletons.ToArray());
        pEntry.MarkDirty();
        pEntry.UpdateContentID(ContentID, !IsIPC);
        return true;
    }

    void SetHomeworld(ushort homeworld)
    {
        Homeworld = homeworld;
        HomeworldName = PetServices.PetSheets.GetWorldName(Homeworld) ?? Translator.GetLine("...");
    }

    void SetName(string name)
    {
        Name = name;
    }

    void SetActiveDatabase(int[] ids, string[] names)
    {
        ActiveDatabase.Update(ids, names);
    }

    void SetSoftSkeletons(int[] softSkeletons)
    {
        SoftSkeletons = ImmutableArray.Create(softSkeletons);
    }

    public void UpdateContentID(ulong contentID, bool removeIPCStatus = false)
    {
        ContentID = contentID;
        IsActive = true;
        if (removeIPCStatus) IsIPC = false;
    }

    public string? GetName(int skeletonID) => ActiveDatabase.GetName(skeletonID);
    public void SetName(int skeletonID, string? name) => ActiveDatabase.SetName(skeletonID, name);

    public void NotifySeenDirty()
    {
        _IsDirty = false;
        ActiveDatabase.MarkDirtyAsNoticed();
    }

    public void MarkDirtyUIAsNotified()
    {
        _IsDirtyForUI = false;
        ActiveDatabase.MarkDirtyUIAsNotified();
    }

    public void NotifySeenCleared()
    {
        IsCleared = false;
    }

    public int? GetSoftSkeleton(int softIndex)
    {
        if (softIndex < 0 || softIndex >= SoftSkeletons.Length) return null;
        return SoftSkeletons[softIndex];
    }

    public void SetSoftSkeleton(int index, int softSkeleton)
    {
        if (index < 0 || index >= SoftSkeletons.Length) return;

        int[] temporaryArray = SoftSkeletons.ToArray();
        int oldSkeleton = temporaryArray[index];

        if (oldSkeleton == softSkeleton) return;

        MarkDirty();

        temporaryArray[index] = softSkeleton;
        SoftSkeletons = ImmutableArray.Create(temporaryArray);
    }

    public SerializableUserV4 SerializeEntry() => new SerializableUserV4(this);

    public void UpdateEntry(IModernParseResult parseResult, bool asIPC)
    {
        UpdateEntryBase(parseResult, asIPC);

        SetSoftSkeletons(parseResult.SoftSkeletons);
        UpdateContentID(parseResult.ContentID);
    }

    public void UpdateEntryBase(IBaseParseResult parseResult, bool asIPC)
    {
        SetActiveDatabase(parseResult.IDs, parseResult.Names);
        SetName(parseResult.UserName);
        SetHomeworld(parseResult.Homeworld);

        MarkDirty();

        if (!IsIPC) return;
        IsIPC = asIPC;
    }

    public void Clear()
    {
        SetActiveDatabase([], []);
        IsIPC = false;
        IsActive = false;

        MarkCleared();
        MarkDirty();
    }

    void MarkCleared()
    {
        IsCleared = true;
    }

    void MarkDirty()
    {
        _IsDirty = true;
        _IsDirtyForUI = true;
    }
}
