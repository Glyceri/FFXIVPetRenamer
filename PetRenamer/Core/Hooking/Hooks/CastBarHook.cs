using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class CastBarHook : QuickTextHookableElement
{
    internal override void OnQuickInit() => RegisterHook("_CastBar", 4, -1);

    internal override void OnUpdate(Framework framework) => 
        OnBaseUpdate(framework, PluginLink.Configuration.displayCustomNames && PluginLink.Configuration.allowCastBar);
}
