using Dalamud.Game.ClientState.Objects.Types;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class TargetManagerWrapper : ITargetManager
{
    private readonly DalamudServices   DalamudServices;
    private readonly IPettableUserList UserList;

    public TargetManagerWrapper(DalamudServices dalamudServices, IPettableUserList userList)
    {
        DalamudServices = dalamudServices;
        UserList        = userList;
    }

    public IPettableEntity? Target                   => GetEntity(DalamudServices.TargetManager.Target ?? DalamudServices.TargetManager.SoftTarget);
    public IPettableEntity? TargetOfTarget           => GetEntity(DalamudServices.TargetManager.Target?.TargetObject);
    public IPettableEntity? FocusTarget              => GetEntity(DalamudServices.TargetManager.FocusTarget);
    public IPettableEntity? MouseOverTarget          => GetEntity(DalamudServices.TargetManager.MouseOverTarget);
    public IPettableEntity? PreviousTarget           => GetEntity(DalamudServices.TargetManager.PreviousTarget);
    public IPettableEntity? GPoseTarget              => GetEntity(DalamudServices.TargetManager.GPoseTarget);
    public IPettableEntity? MouseOverNameplateTarget => GetEntity(DalamudServices.TargetManager.MouseOverNameplateTarget);

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
        IPettableEntity? entity = UserList.GetUser(address, false);

        if (entity != null)
        {
            return entity;
        }

        return UserList.GetPet(address);
    }
}
