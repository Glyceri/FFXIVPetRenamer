using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class CastBarHook : QuickTextHookableElement
{
    internal override void OnQuickInit() => RegisterHook("_CastBar", 4, Allowed);

    bool Allowed(int id) => id < -1 && PluginLink.Configuration.allowCastBarPet && PluginLink.Configuration.displayCustomNames;
}
