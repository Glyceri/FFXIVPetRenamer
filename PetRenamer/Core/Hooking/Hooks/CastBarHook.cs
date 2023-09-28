using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class CastBarHook : QuickTextHookableElement
{
    internal override void OnQuickInit() 
    { 
        RegisterHook("_CastBar", 4, Allowed, -1); 
        RegisterHook("_TargetInfoCastBar", 4, Allowed, -1, GetThisInstead);
        RegisterHook("_FocusTargetInfo", 5, Allowed, -1, GetThisInsteadFocus);
    }

    bool Allowed(int id)
    {
        if (id <= -2 && !PluginLink.Configuration.allowCastBarPet) return false;
        return true;
    }

    PettableUser GetThisInsteadFocus() => GetFromTarget(PluginHandlers.TargetManager.FocusTarget);

    PettableUser GetThisInstead()
    {
        GameObject? target = PluginHandlers.TargetManager.SoftTarget;
        target ??= PluginHandlers.TargetManager.Target;
        return GetFromTarget(target);
    }

    PettableUser GetFromTarget(GameObject? target)
    {
        if (target == null) return null!;

        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
            if (user.UserName == target.Name.ToString())
                return user;

        return null!;
    }

    internal override void OnUpdate(IFramework framework) => 
        OnBaseUpdate(framework, PluginLink.Configuration.displayCustomNames && PluginLink.Configuration.allowCastBarPet);
}
