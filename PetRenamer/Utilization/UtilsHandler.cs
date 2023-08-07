using PetRenamer.Core.AutoRegistry;
using PetRenamer.Utilization.Attributes;
using System;
using System.Reflection;

namespace PetRenamer.Utilization;

internal class UtilsHandler : RegistryBase<UtilsRegistryType, UtilsDeclarableAttribute>
{
    protected override void OnElementCreation(UtilsRegistryType element) 
    {
        Type t = element.GetType();
        PropertyInfo[] properties = t.GetProperties();
        foreach (PropertyInfo property in properties)
            if (property.PropertyType == t && property.Name == "instance")
                property.SetValue(element, element);
        element.OnRegistered(); 
    }
    protected override void OnLateElementCreation(UtilsRegistryType element) => element.OnLateRegistered();
    [Obsolete("Use the singleton now")]
    internal T Get<T>() where T : UtilsRegistryType => (GetElement(typeof(T)) as T)!;
}
