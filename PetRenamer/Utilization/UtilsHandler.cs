using PetRenamer.Core.AutoRegistry;
using PetRenamer.Utilization.Attributes;

namespace PetRenamer.Utilization;

internal class UtilsHandler : RegistryBase<UtilsRegistryType, UtilsDeclarableAttribute>
{
    protected override void OnElementCreation(UtilsRegistryType element) => element.OnRegistered();
    protected override void OnLateElementCreation(UtilsRegistryType element) => element.OnLateRegistered();
}
