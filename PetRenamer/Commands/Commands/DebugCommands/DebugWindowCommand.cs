using PetRenamer.Commands.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Commands.Commands;

#if DEBUG
[PetCommand("/petdebug", "Opens the PetRenamer Debug Window", true)]
#endif
internal class DebugWindowCommand : PetCommand
{
    internal override void OnCommand(string command, string args) => PluginLink.WindowHandler.ToggleWindow<DebugWindow>();
}
