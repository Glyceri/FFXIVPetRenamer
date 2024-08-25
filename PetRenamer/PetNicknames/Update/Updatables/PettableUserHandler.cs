using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal unsafe class PettableUserHandler : IUpdatable
{
    public bool Enabled { get; set; } = true;

    readonly IPettableUserList UserList;

    public PettableUserHandler(IPettableUserList userList)
    {
        UserList = userList;
    }

    public void OnUpdate(IFramework framework)
    {
        for (int i = 0; i < PettableUsers.PettableUserList.PettableUserArraySize; i++)
        {
            IPettableUser? user = UserList.PettableUsers[i];
            if (user == null) continue;

            user.Update();
        }
    }
}
