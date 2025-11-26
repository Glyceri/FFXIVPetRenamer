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
    public bool Enabled { get; set; } 
        = true;

    private bool isDirty = false;

    private readonly IPettableUserList      UserList;
    private readonly IPetServices           PetServices;
    private readonly IIslandHook            IslandHook;
    private readonly IPettableDirtyListener DirtyListener;
    private readonly IPettableDatabase      Database;

    public PettableUserHandler(IPettableUserList userList, IPetServices petServices, IIslandHook islandHook, IPettableDirtyListener dirtyListener, IPettableDatabase database)
    {
        UserList        = userList;
        PetServices     = petServices;
        IslandHook      = islandHook;
        DirtyListener   = dirtyListener;
        Database        = database;

        DirtyListener.RegisterOnClearEntry(OnDirty);
        DirtyListener.RegisterOnDirtyDatabase(OnDirty);
        DirtyListener.RegisterOnDirtyEntry(OnDirty);
        DirtyListener.RegisterOnDirtyName(OnDirty);
    }

    public void OnUpdate(IFramework framework)
    {
        for (int i = 0; i < PettableUserList.PettableUserArraySize; i++)
        {
            IPettableUser? user = UserList.PettableUsers[i];

            if (user == null)
            {
                continue;
            }

            user.Update();
        }

        HandleIsland();
    }

    private void HandleIsland()
    {
        if (!PetServices.Configuration.showOnIslandPets)
        {
            return;
        }

        IslandHook.Update();

        if (!IslandHook.IslandStatusChanged && !isDirty)
        {
            return;
        }

        ClearDirty();

        if (IslandHook.IsOnIsland)
        {
            HandleOnIsland();
        }
        else
        {
            HandleNotOnIsland();
        }
    }

    private void OnDirty(IPettableDatabase database)    
        => SetDirty();

    private void OnDirty(INamesDatabase database)       
        => SetDirty();

    private void OnDirty(IPettableDatabaseEntry entry)  
        => SetDirty();

    private void SetDirty()     
        => isDirty = true;

    private void ClearDirty()   
        => isDirty = false;

    private void HandleNotOnIsland() 
        => ClearIslandUser();

    private void HandleOnIsland()
    {
        if (IslandHook.VisitingFor == null || IslandHook.VisitingHomeworld == null)
        {
            return;
        }

        IPettableDatabaseEntry? entry = Database.GetEntry(IslandHook.VisitingFor, (ushort)IslandHook.VisitingHomeworld, false);

        if (entry == null)
        {
            return;
        }

        CreateIslandUser(entry);
    }

    private void ClearIslandUser()
    {
        UserList.PettableUsers[PettableUserList.IslandIndex]?.Dispose(Database);
        UserList.PettableUsers[PettableUserList.IslandIndex] = null;
    }

    private void CreateIslandUser(IPettableDatabaseEntry entry)
    {
        ClearIslandUser();

        UserList.PettableUsers[PettableUserList.IslandIndex] = new PettableIslandUser(PetServices, entry);
    }
}
