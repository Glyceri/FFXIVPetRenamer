
namespace PetRenamer.Core.Handlers;

internal class QuitHandler
{
    public void Quit() 
    {
        PluginLink.WindowHandler.RemoveAllWindows();
        PluginLink.CommandHandler.ClearAllCommands();
        PluginLink.UpdatableHandler.ClearAllUpdatables();
    }
}

