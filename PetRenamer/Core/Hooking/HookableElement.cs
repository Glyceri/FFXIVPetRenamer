using PetRenamer.Core.AutoRegistry.Interfaces;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.Hooking;

public unsafe class HookableElement : IDisposableRegistryElement
{
    public void Dispose() { OnDispose(); }
    internal virtual void OnDispose() { }
    internal virtual void OnInit() { }
    public void ResetHook()
    {
        Dispose();
        PluginHandlers.Hooking.InitializeFromAttributes(this);
        OnInit();
    }
}
