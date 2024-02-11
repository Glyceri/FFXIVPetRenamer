using PetRenamer.Commands.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

[PetCommand("/petnews", "Opens the Pet News Window.", false, 1)]
internal class PetnewsCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<PetNewsWindow>();
}
