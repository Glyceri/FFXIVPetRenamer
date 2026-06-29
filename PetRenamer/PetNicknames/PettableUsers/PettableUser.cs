using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableUser : IPettableUser
{
    public GameObjectId ObjectId    { get; }

    public List<IPettablePet> PettablePets { get; } = [];

    public nint         Address     { get; }
    public BattleChara* BattleChara { get; }

    public IPettableDatabaseEntry DataBaseEntry { get; }

    public uint EntityId      { get; }
    public uint CurrentCastId { get; private set; }
    public bool IsLocalPlayer { get; }

    private uint _lastCast;

    private readonly IPetServices       PetServices;
    private readonly ISharingDictionary SharingDictionary;

    public PettableUser(IPetServices petServices, ISharingDictionary sharingDictionary, IPettableDatabase dataBase, ILegacyDatabase legacyDatabase, BattleChara* battleChara)
    {
        PetServices       = petServices;
        SharingDictionary = sharingDictionary;

        PetServices.DirtyListener.RegisterOnClearEntry(OnDirty);
        PetServices.DirtyListener.RegisterOnDirtyEntry(OnDirty);
        PetServices.DirtyListener.RegisterOnDirtyName(OnDirty);

        BattleChara     = battleChara;
        Address         = (nint)BattleChara;

        IsLocalPlayer   = BattleChara->ObjectIndex == 0;
        ObjectId        = BattleChara->GetGameObjectId();
        EntityId        = BattleChara->EntityId;

        IPettableDatabaseEntry? legacyEntry = legacyDatabase.GetEntry(BattleChara->NameString, BattleChara->HomeWorld, false);

        if (legacyEntry != null)
        {
            legacyEntry.UpdateContentId(BattleChara->ContentId, true);
            legacyDatabase.RemoveEntry(legacyEntry, ParseSource.Manual);
            _ = legacyEntry.MoveToDataBase(dataBase);
            legacyDatabase.SetDirty();
        }

        DataBaseEntry = dataBase.GetEntry(BattleChara->ContentId);
        DataBaseEntry.RegisterUsage();
        DataBaseEntry.UpdateEntry(BattleChara->NameString, BattleChara->HomeWorld, IsLocalPlayer);

        if (IsLocalPlayer)
        {
            DataBaseEntry.UpdateContentId(BattleChara->ContentId, true);
        }

        if (petServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.LogVerbose($"Just created a new user: {DataBaseEntry.ContentId}@{DataBaseEntry.HomeworldName}, Address: {Address}, ContentID: {DataBaseEntry.ContentId}");
        }
    }

    public bool IsActive
        => DataBaseEntry.IsActive;

    public void Update()
    {
        CurrentCastId = BattleChara->CastInfo.ActionId;

        if (_lastCast == CurrentCastId)
        {
            return;
        }
        
        OnLastCastChanged(CurrentCastId);
    }

    public void OnLastCastChanged(uint cast)
    {
        if (!IsActive)
        {
            return;
        }

        CurrentCastId = cast;

        if (_lastCast == CurrentCastId)
        {
            return;
        }
        
        int? softIndex = PetServices.PetSheets.CastToSoftIndex(_lastCast);

        _lastCast = CurrentCastId;

        if (CurrentCastId != 0)
        {
            return;
        }

        if (softIndex == null)
        {
            return;
        }

        int sIndex = softIndex.Value;

        IPettablePet? youngestPet = GetYoungestPet(SkeletonType.BattlePet);

        if (youngestPet == null)
        {
            return;
        }

        DataBaseEntry.SetSoftSkeleton(sIndex, youngestPet.SkeletonId);
    }

    private void OnDirty(INamesDatabase database)
    {
        if (database != DataBaseEntry.ActiveDatabase)
        {
            return;
        }

        Recalculate();
    }

    private void OnDirty(IPettableDatabaseEntry entry)
    {
        if (entry != DataBaseEntry)
        {
            return;
        }

        Recalculate();
    }

    public void Recalculate()
    {
        foreach (IPettablePet pet in PettablePets)
        {
            pet.Recalculate();
        }
    }

    private void CreateNewPet(IPettablePet pet, int index = -1)
    {
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.LogVerbose($"Added the pet: {pet.Address}, and the ObjectID: {pet.ObjectId} to the user: {DataBaseEntry.Name}@{DataBaseEntry.HomeworldName}, Address: {Address}, ContentID: {DataBaseEntry.ContentId}");
        }

        if (index == -1)
        {
            PettablePets.Add(pet);
        }
        else
        {
            PettablePets.Insert(index, pet);
        }

        PetServices.DirtyCaller.DirtyPlayer(this);
    }

    public IPettablePet? GetPet(nint pet)
    {
        int petCount = PettablePets.Count;

        for (int i = 0; i < petCount; i++)
        {
            IPettablePet pPet = PettablePets[i];

            if (pPet.Address != pet)
            {
                continue; 
            }
            
            return pPet;
        }

        return null;
    }

    public IPettablePet? GetPet(GameObjectId gameObjectId)
    {
        int petCount = PettablePets.Count;

        for (int i = 0; i < petCount; i++)
        {
            IPettablePet pPet = PettablePets[i];

            if (pPet.ObjectId == (ulong)gameObjectId)
            {
                return pPet;
            }
        }

        return null;
    }

    public IPettablePet? GetYoungestPet(SkeletonType skeletonType = SkeletonType.None)
    {
        for (int i = PettablePets.Count - 1; i >= 0; i--)
        {
            IPettablePet pPet = PettablePets[i];

            if ((int)skeletonType == 0)
            {
                return pPet;
            }
            
            if (pPet.SkeletonId.SkeletonType != skeletonType)
            {
                continue;
            }
            
            return pPet;
        }

        return null;
    }

    public string? GetCustomName(PetSkeleton petSkeleton) 
        => DataBaseEntry.GetName(petSkeleton);
    
    public void Dispose(IPettableDatabase database)
    {
        DataBaseEntry.DeregisterUsage();
        
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.LogVerbose($"Just removed the user: {DataBaseEntry.Name}@{DataBaseEntry.HomeworldName}, Address: {Address}, ContentID: {DataBaseEntry.ContentId}");
        }

        PetServices.DirtyListener.UnregisterOnClearEntry(OnDirty);
        PetServices.DirtyListener.UnregisterOnDirtyEntry(OnDirty);
        PetServices.DirtyListener.UnregisterOnDirtyName(OnDirty);

        if (DataBaseEntry.IsIpc)
        {
            DataBaseEntry.Clear(ParseSource.IPC);
        }

        if (!IsActive)
        {
            database.RemoveEntry(DataBaseEntry, ParseSource.IPC);
        }

        foreach(IPettablePet pet in PettablePets)
        {
            pet.Dispose();
        }
    }

    public void SetBattlePet(BattleChara* pointer)
    {
        for (int i = PettablePets.Count - 1; i >= 0; i--)
        {
            IPettablePet pet = PettablePets[i];

            if (pet.Address != (nint)pointer)
            {
                continue;
            }

            return;
        }

        CreateNewPet(new PettableBattlePet(pointer, this, SharingDictionary, DataBaseEntry, PetServices));
    }

    public void RemoveBattlePet(BattleChara* pointer)
    {
        if (pointer == null)
        {
            return;
        }

        for (int i = PettablePets.Count - 1; i >= 0; i--)
        {
            IPettablePet pet = PettablePets[i];
            
            if (pet.Address != (nint)pointer)
            {
                continue;
            }

            pet.Dispose();
            PettablePets.RemoveAt(i);
        }
    }

    public void SetCompanion(Companion* companion)
    {
        RemoveCompanion();

        CreateNewPet(new PettableCompanion(companion, this, SharingDictionary, DataBaseEntry, PetServices), 0);
    }

    public void RemoveCompanion()
    {
        if (PettablePets.Count == 0)
        {
            return;
        }

        if (PettablePets[0] is not PettableCompanion pCompanion)
        {
            return;
        }

        pCompanion.Dispose();
        PettablePets.RemoveAt(0);
    }
    
    public void GetDrawColours(PetSkeleton petSkeleton, Configuration.ColourConfig colourConfig, out Vector3? edgeColour, out Vector3? textColour)
    {
        edgeColour = null;
        textColour = null;

        Configuration.ColourMode colourSetting = PetServices.Configuration.SelectedColourMode;

        if (colourConfig.OverrideColourMode)
        {
            colourSetting = colourConfig.ColourMode;
        }
        
        if (colourSetting == Configuration.ColourMode.None)
        {
            return;
        }

        if (colourSetting == Configuration.ColourMode.Personal && !IsLocalPlayer)
        {
            return;
        }

        edgeColour = DataBaseEntry.GetEdgeColour(petSkeleton);
        textColour = DataBaseEntry.GetTextColour(petSkeleton);
    }
}
