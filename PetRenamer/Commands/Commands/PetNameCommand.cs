using PetRenamer.Commands.Attributes;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

[PetCommand(PluginConstants.mainCommandAlt, "Opens the nickname window.", true, -4, PluginConstants.mainCommand)]
internal class PetNameCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<PetRenameWindow>();
}
