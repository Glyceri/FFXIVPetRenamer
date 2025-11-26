using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetNicknames.PetNicknames.Update.Updatables;

internal class IPCPreparer : IUpdatable
{
    public bool Enabled { get; set; } 
        = true;

    private readonly IPettableUserList UserList;
    private readonly IIpcProvider      IIpcProvider;

    public IPCPreparer(IPettableUserList userList, IIpcProvider ipcProvider)
    {
        UserList     = userList;
        IIpcProvider = ipcProvider;
    }

    public void OnUpdate(IFramework framework)
    {
        if (UserList.LocalPlayer == null)
        {
            return;
        }

        IIpcProvider.Prepare();

        Enabled = false;
    }
}

