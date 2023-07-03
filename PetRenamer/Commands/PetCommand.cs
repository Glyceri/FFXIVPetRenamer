using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Commands
{
    internal abstract class PetCommand : IRegistryElement
    {
        internal abstract void OnCommand(string command, string args);
    }
}
