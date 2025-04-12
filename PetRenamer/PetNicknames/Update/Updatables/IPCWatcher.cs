using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update.Interfaces;
using System.Linq;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class IPCWatcher : IUpdatable
{
    public bool Enabled { get; set; } = true;

    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPetServices PetServices;

    double counter = 0;
    const int CheckDelay = 300; // 5 minutes

    public IPCWatcher(IPettableUserList userList, IPettableDatabase database, IPetServices petServices)
    {
        UserList = userList;
        Database = database;
        PetServices = petServices;
    }

    public void OnUpdate(IFramework framework)
    {
        counter += framework.UpdateDelta.TotalSeconds;

        if (counter < CheckDelay) return;

        counter -= CheckDelay;

        Verify();
    }

    void Verify()
    {
        PetServices.PetLog.LogVerbose("Verify Database");

        foreach (IPettableDatabaseEntry entry in Database.DatabaseEntries)
        {
            if (!entry.IsActive) continue;
            if (!entry.IsIPC) continue;

            IPettableUser? user = UserList.GetUser(entry.ContentID);
            if (user != null) continue; // User exists so its fine to keep this IPC user

            PetServices.PetLog.LogVerbose($"IPCUser: {entry.Name} {entry.HomeworldName} was not found and has been removed from the database.");

            Database.RemoveEntry(entry);
        }
    }
}
