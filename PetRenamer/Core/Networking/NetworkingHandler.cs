using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Networking.Attributes;
using System.Reflection;
using System;

namespace PetRenamer.Core.Networking;

internal class NetworkingHandler : RegistryBase<NetworkingElement, NetworkedAttribute>
{
    public NetworkingCache NetworkingCache { get; private set; } = new NetworkingCache();

    protected override void OnElementCreation(NetworkingElement element)
    {
        Type t = element.GetType();
        PropertyInfo[] properties = t.GetProperties();
        foreach (PropertyInfo property in properties)
            if (property.PropertyType == t && property.Name == "instance")
                property.SetValue(element, element);
        element.Initialize();
    }

    protected override void OnSelfInitialize()
    {
        NetworkingCache?.Initialize();
    }

    protected override void OnDipose()
    {
        NetworkingCache?.Dispose();
    }
}
