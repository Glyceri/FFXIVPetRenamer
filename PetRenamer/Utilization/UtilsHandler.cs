using PetRenamer.Core.AutoRegistry;
using PetRenamer.Utilization.Attributes;
using System;

namespace PetRenamer.Utilization;

internal class UtilsHandler : RegistryBase<UtilsRegistryType, UtilsDeclarableAttribute>
{
    protected override void OnElementCreation(UtilsRegistryType element) => element.OnRegistered();
    protected override void OnLateElementCreation(UtilsRegistryType element) => element.OnLateRegistered();
    [Obsolete("Use the singleton now")]
    internal T Get<T>() where T : UtilsRegistryType => (GetElement(typeof(T)) as T)!;
}
