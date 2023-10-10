using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Networking.Attributes;

namespace PetRenamer.Core.Networking;

internal class NetworkingHandler : RegistryBase<NetworkingElement, NetworkedAttribute>
{
    public NetworkingCache NetworkingCache { get; private set; } = new NetworkingCache();

    protected override void OnElementCreation(NetworkingElement element) => element.Initialize();
    protected override void OnSelfInitialize() => NetworkingCache?.Initialize();
    protected override void OnDipose() => NetworkingCache?.Dispose();
}
