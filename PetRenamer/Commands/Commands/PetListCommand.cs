using PetRenamer.Commands.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

[PetCommand("/petlist", "Opens a list with all your nicknames.", true, "/minionnames", "/petnames", "/minionlist")]
internal class PetListCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<NewPetListWindow>();
}
