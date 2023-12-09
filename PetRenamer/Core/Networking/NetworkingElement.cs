using PetRenamer.Core.Attributes;
using PetRenamer.Core.AutoRegistry.Interfaces;
using PetRenamer.Core.Handlers;
using System.Net.Http;

namespace PetRenamer.Core.Networking;

public class NetworkingElement : IDisposableRegistryElement, IInitializable
{
    public void Dispose() => OnDispose();
    public void Initialize() => OnInitialize();
    protected readonly HttpClient client = new HttpClient();
    internal NetworkingCache Cache
    {
        get => PluginLink.NetworkingHandler.NetworkingCache;
    } 

    internal virtual void OnDispose() { }
    internal virtual void OnInitialize() { }
}
