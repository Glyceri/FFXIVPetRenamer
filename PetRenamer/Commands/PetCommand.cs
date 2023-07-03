namespace PetRenamer.Commands
{
    internal abstract class PetCommand
    {
        internal abstract void OnCommand(string command, string args);
    }
}
