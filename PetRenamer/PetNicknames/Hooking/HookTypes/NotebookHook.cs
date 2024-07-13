using Dalamud.Game.Addon.Lifecycle;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal class NotebookHook : SimpleTextHook
{
    public override void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false)
    {
        base.Setup(services, userList, petServices, AddonName, textPos, allowedCallback, isSoft);
        services.AddonLifecycle.UnregisterListener(AddonEvent.PostRequestedUpdate, HandleUpdate);
        services.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, AddonName, HandleUpdate);
        SetUnfaulty();
    }

    public override void OnDispose()
    {
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostRefresh, HandleUpdate);
    }
}
