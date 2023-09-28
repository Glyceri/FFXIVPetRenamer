using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;
using Dalamud.Plugin.Services;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Pet;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal unsafe class TargetBarHooking : QuickTextHookableElement
{
    internal override void OnQuickInit()
    {
        RegisterHook("_TargetInfoMainTarget",   10, null!, -1, () => PluginLink.PettableUserHandler.GetUser(RequestTarget()?.Address ?? nint.Zero));                 // TARGET
        RegisterHook("_TargetInfoMainTarget",   7,  null!, -1, () => PluginLink.PettableUserHandler.GetUser(RequestTarget()?.TargetObject?.Address ?? nint.Zero));   // TARGET OF TARGET
        RegisterHook("_FocusTargetInfo",        10, null!, -1, () => PluginLink.PettableUserHandler.GetUser(RequestFocusTarget()?.Address ?? nint.Zero));            // FOCUS TARGET
    }

    internal override void OnUpdate(IFramework framework) => OnBaseUpdate(framework, PluginLink.Configuration.displayCustomNames);

    DGameObject RequestFocusTarget() => PluginHandlers.TargetManager.FocusTarget!;
    DGameObject RequestTarget()
    {
        DGameObject target = PluginHandlers.TargetManager.SoftTarget!;
        return target ??= PluginHandlers.TargetManager.Target!;
    }
}
