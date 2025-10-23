using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class IPCWatcher : IUpdatable
{
    // This system doesn't work as I intended it to yet... this is extremely problematic as it causes double references and stale references and teehee heehee's all around
    public bool Enabled { get; set; } 
        = false;  

    private readonly IPettableUserList UserList;
    private readonly IPettableDatabase Database;
    private readonly IPetServices      PetServices;

    private double counter = 0;

    private const int CheckDelay = 300; // 5 minutes

    public IPCWatcher(IPettableUserList userList, IPettableDatabase database, IPetServices petServices)
    {
        UserList    = userList;
        Database    = database;
        PetServices = petServices;
    }

    public void OnUpdate(IFramework framework)
    {
        counter += framework.UpdateDelta.TotalSeconds;

        if (counter < CheckDelay)
        {
            return;
        }

        counter -= CheckDelay;

        Verify();
    }

    void Verify()
    {
        PetServices.PetLog.LogVerbose("Verify Database");

        foreach (IPettableDatabaseEntry entry in Database.DatabaseEntries)
        {
            if (!entry.IsActive)
            {
                continue;
            }

            if (!entry.IsIPC)
            {
                continue;
            }

            IPettableUser? user = UserList.GetUserFromContentID(entry.ContentID);

            // User exists so its fine to keep this IPC user
            if (user != null)
            {
                continue;
            }

            PetServices.PetLog.LogVerbose($"IPCUser: {entry.Name} {entry.HomeworldName} was not found and has been removed from the database.");

            Database.RemoveEntry(entry);
        }
    }
}
