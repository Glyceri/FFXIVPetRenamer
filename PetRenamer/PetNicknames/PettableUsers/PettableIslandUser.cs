using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableIslandUser : IIslandUser
{
    public string       Name      { get; }
    public ulong        ContentId { get; }
    public ushort       Homeworld { get; }
    public uint         EntityId  { get; }
    
    public BattleChara* BattleChara   { get; } = null;
    public GameObjectId ObjectId      { get; } = 0;
    public uint         CurrentCastId { get; } = 0;
    public nint         Address       { get; } = 0;
    
    public IPettableDatabaseEntry DataBaseEntry { get; }
    public List<IPettablePet>     PettablePets  { get; } = [];
    
    private readonly IPetServices PetServices;

    public PettableIslandUser(IPetServices petServices, IPettableDatabaseEntry entry)
    {
        PetServices     = petServices;

        DataBaseEntry   = entry;
        Name            = entry.Name;
        ContentId       = entry.ContentId;
        Homeworld       = entry.Homeworld;
        EntityId        = (uint)PluginConstants.InvalidId;
        
        entry.RegisterUsage();
    }

    public bool IsActive
        => true;

    public bool IsLocalPlayer
        => false;
    
    public void SetBattlePet(BattleChara* pointer)
    {
        PettablePets.Add(new PettableIslandPet(pointer, this, PetServices));
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

            if (pet.ObjectId != pointer->GetGameObjectId())
            {
                continue;
            }

            PettablePets.RemoveAt(i);
        }
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

            if (pPet.ObjectId != (ulong)gameObjectId)
            {
                continue;
            }

            return pPet;
        }

        return null;
    }
    
    public string? GetCustomName(PetSkeleton petSkeleton) 
        => DataBaseEntry.GetName(petSkeleton);

    public IPettablePet? GetYoungestPet(SkeletonType filter = SkeletonType.None)
        => null;

    public void OnLastCastChanged(uint cast) { } // Unused
    public void Update() { } // Unused
    public void SetCompanion(Companion* companion) { } // Unused
    public void RemoveCompanion() { } // Unused
    public void Recalculate() { } // unused

    public void Dispose(IPettableDatabase d)
    {
        DataBaseEntry.DeregisterUsage();
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
