using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Types;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class CastBarHook : QuickTextHookableElement
{
    internal override void OnQuickInit() 
    { 
        RegisterHook("_CastBar", 4, -1); 
        RegisterHook("_TargetInfoCastBar", 4, -1, GetThisInstead);
        RegisterHook("_FocusTargetInfo", 5, -1, GetThisInsteadFocus);
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

    internal override void OnUpdate(Framework framework) => 
        OnBaseUpdate(framework, PluginLink.Configuration.displayCustomNames && PluginLink.Configuration.allowCastBar);
}
