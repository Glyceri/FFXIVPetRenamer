using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable(-10)]
internal class UserFindUpdatable : Updatable
{
    public override void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
            PettableUserUtils.instance.Solve(user);
    }
}