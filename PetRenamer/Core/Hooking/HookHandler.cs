using Dalamud.Game;
using Dalamud.Utility.Signatures;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking;

internal class HookHandler : RegistryBase<HookableElement, HookAttribute>
{
    public unsafe HookHandler()
    {
        PluginHandlers.Framework.Update += MainUpdate;
    }

    ~HookHandler()
    {
        PluginHandlers.Framework.Update -= MainUpdate;
    }

    protected override void OnElementCreation(HookableElement element)
    {
        SignatureHelper.Initialise(element);
        element.OnInit();
    }

    public void MainUpdate(Framework framework)
    {
        if (PluginHandlers.ClientState.LocalPlayer == null) return;
        foreach(HookableElement e in elements)
            e.OnUpdate(framework);
    }
}
