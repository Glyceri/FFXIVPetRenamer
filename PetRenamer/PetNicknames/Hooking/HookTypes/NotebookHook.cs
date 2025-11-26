using Dalamud.Game.Addon.Lifecycle;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal class NotebookHook : SimpleTextHook
{
    public override void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, string AddonName, uint[] textPos, Func<PetSkeleton, bool> allowedCallback, bool allowColours, bool isSoft = false)
    {
        base.Setup(services, userList, petServices, dirtyListener, AddonName, textPos, allowedCallback, allowColours, isSoft);

        services.AddonLifecycle.UnregisterListener(AddonEvent.PostRequestedUpdate, HandleUpdate);
        services.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, AddonName, HandleUpdate);

        SetUnfaulty();
    }

    public override void OnDispose()
    {
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostRefresh, HandleUpdate);
    }
}
