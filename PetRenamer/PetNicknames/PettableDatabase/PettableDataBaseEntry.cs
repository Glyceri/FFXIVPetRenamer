using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using PN.S;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableDataBaseEntry : IPettableDatabaseEntry
{
    public bool IsActive        { get; private set; }

    public ulong ContentID      { get; private set; }
    public string Name          { get; private set; } = string.Empty;
    public ushort Homeworld     { get; private set; }
    public string HomeworldName { get; private set; } = string.Empty;

    public ImmutableArray<PetSkeleton> SoftSkeletons { get; private set; } = [];

    public INamesDatabase ActiveDatabase { get; }

    public bool IsIPC       { get; private set; } = false;
    public bool IsLegacy    { get; private set; } = false;

    private readonly IPetServices         PetServices;
    private readonly IPettableDirtyCaller DirtyCaller;

    public PettableDataBaseEntry(IPetServices petServices, IPettableDirtyCaller dirtyCaller, ulong contentID, string name, ushort homeworld, PetSkeleton[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours, PetSkeleton[] softSkeletons, bool active, bool isLegacy = false)
    {
        PetServices    = petServices;
        DirtyCaller    = dirtyCaller;
        ActiveDatabase = new PettableNameDatabase([], [], [], [], DirtyCaller);

        ContentID      = contentID;
        IsActive       = active;
        IsIPC          = !IsActive;
        IsLegacy       = isLegacy;

        SetName(name);
        SetSoftSkeletons(softSkeletons);
        SetActiveDatabase(ids, names, edgeColours, textColours);
        SetHomeworld(homeworld);
    }

    public INamesDatabase[] AllDatabases 
        => [ActiveDatabase];

    public void UpdateEntry(IPettableUser pettableUser)
    {
        SetName(pettableUser.Name);
        SetHomeworld(pettableUser.Homeworld);

        if (IsActive)
        {
            return;
        }

        if (!pettableUser.IsLocalPlayer)
        {
            return;
        }

        MarkDirty();
    }

    public bool MoveToDataBase(IPettableDatabase database)
    {
        IPettableDatabaseEntry entry = database.GetEntry(ContentID);

        if (entry is not PettableDataBaseEntry pEntry)
        {
            return false;
        }

        pEntry.SetActiveDatabase(ActiveDatabase.IDs, ActiveDatabase.Names, ActiveDatabase.EdgeColours, ActiveDatabase.TextColours);
        pEntry.SetName(Name);
        pEntry.SetHomeworld(Homeworld);
        pEntry.SetSoftSkeletons(SoftSkeletons.ToArray());
        pEntry.UpdateContentID(ContentID, !IsIPC);
        pEntry.MarkDirty();

        return true;
    }

    private void SetHomeworld(ushort homeworld)
    {
        Homeworld = homeworld;
        HomeworldName = PetServices.PetSheets.GetWorldName(Homeworld) ?? Translator.GetLine("...");
    }

    private void SetName(string name)
    {
        Name = name;
    }

    private void SetActiveDatabase(PetSkeleton[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours)
    {
        ActiveDatabase.Update(ids, names, edgeColours, textColours, DirtyCaller);
    }

    private void SetSoftSkeletons(PetSkeleton[] softSkeletons)
    {
        SoftSkeletons = ImmutableArray.Create(softSkeletons);
    }

    public void UpdateContentID(ulong contentID, bool removeIPCStatus = false)
    {
        ContentID = contentID;
        IsActive  = true;

        if (removeIPCStatus)
        {
            IsIPC = false;
        }
    }

    public string? GetName(PetSkeleton skeletonID)
        => ActiveDatabase.GetName(skeletonID);

    public Vector3? GetEdgeColour(PetSkeleton skeletonID)
        => ActiveDatabase.GetEdgeColour(skeletonID);

    public Vector3? GetTextColour(PetSkeleton skeletonID)
        => ActiveDatabase?.GetTextColour(skeletonID);

    public void SetName(PetSkeleton skeletonID, string? name, Vector3? edgeColour, Vector3? textColour)
        => ActiveDatabase.SetName(skeletonID, name, edgeColour, textColour);
    

    public PetSkeleton? GetSoftSkeleton(int softIndex)
    {
        if (softIndex < 0 || softIndex >= SoftSkeletons.Length)
        {
            return null;
        }

        return SoftSkeletons[softIndex];
    }

    public void SetSoftSkeleton(int index, PetSkeleton softSkeleton)
    {
        if (index < 0 || index >= SoftSkeletons.Length)
        {
            return;
        }

        PetSkeleton[] temporaryArray = SoftSkeletons.ToArray();
        PetSkeleton oldSkeleton      = temporaryArray[index];

        if (oldSkeleton == softSkeleton)
        {
            return;
        }

        temporaryArray[index] = softSkeleton;

        SoftSkeletons = ImmutableArray.Create(temporaryArray);

        MarkDirty();
    }

    public SerializableUserV6 SerializeEntry()
        => new SerializableUserV6(this);

    public void UpdateEntry(IModernParseResult parseResult, ParseSource parseSource)
    {
        UpdateEntryBase(parseResult, parseSource);

        SetSoftSkeletons(parseResult.SoftSkeletons);
        UpdateContentID(parseResult.ContentID);
    }

    public void UpdateEntryBase(IBaseParseResult parseResult, ParseSource parseSource)
    {
        SetActiveDatabase(parseResult.IDs, parseResult.Names, parseResult.EdgeColous, parseResult.TextColours);
        SetName(parseResult.UserName);
        SetHomeworld(parseResult.Homeworld);

        IsIPC = (parseSource == ParseSource.IPC);

        MarkDirty();        
    }

    public void Clear(ParseSource parseSource)
    {
        SetActiveDatabase([], [], [], []);

        IsActive = false;
        IsLegacy = false;

        if (parseSource == ParseSource.IPC || 
            parseSource == ParseSource.None)
        {
            return;
        }

        MarkCleared();
        MarkDirty();
    }

    private void MarkCleared()
    {
        DirtyCaller.ClearEntry(this);
    }

    private void MarkDirty()
    {
        DirtyCaller.DirtyEntry(this);
    }
}
