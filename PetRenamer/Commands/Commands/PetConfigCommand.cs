using PetRenamer.Commands.Attributes;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

[PetCommand(PluginConstants.petConfigCommandAlt, "Opens the configuration Window.", true, PluginConstants.petConfigCommand, "/petsettings", "/minionsettings")]
internal class PetConfigCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<ConfigWindow>();
}
