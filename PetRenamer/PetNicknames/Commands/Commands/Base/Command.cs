using PetRenamer.PetNicknames.Commands.Interfaces;
using PetRenamer.PetNicknames.KTKWindowing;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.Commands.Commands.Base;

internal abstract class Command : ICommand
{
    public abstract string Description { get; }
    public abstract bool   ShowInHelp { get; }
    public abstract string CommandCode { get; }

    protected readonly DalamudServices  DalamudServices;
    protected readonly IWindowHandler   WindowHandler;
    protected readonly KTKWindowHandler KTKWindowHandler;

    public Command(DalamudServices dalamudServices, IWindowHandler windowHandler, KTKWindowHandler ktkWindowHandler)
    {
        DalamudServices  = dalamudServices;
        WindowHandler    = windowHandler;
        KTKWindowHandler = ktkWindowHandler;

        _ = DalamudServices.CommandManager.AddHandler(CommandCode, new Dalamud.Game.Command.CommandInfo(OnCommand)
        {
            HelpMessage = Description,
            ShowInHelp  = ShowInHelp,
        });
    }

    public abstract void OnCommand(string command, string args);

    public void Dispose()
    {
        _ = DalamudServices.CommandManager.RemoveHandler(CommandCode);
    }
}
