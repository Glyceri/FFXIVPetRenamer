using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPluginWatcher : IDisposable
{
    /// <summary>
    /// Register a callback that gets called when a plugin has changed.
    /// </summary>
    /// <param name="callback">The callback that should get called.</param>
    void RegisterListener(Action<string[]> callback);

    /// <summary>
    /// Deregister a callback that gets called when a plugin has changed.
    /// </summary>
    /// <param name="callback">The callback that should get called.</param>
    void DeregisterListener(Action<string[]> callback);
}
