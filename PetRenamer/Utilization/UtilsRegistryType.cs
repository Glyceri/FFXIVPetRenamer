using PetRenamer.Core.AutoRegistry.Interfaces;
using PetRenamer.Core.Handlers;
using System;

namespace PetRenamer.Utilization;

internal class UtilsRegistryType : IRegistryElement, IDisposable
{
    protected UtilsHandler Utils => PluginLink.Utils;

    internal virtual void OnRegistered() { }
    internal virtual void OnLateRegistered() { }

    internal virtual void Dispose() { }

    void IDisposable.Dispose() => Dispose();
}
