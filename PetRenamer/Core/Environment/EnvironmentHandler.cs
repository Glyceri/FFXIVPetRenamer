using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Environment.Attributes;

namespace PetRenamer.Core.Environment;

internal class EnvironmentHandler : RegistryBase<EnvironmentElement, EnvironmentAttribute> 
{
    protected override void OnElementCreation(EnvironmentElement element) => element.Initialize();
}
