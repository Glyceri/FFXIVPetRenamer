using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class SaveHelper : IUpdatable
{
    readonly IPettableDatabase Database;
    readonly Configuration Configuration;
    readonly IPettableUserList UserList;
    readonly IPetServices PetServices;

    public bool Enabled { get; set; } = true;

    public SaveHelper(in IPetServices petServices, in Configuration configuration, in IPettableDatabase database, in IPettableUserList userList)
    {
        PetServices = petServices;
        Configuration = configuration;
        Database = database;
        UserList = userList;
    }

    public void OnUpdate(IFramework framework)
    {
        IPettableDatabaseEntry[] entries = Database.DatabaseEntries;
        int length = entries.Length;

        bool hasDirty = false;
        bool dirtyIsLocal = false;

        IPettableUser? localUser = UserList.LocalPlayer;

        for (int i = 0; i < length; i++)
        {
            IPettableDatabaseEntry entry = entries[i];
            if (!entry.IsActive) continue;
            if (!entry.IsDirty) continue;
            hasDirty = true;

            if (localUser == null) break;

            if (localUser.ContentID != entry.ContentID) continue;

            dirtyIsLocal = true;
            break;
        }

        if (!hasDirty) return;

        

        Configuration.Save();

        //TODO: Add IPC notify
        PetServices.PetLog.Log($"Dirty changes saved. [Was local: {dirtyIsLocal}]");
    }
}
