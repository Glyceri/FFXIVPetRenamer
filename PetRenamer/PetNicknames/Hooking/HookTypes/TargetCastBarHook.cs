using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal class TargetCastBarHook : CastBarHook 
{
    private Func<IPettableEntity?>? callGetUser = null;

    public override void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, string AddonName, uint[] textPos, Func<PetSkeleton, bool> allowedCallback, bool allowColours, bool isSoft = false)
    {
        base.Setup(services, userList, petServices, dirtyListener, AddonName, textPos, allowedCallback, allowColours, isSoft);

        SetFaulty();
    }

    public TargetCastBarHook RegisterTarget(Func<IPettableEntity?> callGetUser)
    {
        this.callGetUser = callGetUser;

        SetUnfaulty();

        return this;
    }

    protected override IPettableUser? GetUser()
    {
        IPettableEntity? entity = callGetUser?.Invoke();

        if (entity is not IPettableUser user)
        {
            return null;
        }

        return user;
    }
}
