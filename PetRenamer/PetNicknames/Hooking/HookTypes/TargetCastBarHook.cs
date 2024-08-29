using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal class TargetCastBarHook : CastBarHook 
{
    Func<IPettableUser?>? callGetUser = null;

    public override void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false)
    {
        base.Setup(services, userList, petServices, dirtyListener, AddonName, textPos, allowedCallback, isSoft);
        SetFaulty();
    }

    public void RegsterTarget(Func<IPettableUser?> callGetUser)
    {
        this.callGetUser = callGetUser;
        SetUnfaulty();
    }

    protected override IPettableUser? GetUser() => callGetUser?.Invoke();  
}
