using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;
using Dalamud.Plugin.Services;
using PetRenamer.Core.PettableUserSystem;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal unsafe class TargetBarHooking : QuickTextHookableElement
{
    internal override void OnQuickInit()
    {
        RegisterHook("_TargetInfoMainTarget",   10, null!,  -1, TargetUser);
        RegisterHook("_TargetInfoMainTarget",   7,  null!,  -1, TargetOfTargetUser);
        RegisterHook("_FocusTargetInfo",        10, null!,  -1, FocusTargetUser);
        RegisterHook("_TargetInfoCastBar",      4, Allowed, -1, TargetUser);
        RegisterHook("_FocusTargetInfo",        5, Allowed, -1, FocusTargetUser);
    }

    internal override void OnUpdate(IFramework framework) => OnBaseUpdate(framework, PluginLink.Configuration.displayCustomNames);

    PettableUser TargetUser() => PluginLink.PettableUserHandler.GetUser(RequestTarget()?.Address ?? nint.Zero);
    PettableUser TargetOfTargetUser() => PluginLink.PettableUserHandler.GetUser(RequestTarget()?.TargetObject?.Address ?? nint.Zero);
    PettableUser FocusTargetUser() => PluginLink.PettableUserHandler.GetUser(RequestFocusTarget()?.Address ?? nint.Zero);

    DGameObject RequestFocusTarget() => PluginHandlers.TargetManager.FocusTarget!;
    DGameObject RequestTarget() => PluginHandlers.TargetManager.SoftTarget! ?? PluginHandlers.TargetManager.Target!;

    bool Allowed(int id) => id < -1 && PluginLink.Configuration.allowCastBarPet;
}
