using PetRenamer.Commands.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

[PetCommand("/minionhelp", "Opens the help window.", true, "/pethelp")]
internal class PetHelpCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<PetHelpWindow>();
}
