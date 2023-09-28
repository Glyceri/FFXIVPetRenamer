using Dalamud.Plugin.Services;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking;

internal class HookHandler : RegistryBase<HookableElement, HookAttribute>
{
    public HookHandler() => PluginHandlers.Framework.Update += OnUpdate;
    protected override void OnDipose() => PluginHandlers.Framework.Update -= OnUpdate;

    protected override void OnElementCreation(HookableElement element)
    {
        PluginHandlers.Hooking.InitializeFromAttributes(element);
        element.OnInit();
    }

    protected void OnUpdate(IFramework framework)
    {
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        foreach (HookableElement el in elements)
            el?.OnUpdate(framework);
    }
}
