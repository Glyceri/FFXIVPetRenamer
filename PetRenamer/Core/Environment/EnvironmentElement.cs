using PetRenamer.Core.Attributes;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.Environment;

internal abstract class EnvironmentElement : IDisposableRegistryElement, IInitializable
{
    internal abstract void OnDispose();
    internal abstract void OnInitialize();

    public void Dispose() => OnDispose();
    public void Initialize() => OnInitialize();
}
