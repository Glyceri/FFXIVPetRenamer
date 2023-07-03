using PetRenamer.Core.Handlers;

namespace PetRenamer.Utilization
{
    internal class UtilsRegistryType
    {
        protected Utils Utils => PluginLink.Utils;
        internal virtual void OnRegistered() { }
    }
}
