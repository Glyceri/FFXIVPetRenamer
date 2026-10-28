using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using ITargetManager = PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces.ITargetManager;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal unsafe class TargetManagerWrapper : ITargetManager
{
    private readonly DalamudServices DalamudServices;
    private readonly IUserList       UserList;

    public TargetManagerWrapper(DalamudServices dalamudServices, IUserList userList)
    {
        DalamudServices = dalamudServices;
        UserList        = userList;
    }
    
    public IPettableEntity? SoftTarget
        => GetSoftTarget(UserList.LocalPlayer);

    public IPettableEntity? Target                   
        => GetTarget(UserList.LocalPlayer);

    public IPettableEntity? LeadingTarget
        => GetLeadingTarget(UserList.LocalPlayer);

    public IPettableEntity? TargetOfTarget
        => GetTargetOfTarget(UserList.LocalPlayer);

    public IPettableEntity? LeadingTargetOfTarget
        => GetTargetOfLeadingTarget(UserList.LocalPlayer);

    public IPettableEntity? LeadingTargetOfLeadingTarget
        => GetLeadingTargetOfLeadingTarget(UserList.LocalPlayer);

    public IPettableEntity? TargetOfLeadingTarget
        => GetTargetOfLeadingTarget(UserList.LocalPlayer);

    public IPettableEntity? FocusTarget              
        => GetEntity(DalamudServices.TargetManager.FocusTarget);

    public IPettableEntity? MouseOverTarget          
        => GetEntity(DalamudServices.TargetManager.MouseOverTarget);

    public IPettableEntity? PreviousTarget           
        => GetEntity(DalamudServices.TargetManager.PreviousTarget);

    public IPettableEntity? GPoseTarget              
        => GetEntity(DalamudServices.TargetManager.GPoseTarget);

    public IPettableEntity? MouseOverNameplateTarget 
        => GetEntity(DalamudServices.TargetManager.MouseOverNameplateTarget);

    private IPettableEntity? GetEntity(IGameObject? target)
    {
        if (target == null)
        {
            return null;
        }

        return GetEntity(target.Address);
    }

    private IPettableEntity? GetEntity(nint address)
    {
        IPettableEntity? entity = UserList.GetUser(address, UserListFindType.Direct);

        if (entity != null)
        {
            return entity;
        }

        return UserList.GetPet(address);
    }
    
    private IPettableEntity? PettableEntityFromTargetId(GameObjectId targetId)
    {
        if (targetId == PluginConstants.InvalidId)
        {
            return null;
        }

        IPettableEntity? entity = UserList.GetPet(targetId);
        
        entity ??= UserList.GetUserFromObjectId(targetId);

        return entity;
    }

    private IPettableBattleEntity? AsBattleEntity(IPettableEntity? entity)
    {
        if (entity is not IPettableBattleEntity battleEntity)
        {
            return null;
        }

        return battleEntity;
    }

    public IPettableEntity? GetLeadingTarget(IPettableBattleEntity? battleEntity)
        => GetSoftTarget(battleEntity) ?? GetTarget(battleEntity);

    public IPettableEntity? GetSoftTarget(IPettableBattleEntity? battleEntity)
        => battleEntity != null ? PettableEntityFromTargetId(battleEntity.BattleChara->GetSoftTargetId()) : null;

    public IPettableEntity? GetTarget(IPettableBattleEntity? battleEntity)
        => battleEntity != null ? PettableEntityFromTargetId(battleEntity.BattleChara->GetTargetId()) : null;

    public IPettableEntity? GetLeadingTargetOfLeadingTarget(IPettableBattleEntity? battleEntity)
        => GetLeadingTarget(AsBattleEntity(GetLeadingTarget(battleEntity)));

    public IPettableEntity? GetSoftTargetOfLeadingTarget(IPettableBattleEntity? battleEntity)
        => GetSoftTarget(AsBattleEntity(GetLeadingTarget(battleEntity)));

    public IPettableEntity? GetTargetOfLeadingTarget(IPettableBattleEntity? battleEntity)
        => GetTarget(AsBattleEntity(GetLeadingTarget(battleEntity)));

    public IPettableEntity? GetTargetOfTarget(IPettableBattleEntity? battleEntity)
        => GetTarget(AsBattleEntity(GetTarget(battleEntity)));

    public IPettableEntity? GetSoftTargetOfTarget(IPettableBattleEntity? battleEntity)
        => GetSoftTarget(AsBattleEntity(GetTarget(battleEntity)));

    public IPettableEntity? GetTargetOfSoftTarget(IPettableBattleEntity? battleEntity)
        => GetTarget(AsBattleEntity(GetSoftTarget(battleEntity)));

    public IPettableEntity? GetSoftTargetOfSoftTarget(IPettableBattleEntity? battleEntity)
        => GetSoftTarget(AsBattleEntity(GetSoftTarget(battleEntity)));
}
