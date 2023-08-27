using PetRenamer.Commands.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

//[PetCommand("/minionnames", "Opens a list with all your nicknamed minions", true, "/petnames", "/petlist", "/minionlist")]
internal class PetListCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<PetListWindow>();
}
