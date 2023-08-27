using PetRenamer.Commands.Attributes;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

//[PetCommand(PluginConstants.petConfigCommand, "Opens the Minion Nickname configuration Window.", true, PluginConstants.petConfigCommandAlt)]
internal class PetConfigCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<ConfigWindow>();
}
