using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class SaveHelper : IUpdatable
{
    readonly DalamudServices DalamudServices;
    readonly IPettableDatabase Database;
    readonly Configuration Configuration;
    readonly IPettableUserList UserList;
    readonly IIpcProvider IpcProvider;

    public bool Enabled { get; set; } = true;

    IPettableUser? lastLocalUser;

    public SaveHelper(in DalamudServices dalamudServices, in Configuration configuration, in IPettableDatabase database, in IPettableUserList userList, in IIpcProvider ipcProvider)
    {
        DalamudServices = dalamudServices;
        Configuration = configuration;
        Database = database;
        UserList = userList;
        IpcProvider = ipcProvider;
    }

    public unsafe void OnUpdate(IFramework framework)
    {
        if (Database.IsDirty)
        {
            Configuration.Save();
            Database.NotifySeenDirty();
        }

        IPettableDatabaseEntry[] entries = Database.DatabaseEntries;
        int length = entries.Length;

        bool hasDirty = false;
        bool hasClear = false;
        bool dirtyContainsLocal = false;

        IPettableUser? localUser = UserList.LocalPlayer;
        bool hasLocalUser = true;

        if (localUser != null && lastLocalUser != localUser)
        {
            lastLocalUser = localUser;
            IpcProvider.NotifyDataChanged();
        }

        if (localUser == null)
        {
            hasLocalUser = false;
            IpcProvider.ClearCachedData();
        }

        for (int i = 0; i < length; i++)
        {
            IPettableDatabaseEntry entry = entries[i];

            if (entry.IsDirty || entry.IsDirtyForUI)
            {
                entry.NotifySeenDirty();
                hasDirty = true;
            }

            if (entry.IsCleared)
            {
                entry.NotifySeenCleared();

                if (!entry.IsIPC)
                {
                    hasClear = true;
                }
            }

            if (!hasLocalUser) continue;

            if (localUser!.ContentID != entry.ContentID) continue;

            dirtyContainsLocal = true;
        }

        if (!hasDirty && !hasClear) return;
        Configuration.Save();

        if (!dirtyContainsLocal) return;
        IpcProvider.NotifyDataChanged();
    }
}
