using PetRenamer.Commands.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

[PetCommand("/petnames", "Opens a list with all your nicknames.", true, "/minionnames", "/petlist", "/minionlist")]
internal class PetListCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<NewPetListWindow>();
}
