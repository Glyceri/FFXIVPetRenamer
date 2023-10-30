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
        for (int i = PluginLink.PettableUserHandler.Users.Count - 1; i >= 0; i--)
        {
            PettableUser user = PluginLink.PettableUserHandler.Users[i];
            if (user is not PettableIPCUser ipcUser) continue;
            if (ipcUser.SerializableUser.AccurateIPCCount() != 0) continue;
            if (ipcUser.DeathsMark) PluginLink.PettableUserHandler.Users.Remove(user);
            else ipcUser.Destroy();
        }

        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
            PettableUserUtils.instance.Solve(user);
    }
}