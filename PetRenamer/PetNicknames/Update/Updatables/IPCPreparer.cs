using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetNicknames.PetNicknames.Update.Updatables;

internal class IPCPreparer : IUpdatable
{
    public bool Enabled { get; set; } = true;

    readonly IPettableUserList UserList;
    readonly IIpcProvider IIpcProvider;

    public IPCPreparer(in IPettableUserList userList, in IIpcProvider ipcProvider)
    {
        UserList = userList;
        IIpcProvider = ipcProvider;
    }

    public void OnUpdate(IFramework framework)
    {
        if (UserList.LocalPlayer == null) return;

        IIpcProvider.Prepare();
        Enabled = false;
    }
}

