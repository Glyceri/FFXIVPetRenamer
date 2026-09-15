using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Structs;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableUser : IPettableUser
{
    public nint         Address       { get; }
    public BattleChara* BattleChara   { get; }
    public GameObjectId ObjectId      { get; }
    public ActionData   CurrentAction { get; private set; } = new ActionData();
    public bool         IsLocalPlayer { get; }

    public List<IPettablePet>     PettablePets  { get; } = [];
    public IPettableDatabaseEntry DataBaseEntry { get; }

    private uint _lastCast;

    private readonly IPetServices       PetServices;
    private readonly ISharingDictionary SharingDictionary;

    public PettableUser(IPetServices petServices, ISharingDictionary sharingDictionary, BattleChara* battleChara)
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

        IPettableDatabaseEntry? legacyEntry = PetServices.LegacyDatabase.GetEntry(BattleChara->NameString, BattleChara->HomeWorld, false);

        if (legacyEntry != null)
        {
            legacyEntry.UpdateContentId(BattleChara->ContentId, true);
            PetServices.LegacyDatabase.RemoveEntry(legacyEntry, ParseSource.Manual);
            _ = legacyEntry.MoveToDataBase(PetServices.Database);
            PetServices.LegacyDatabase.SetDirty();
        }

        DataBaseEntry = PetServices.Database.GetEntry(BattleChara->ContentId);
        DataBaseEntry.RegisterUsage();
        DataBaseEntry.UpdateEntry(BattleChara->NameString, BattleChara->HomeWorld, IsLocalPlayer);

        if (IsLocalPlayer)
        {
            DataBaseEntry.UpdateContentId(BattleChara->ContentId, true);
        }

        PetServices.PetLog.LogVerbose($"Just created a new user: {DataBaseEntry.ContentId}@{DataBaseEntry.HomeworldName}, Address: {Address}, ContentID: {DataBaseEntry.ContentId}");
    }

    public void Dispose()
    {
        DataBaseEntry.DeregisterUsage();

        PetServices.PetLog.DevLogVerbose($"Just removed the user: {DataBaseEntry.Name}@{DataBaseEntry.HomeworldName}, Address: {Address}, ContentID: {DataBaseEntry.ContentId}");

        PetServices.DirtyListener.UnregisterOnClearEntry(OnDirty);
        PetServices.DirtyListener.UnregisterOnDirtyEntry(OnDirty);
        PetServices.DirtyListener.UnregisterOnDirtyName(OnDirty);

        if (DataBaseEntry.IsIpc)
        {
            DataBaseEntry.Clear(ParseSource.IPC);
        }

        if (!IsActive)
        {
            PetServices.Database.RemoveEntry(DataBaseEntry, ParseSource.IPC);
        }

        foreach (IPettablePet pet in PettablePets)
        {
            pet.Dispose();
        }
    }

    public bool IsActive
        => DataBaseEntry.IsActive;

    public string? GetCustomName(PetSkeleton petSkeleton)
        => DataBaseEntry.GetName(petSkeleton);

    public void Update()
    {
        if (_lastCast == BattleChara->CastInfo.ActionId)
        {
            return;
        }
        
        OnLastCastChanged(new ActionData(BattleChara->CastInfo.ActionId, (ActionKind)BattleChara->CastInfo.ActionType));
    }
    
    public void OnLastCastChanged(ActionData actionData)
    {
        if (!IsActive)
        {
            return;
        }

        CurrentAction = actionData;

        if (_lastCast == CurrentAction.ActionId)
        {
            return;
        }
        
        int? softIndex = PetServices.PetSheets.CastToSoftIndex(_lastCast);

        _lastCast = CurrentAction.ActionId;

        if (CurrentAction.ActionId != 0)
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
        PetServices.PetLog.DevLogVerbose($"Added the pet: {pet.Address}, and the ObjectID: {pet.ObjectId} to the user: {DataBaseEntry.Name}@{DataBaseEntry.HomeworldName}, Address: {Address}, ContentID: {DataBaseEntry.ContentId}");

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

    public IPettablePet? GetYoungestPet(SkeletonType[] filter)
    {
        IPettablePet? youngestPet = null;
        
        int index = -1;
        
        while (++index < filter.Length && youngestPet == null)
        {
            SkeletonType skeletonType = filter[index];
            
            youngestPet = GetYoungestPet(skeletonType);
        }
        
        return youngestPet;
    }
    
    public void AddBattlePet(BattleChara* pointer)
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

        switch (pointer->BattleNpcSubKind)
        {
            case BattleNpcSubKind.Buddy:      CreateNewPet(new PettableChocoboPet(pointer, this, SharingDictionary, DataBaseEntry, PetServices)); break;
            case BattleNpcSubKind.Pet:        HandleSubKindPet(pointer); break;
            case BattleNpcSubKind.LovmMinion: CreateNewPet(new PettableLovmPet(pointer, this, SharingDictionary, DataBaseEntry, PetServices));    break;
        }
    }
    
    private void HandleSubKindPet(BattleChara* pointer)
    {
        uint modelCharaId = (uint)pointer->ModelContainer.ModelCharaId;
        
        if (InList(new PetSkeleton(SkeletonType.BattlePet, modelCharaId), PluginConstants.BattlePetRegistrations))
        {
            CreateNewPet(new PettablePet(pointer, this, SharingDictionary, DataBaseEntry, PetServices));
        }
        else if (InList(new PetSkeleton(SkeletonType.BeastMaster, modelCharaId), PluginConstants.BeastMasterPetRegistrations))
        {
            CreateNewPet(new PettableBeastMasterPet(pointer, this, SharingDictionary, DataBaseEntry, PetServices));
        }
    }
    
    private bool InList(PetSkeleton petSkeleton, PetRegistration[] petRegistrations)
    {
        foreach (PetRegistration petRegistration in petRegistrations)
        {
            if (petRegistration.PetSkeleton != petSkeleton)
            {
                continue;
            }
        
            return true;
        }
        
        return false;
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
