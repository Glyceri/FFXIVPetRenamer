using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class YokaiTextHook : QuickTextHookableElement
{
    internal override void OnQuickInit() => RegisterHook("YKWNote", 28, Allowed);

    bool Allowed(int id) => id > -1 && PluginLink.Configuration.showNamesInMinionBook && PluginLink.Configuration.displayCustomNames;
}
