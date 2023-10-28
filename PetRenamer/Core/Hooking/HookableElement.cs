using Dalamud.Plugin.Services;
using PetRenamer.Core.AutoRegistry.Interfaces;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.Hooking;

public unsafe class HookableElement : IDisposableRegistryElement
{
    public void Dispose() { OnDispose(); }
    internal virtual void OnDispose() { }
    internal virtual void OnInit() { }
    internal virtual void OnUpdate(IFramework framework) { }
    public void ResetHook()
    {
        Dispose();
        PluginHandlers.Hooking.InitializeFromAttributes(this);
        OnInit();
    }
}
