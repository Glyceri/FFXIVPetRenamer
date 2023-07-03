using PetRenamer.Commands.Attributes;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands
{
    [PetCommand(PluginConstants.petConfigCommand, "Opens the Pet Nickname configuration Window.", true)]
    internal class PetConfigCommand : PetCommand
    {
        internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<ConfigWindow>();
    }
}
