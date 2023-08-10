using Dalamud.Utility.Signatures;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking;

internal class HookHandler : RegistryBase<HookableElement, HookAttribute>
{
    protected override void OnElementCreation(HookableElement element)
    {
        SignatureHelper.Initialise(element);
        element.OnInit();
    }
}
