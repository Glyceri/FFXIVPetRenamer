using PetRenamer.Commands.Attributes;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands
{
    [PetCommand(PluginConstants.mainCommand, "Opens the Pet Nickname window.", true)]
    internal class PetNameCommand : PetCommand
    {
        internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<MainWindow>();
    }
}
