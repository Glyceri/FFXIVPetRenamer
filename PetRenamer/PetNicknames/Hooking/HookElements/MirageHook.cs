using Dalamud.Game.Config;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class MirageHook : HookableElement
{
    private uint _carbuncleMirage;
    private uint _eosMirage;
    private uint _egiGarudaMirage;
    private uint _egiTitanMirage;
    private uint _egiIfritMirage;
    
    public MirageHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices) { }
    
    public override void Init()
    {
        DalamudServices.GameConfig.Changed += OnConfigChanged;
        
        PetServices.DirtyListener.RegisterOnPlayerCharacterDirty(OnDirtyUser);
        
        UpdateMirageConfig();
    }

    protected override void OnDispose()
    {
        DalamudServices.GameConfig.Changed -= OnConfigChanged;
        
        PetServices.DirtyListener.UnregisterOnPlayerCharacterDirty(OnDirtyUser);
    }
    
    private void OnDirtyUser(IPettableUser user)
    {
        if (!user.IsLocalPlayer)
        {
            return;
        }
        
        UpdateMirageConfig();
    }
    
    private PetSkeleton GetPetSkeleton(uint id, PetSkeleton baseSkeleton)
        => new PetSkeleton(PetServices.PetSheets.GetPetMirage(id)?.ModelChara.RowId ?? PetServices.PetSheets.GetPet(baseSkeleton)?.Model.SkeletonId ?? 0, SkeletonType.BattlePet);
    
    private void OnConfigChanged(object? _, ConfigChangeEvent change)
    {  
        if (change.Option 
            is  not UiConfigOption.PetMirageTypeCarbuncleSupport 
            and not UiConfigOption.PetMirageTypeFairy
            and not UiConfigOption.EgiMirageTypeGaruda
            and not UiConfigOption.EgiMirageTypeTitan
            and not UiConfigOption.EgiMirageTypeIfrit)
        {
            return;
        }
        
        UpdateMirageConfig();
    }
    
    private void UpdateMirageConfig()
    {
        if (PetServices.UserList.LocalPlayer == null)
        {
            return;
        }
        
        DalamudServices.GameConfig.TryGet(UiConfigOption.PetMirageTypeCarbuncleSupport, out _carbuncleMirage);
        DalamudServices.GameConfig.TryGet(UiConfigOption.PetMirageTypeFairy,            out _eosMirage);
        DalamudServices.GameConfig.TryGet(UiConfigOption.EgiMirageTypeGaruda,           out _egiGarudaMirage);
        DalamudServices.GameConfig.TryGet(UiConfigOption.EgiMirageTypeTitan,            out _egiTitanMirage);
        DalamudServices.GameConfig.TryGet(UiConfigOption.EgiMirageTypeIfrit,            out _egiIfritMirage);
        
        // Sheets wrapper explains why the order is like this... it's crucial it stays like this.
        // Soft Mapping is the most hardcoded thing in this plogon :c
        // 0 --> Karfunkel
        // 1 --> Garuda-Egi
        // 2 --> Titan-Egi
        // 3 --> Ifrit-Egi
        // 4 --> Eos
        PetSkeleton carbuncleSkeleton = GetPetSkeleton(_carbuncleMirage, PluginConstants.BaseSummonerSkeleton);
        PetSkeleton garudaSkeleton    = GetPetSkeleton(_egiGarudaMirage, PluginConstants.BaseGarudaEgiSkeleton);
        PetSkeleton titanSkeleton     = GetPetSkeleton(_egiTitanMirage,  PluginConstants.BaseTitanEgiSkeleton);
        PetSkeleton ifritSkeleton     = GetPetSkeleton(_egiIfritMirage,  PluginConstants.BaseIfritEgiSkeleton);
        PetSkeleton eosSkeleton       = GetPetSkeleton(_eosMirage,       PluginConstants.BaseScholarSkeleton);
        
        PetServices.UserList.LocalPlayer.DataBaseEntry.SetSoftSkeleton(0, carbuncleSkeleton);
        PetServices.UserList.LocalPlayer.DataBaseEntry.SetSoftSkeleton(1, garudaSkeleton);
        PetServices.UserList.LocalPlayer.DataBaseEntry.SetSoftSkeleton(2, titanSkeleton);
        PetServices.UserList.LocalPlayer.DataBaseEntry.SetSoftSkeleton(3, ifritSkeleton);
        PetServices.UserList.LocalPlayer.DataBaseEntry.SetSoftSkeleton(4, eosSkeleton);
        
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.LogInfo($"Mirage Settings are: _carbuncleMirage: {carbuncleSkeleton}, _eosMirage: {eosSkeleton}, _egiGarudaMirage: {garudaSkeleton}, _egiTitanMirage: {titanSkeleton}, _egiIfritMirage: {ifritSkeleton}");
        }
    }
}