using PetRenamer.PetNicknames.Commands.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.Commands.Commands.Base;

internal abstract class Command : ICommand
{
    public abstract string Description { get; }
    public abstract bool ShowInHelp { get; }
    public abstract string CommandCode { get; }

    public abstract void OnCommand(string command, string args);

    protected readonly DalamudServices DalamudServices;
    protected readonly IWindowHandler WindowHandler;

    public Command(in DalamudServices dalamudServices, in IWindowHandler windowHandler)
    {
        DalamudServices = dalamudServices;
        WindowHandler = windowHandler;

        DalamudServices.CommandManager.AddHandler(CommandCode, new Dalamud.Game.Command.CommandInfo(OnCommand)
        {
            HelpMessage = Description,
            ShowInHelp = ShowInHelp,
        });
    }

    public void Dispose()
    {
        DalamudServices.CommandManager.RemoveHandler(CommandCode);
    }
}
