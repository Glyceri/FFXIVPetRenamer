using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableUserTargetManager : IPettableUserTargetManager
{
    private readonly IPettableUser     Self;
    private readonly IPettableUserList UserList;

    public PettableUserTargetManager(IPettableUser self, IPettableUserList userList)
    {
        Self     = self;
        UserList = userList;
    }

    private IPettableEntity? PettableEntityFromTargetId(GameObjectId targetId)
    {
        if (targetId == PluginConstants.InvalidId)
        {
            return null;
        }

        IPettablePet? finalPet = UserList.GetPet(targetId.Id);

        if (finalPet != null)
        {
            return finalPet;
        }

        IPettableUser? finalUser = UserList.GetUserFromObjectId(targetId.ObjectId);

        if (finalUser != null)
        {
            return finalUser;
        }

        return null;
    }

    private IPettableUser? AsUser(IPettableEntity? entity)
    {
        if (entity == null)
        {
            return null;
        }

        if (entity is not IPettableUser user)
        {
            return null;
        }

        return user;
    }

    public IPettableEntity? GetLeadingTarget()
        => GetSoftTarget() ?? GetTarget();

    public IPettableEntity? GetSoftTarget()
        => PettableEntityFromTargetId(Self.BattleChara->GetSoftTargetId());

    public IPettableEntity? GetTarget()
        => PettableEntityFromTargetId(Self.BattleChara->GetTargetId());

    public IPettableEntity? GetLeadingTargetOfLeadingTarget()
        => AsUser(GetLeadingTarget())?.TargetManager?.GetLeadingTarget();

    public IPettableEntity? GetSoftTargetOfLeadingTarget()
        => AsUser(GetLeadingTarget())?.TargetManager?.GetSoftTarget();

    public IPettableEntity? GetTargetOfLeadingTarget()
        => AsUser(GetLeadingTarget())?.TargetManager?.GetTarget();

    public IPettableEntity? GetTargetOfTarget()
        => AsUser(GetTarget())?.TargetManager?.GetTarget();

    public IPettableEntity? GetSoftTargetOfTarget()
        => AsUser(GetTarget())?.TargetManager?.GetSoftTarget();

    public IPettableEntity? GetTargetOfSoftTarget()
        => AsUser(GetSoftTarget())?.TargetManager?.GetTarget();

    public IPettableEntity? GetSoftTargetOfSoftTarget()
        => AsUser(GetSoftTarget())?.TargetManager?.GetSoftTarget();
}
