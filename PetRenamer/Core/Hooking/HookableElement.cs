using Dalamud.Game;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.Hooking;

public unsafe class HookableElement : IDisposableRegistryElement
{
    public void Dispose() { OnDispose(); }

    internal virtual void OnDispose() { }

    internal virtual void OnInit() { }
}
