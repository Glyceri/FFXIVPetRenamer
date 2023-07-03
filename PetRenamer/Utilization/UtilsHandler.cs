using PetRenamer.Core.AutoRegistry;
using PetRenamer.Utilization.Attributes;

namespace PetRenamer.Utilization;

internal class UtilsHandler : RegistryBase<UtilsRegistryType, UtilsDeclarableAttribute>
{
    protected override void OnElementCreation(UtilsRegistryType element) => element.OnRegistered();
    internal T Get<T>() where T : UtilsRegistryType => (GetElement(typeof(T)) as T)!;
}
