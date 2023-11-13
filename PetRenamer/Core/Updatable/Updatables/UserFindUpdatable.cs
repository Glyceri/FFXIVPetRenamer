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
            if (user is PettableIPCUser ipcUser)
                if (ipcUser.SerializableUser.AccurateIPCCount() == 0)
                    ipcUser.Destroy();
            if (user.DeathsMark) PluginLink.PettableUserHandler.DeclareUser(user.SerializableUser, PettableUserSystem.Enums.UserDeclareType.Remove);
        }

        int userCount = PluginLink.PettableUserHandler.Users.Count;
        for (int i = 0; i < userCount; i++)
            PettableUserUtils.instance.Solve(PluginLink.PettableUserHandler.Users[i]);
    }
}