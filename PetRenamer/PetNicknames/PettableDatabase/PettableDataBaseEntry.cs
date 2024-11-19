using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PN.S;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Immutable;
using System.Linq;
using PetRenamer.PetNicknames.TranslatorSystem;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDataBaseEntry : IPettableDatabaseEntry
{
    public bool IsActive { get; private set; }

    public ulong ContentID { get; private set; }
    public string Name { get; private set; } = "";
    public ushort Homeworld { get; private set; }
    public string HomeworldName { get; private set; } = "";

    public ImmutableArray<int> SoftSkeletons { get; private set; } = new ImmutableArray<int>();

    public INamesDatabase ActiveDatabase { get; }
    public INamesDatabase[] AllDatabases { get => [ActiveDatabase]; }

    public bool IsIPC { get; private set; } = false;
    public bool IsLegacy { get; private set; } = false;

    readonly IPetServices PetServices;
    readonly IPettableDirtyCaller DirtyCaller;

    public PettableDataBaseEntry(IPetServices petServices, IPettableDirtyCaller dirtyCaller, ulong contentID, string name, ushort homeworld, int[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours, int[] softSkeletons, bool active, bool isLegacy = false)
    {
        PetServices = petServices;
        DirtyCaller = dirtyCaller;
        ActiveDatabase = new PettableNameDatabase([], [], [], [], DirtyCaller);

        ContentID = contentID;
        IsActive = active;
        IsIPC = !IsActive;
        IsLegacy = isLegacy;

        SetName(name);
        SetSoftSkeletons(softSkeletons);
        SetActiveDatabase(ids, names, edgeColours, textColours);
        SetHomeworld(homeworld);
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
        pEntry.SetActiveDatabase(ActiveDatabase.IDs, ActiveDatabase.Names, ActiveDatabase.EdgeColours, ActiveDatabase.TextColours);
        pEntry.SetName(Name);
        pEntry.SetHomeworld(Homeworld);
        pEntry.SetSoftSkeletons(SoftSkeletons.ToArray());
        pEntry.UpdateContentID(ContentID, !IsIPC);
        pEntry.MarkDirty();
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

    void SetActiveDatabase(int[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours)
    {
        ActiveDatabase.Update(ids, names, edgeColours, textColours, DirtyCaller);
    }

    void SetSoftSkeletons(int[] softSkeletons)
    {
        SoftSkeletons = ImmutableArray.Create(softSkeletons);
    }

    public void UpdateContentID(ulong contentID, bool removeIPCStatus = false)
    {
        ContentID = contentID;
        IsActive = true;
        if (removeIPCStatus)
        {
            IsIPC = false;
        }
    }

    public string? GetName(int skeletonID) => ActiveDatabase.GetName(skeletonID);
    public Vector3? GetEdgeColour(int skeletonID) => ActiveDatabase.GetEdgeColour(skeletonID);
    public Vector3? GetTextColour(int skeletonID) => ActiveDatabase?.GetTextColour(skeletonID);
    public void SetName(int skeletonID, string? name, Vector3? edgeColour, Vector3? textColour) => ActiveDatabase.SetName(skeletonID, name, edgeColour, textColour);

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

        temporaryArray[index] = softSkeleton;
        SoftSkeletons = ImmutableArray.Create(temporaryArray);

        MarkDirty();
    }

    public SerializableUserV5 SerializeEntry() => new SerializableUserV5(this);

    public void UpdateEntry(IModernParseResult parseResult, bool asIPC)
    {
        UpdateEntryBase(parseResult, asIPC);

        SetSoftSkeletons(parseResult.SoftSkeletons);
        UpdateContentID(parseResult.ContentID);
    }

    public void UpdateEntryBase(IBaseParseResult parseResult, bool asIPC)
    {
        SetActiveDatabase(parseResult.IDs, parseResult.Names, parseResult.EdgeColous, parseResult.TextColours);
        SetName(parseResult.UserName);
        SetHomeworld(parseResult.Homeworld);

        IsIPC = asIPC;

        MarkDirty();        
    }

    public void Clear(bool fromIPC)
    {
        SetActiveDatabase([], [], [], []);
        IsActive = false;
        IsLegacy = false;

        if (fromIPC) return;

        MarkCleared();
        MarkDirty();
    }

    void MarkCleared()
    {
        DirtyCaller.ClearEntry(this);
    }

    void MarkDirty()
    {
        DirtyCaller.DirtyEntry(this);
    }
}
