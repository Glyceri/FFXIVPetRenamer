using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class PettableUserHandler : IUpdatable
{
    private bool isDirty;
    
    private readonly IPetServices      PetServices;
    private readonly IIslandHook       IslandHook;
    private readonly IPettableDatabase Database;

    public PettableUserHandler(IPetServices petServices, IIslandHook islandHook, IPettableDatabase database)
    {
        PetServices     = petServices;
        IslandHook      = islandHook;
        Database        = database;

        PetServices.DirtyListener.RegisterOnClearEntry(OnDirty);
        PetServices.DirtyListener.RegisterOnDirtyDatabase(OnDirty);
        PetServices.DirtyListener.RegisterOnDirtyEntry(OnDirty);
        PetServices.DirtyListener.RegisterOnDirtyName(OnDirty);
    }

    public bool Enabled 
        => true;
    
    public void OnUpdate(IFramework framework)
    {
        foreach (IPettableUser? user in PetServices.UserList)
        {
            user?.Update();
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
        PetServices.UserList[IUserList.IslandIndex]?.Dispose(Database);
        PetServices.UserList[IUserList.IslandIndex] = null;
    }

    private void CreateIslandUser(IPettableDatabaseEntry entry)
    {
        ClearIslandUser();

        PetServices.UserList[IUserList.IslandIndex] = new PettableIslandUser(PetServices, entry);
    }
}
