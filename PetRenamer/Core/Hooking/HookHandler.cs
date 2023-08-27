using Dalamud.Game;
using Dalamud.Utility.Signatures;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking;

internal class HookHandler : RegistryBase<HookableElement, HookAttribute>
{
    protected override void OnElementCreation(HookableElement element)
    {
        SignatureHelper.Initialise(element);
        element.OnInit();
    }

    public HookHandler()
    {
        PluginHandlers.Framework.Update += OnUpdate;
    }

    protected override void OnDipose()
    {
        PluginHandlers.Framework.Update -= OnUpdate;
    }

    protected void OnUpdate(Framework framework)
    {
        foreach(HookableElement el in elements)
            el?.OnUpdate(framework);
    }
}
