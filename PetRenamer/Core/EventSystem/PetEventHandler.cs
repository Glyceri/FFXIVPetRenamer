using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.EventSystem.EventAttribute;
using System.Reflection;
using System;

namespace PetRenamer.Core.EventSystem;

public class PetEventHandler : RegistryBase<EventElement, EventObjectAttribute>
{
    protected override void OnElementCreation(EventElement element)
    {
        Type t = element.GetType();
        PropertyInfo[] properties = t.GetProperties();
        foreach (PropertyInfo property in properties)
            if (property.PropertyType == t && property.Name == "instance")
                property.SetValue(element, element);
    }
}
