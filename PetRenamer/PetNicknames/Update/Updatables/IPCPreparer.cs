using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class IpcPreparer(IPetServices petServices, IIpcProvider ipcProvider) : IUpdatable
{
    public bool Enabled { get; private set; } 
        = true;

    public void OnUpdate(IFramework framework)
    {
        if (petServices.UserList.LocalPlayer == null)
        {
            return;
        }

        ipcProvider.Prepare();

        Enabled = false;
    }
}

