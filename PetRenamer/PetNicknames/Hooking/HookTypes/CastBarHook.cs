﻿using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal class CastBarHook : SimpleTextHook
{
    public override void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool allowColours, bool isSoft = false)
    {
        base.Setup(services, userList, petServices, dirtyListener, AddonName, textPos, allowedCallback, allowColours, isSoft);
        SetUnfaulty();
    }

    protected override IPetSheetData? GetPetData(string _, in IPettableUser user)
    {
        user.RefreshCast();
        return PetServices.PetSheets.GetPetFromAction(user.CurrentCastID, in user, IsSoft);
    }
}
