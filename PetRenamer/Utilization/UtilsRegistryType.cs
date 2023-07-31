using PetRenamer.Core.AutoRegistry.Interfaces;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Utilization;

internal class UtilsRegistryType : IRegistryElement
{
    protected UtilsHandler Utils => PluginLink.Utils;
    internal virtual void OnRegistered() { }
    internal virtual void OnLateRegistered() { }
}
