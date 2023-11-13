using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking;

internal class HookHandler : RegistryBase<HookableElement, HookAttribute>
{
    protected override void OnElementCreation(HookableElement element)
    {
        PluginHandlers.Hooking.InitializeFromAttributes(element);
        element.OnInit();
    }

    public void ResetHook<T>() where T : HookableElement
    {
        foreach (HookableElement el in elements)
        {
            if (el is not T tEl) continue;
            tEl.ResetHook();
            break;
        }
    }
}
