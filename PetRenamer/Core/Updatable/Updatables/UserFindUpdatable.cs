using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable(-10)]
internal class UserFindUpdatable : Updatable
{
    public override void Update(Framework frameWork)
    {
        PluginLink.PettableUserHandler.LoopThroughUsers((user) => PettableUserUtils.instance.DissectUser(user));
    }
}