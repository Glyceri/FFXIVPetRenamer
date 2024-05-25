using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable(-10)]
internal class UserFindUpdatable : Updatable
{
    const double maxTimer = 1;
    double timer = maxTimer;

    List<PettableUser> users = new List<PettableUser>();

    int lastUserCount = -1;

    public unsafe override void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        for (int i = PluginLink.PettableUserHandler.Users.Count - 1; i >= 0; i--)
        {
            PettableUser user = PluginLink.PettableUserHandler.Users[i];
            if (user is PettableIPCUser ipcUser)
                if (ipcUser.SerializableUser.AccurateIPCCount() == 0)
                    ipcUser.Destroy();
            if (user.DeathsMark) PluginLink.PettableUserHandler.DeclareUser(user.SerializableUser, PettableUserSystem.Enums.UserDeclareType.Remove);
        }

        int curUserCount = PluginLink.PettableUserHandler.Users.Count;

        if(lastUserCount != curUserCount)
        {
            lastUserCount = curUserCount;
            timer = maxTimer;
        }

        if ((timer += frameWork.UpdateDelta.TotalSeconds) >= maxTimer)
        {
            timer -= maxTimer;
            int uCount = users.Count;
            for (int i = 0; i < uCount; i++)
                users[i].Reset();
            users.Clear();
            uCount = PluginLink.PettableUserHandler.Users.Count;
            for (int i = 0; i < uCount; i++)
            {
                PettableUser user = PluginLink.PettableUserHandler.Users[i];
                BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(user.UserName, true, (short)user.Homeworld);
                if (bChara == null) continue;
                users.Add(user);
            }
        }

        IpcProvider.PetNicknameDict.Clear();

        int userCount = users.Count;
        for (int i = 0; i < userCount; i++)
            PettableUserUtils.instance.Solve(users[i]);
    }
}