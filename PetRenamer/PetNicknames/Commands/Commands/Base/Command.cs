using Dalamud.Game.Command;
using PetRenamer.PetNicknames.Commands.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Interfaces;

namespace PetRenamer.PetNicknames.Commands.Commands.Base;

internal abstract class Command : ICommand
{
    protected abstract string   Description { get; }
    protected abstract bool     ShowInHelp  { get; }
    protected abstract string   CommandCode { get; }
    protected abstract string[] Aliases     { get; }
    
    protected readonly DalamudServices DalamudServices;
    protected readonly IWindowHandler  WindowHandler;

    protected Command(DalamudServices dalamudServices, IWindowHandler windowHandler)
    {
        DalamudServices = dalamudServices;
        WindowHandler   = windowHandler;

        RegisterCommand(CommandCode, Description, ShowInHelp);
        
        foreach (string alias in Aliases)
        {
            RegisterCommand(alias, Description);
        }
    }
    
    private void RegisterCommand(string command, string description, bool showInHelp = false)
    {
        CommandInfo commandInfo = new CommandInfo(OnCommand)
        {
            HelpMessage = description,
            ShowInHelp  = showInHelp,
        };
        
        if (DalamudServices.CommandManager.AddHandler(command, commandInfo))
        {
            return;
        }
        
        DalamudServices.PluginLog.Warning($"Failed to register the command: [{command}].");
    }

    public void Dispose()
    {
        DalamudServices.CommandManager.RemoveHandler(CommandCode);
    }
    
    protected abstract void OnCommand(string command, string args);
}
