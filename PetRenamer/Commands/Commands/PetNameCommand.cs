using PetRenamer.Commands.Attributes;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

[PetCommand(PluginConstants.mainCommand, "Opens the nickname window.", true, PluginConstants.mainCommandAlt)]
internal class PetNameCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<PetRenameWindow>();
}
