using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class TooltipHook : QuickTextHookableElement
{
    internal override void OnQuickInit()
    {
        RegisterHook("ActionDetail", 5, -1);
        RegisterHook("Tooltip", 2, 3);
    }

    internal override void OnUpdate(Framework framework) =>
        OnBaseUpdate(framework, PluginLink.Configuration.displayCustomNames && PluginLink.Configuration.allowTooltips);
}
