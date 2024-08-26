using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal unsafe class PettableUserHandler : IUpdatable
{
    public bool Enabled { get; set; } = true;

    bool isDirty = false;

    readonly IPettableUserList UserList;
    readonly IPetServices PetServices;
    readonly IIslandHook IslandHook;
    readonly IPettableDirtyListener DirtyListener;
    readonly IPettableDatabase Database;

    public PettableUserHandler(IPettableUserList userList, IPetServices petServices, IIslandHook islandHook, IPettableDirtyListener dirtyListener, IPettableDatabase database)
    {
        UserList = userList;
        PetServices = petServices;
        IslandHook = islandHook;
        DirtyListener = dirtyListener;
        Database = database;

        DirtyListener.RegisterOnClearEntry(OnDirty);
        DirtyListener.RegisterOnDirtyDatabase(OnDirty);
        DirtyListener.RegisterOnDirtyEntry(OnDirty);
        DirtyListener.RegisterOnDirtyName(OnDirty);
    }

    public void OnUpdate(IFramework framework)
    {
        for (int i = 0; i < PettableUsers.PettableUserList.PettableUserArraySize; i++)
        {
            IPettableUser? user = UserList.PettableUsers[i];
            if (user == null) continue;

            user.Update();
        }

        HandleIsland();
    }

    void HandleIsland()
    {
        if (PetServices.Configuration.showOnIslandPets)
        {
            IslandHook.Update();

            if (!IslandHook.IslandStatusChanged && !isDirty) return;

            ClearDirty();

            if (IslandHook.IsOnIsland) HandleOnIsland();
            else HandleNotOnIsland();
        }
    }

    void OnDirty(IPettableDatabase database) => SetDirty();
    void OnDirty(INamesDatabase database) => SetDirty();
    void OnDirty(IPettableDatabaseEntry entry) => SetDirty();
    void SetDirty() => isDirty = true;
    void ClearDirty() => isDirty = false;

    void HandleNotOnIsland() => ClearIslandUser();
    void HandleOnIsland()
    {
        if (IslandHook.VisitingFor == null || IslandHook.VisitingHomeworld == null) return;

        IPettableDatabaseEntry? entry = Database.GetEntry(IslandHook.VisitingFor, (ushort)IslandHook.VisitingHomeworld, false);
        if (entry == null) return;

        CreateIslandUser(entry);
    }

    void ClearIslandUser()
    {
        UserList.PettableUsers[PettableUserList.IslandIndex]?.Dispose(Database);
        UserList.PettableUsers[PettableUserList.IslandIndex] = null;
    }

    void CreateIslandUser(IPettableDatabaseEntry entry)
    {
        ClearIslandUser();
        UserList.PettableUsers[PettableUserList.IslandIndex] = new PettableIslandUser(PetServices, entry);
    }
}
