using PetRenamer.PetNicknames.KTKWindowing.Addons;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Enums;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.KTKWindowing;

internal class KTKWindowHandler : IDisposable
{
    private readonly IPetServices         PetServices;
    private readonly DalamudServices      DalamudServices;
    private readonly IPettableUserList    UserList;
    private readonly IPettableDatabase    Database;
    private readonly ILegacyDatabase      LegacyDatabase;
    private readonly PettableDirtyHandler DirtyHandler;

    private readonly List<KTKAddon> KTKWindows = [];

    private readonly PetRenameAddon   PetRenameKTKWindow;
    private readonly PetSettingsAddon PetSettingsAddon;
    private readonly PetListAddon     PetListAddon;
    private readonly KofiAddon        KofiAddon;
    private readonly PetDevAddon      PetDevAddon;

    public KTKWindowHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, ILegacyDatabase legacyDatabase, PettableDirtyHandler dirtyHandler)
    {
        PetServices     = petServices;
        DalamudServices = dalamudServices;
        UserList        = userList;
        Database        = database;
        LegacyDatabase  = legacyDatabase;
        DirtyHandler    = dirtyHandler;

        RegisterWindow(PetRenameKTKWindow = new PetRenameAddon(this, DalamudServices, PetServices, UserList, Database, DirtyHandler));
        RegisterWindow(PetSettingsAddon   = new PetSettingsAddon(this, DalamudServices, PetServices, UserList, Database, DirtyHandler));
        RegisterWindow(PetListAddon       = new PetListAddon(this, DalamudServices, PetServices, UserList, Database, DirtyHandler));
        RegisterWindow(KofiAddon          = new KofiAddon(this, DalamudServices, PetServices, UserList, Database, DirtyHandler));
        RegisterWindow(PetDevAddon        = new PetDevAddon(this, DalamudServices, PetServices, UserList, Database, DirtyHandler));

        PetRenameKTKWindow.Open();
        //PetSettingsAddon.Open();

        dirtyHandler.DirtyPetMode(PetWindowMode.Minion);
    }

    private void RegisterWindow(KTKAddon window)
    {
        KTKWindows.Add(window);
    }

    public bool IsOpen<T>() where T : KTKAddon
    {
        foreach (KTKAddon addon in KTKWindows)
        {
            if (addon is not T)
            {
                continue;
            }

            return addon.IsOpen;
        }

        return false;
    }    

    public KTKAddon? GetAddon<T>() where T : KTKAddon
    {
        foreach (KTKAddon addon in KTKWindows)
        {
            if (addon is not T)
            {
                continue;
            }

            return addon;
        }

        return null;
    }

    public void Open<T>() where T : KTKAddon
    {
        foreach (KTKAddon addon in KTKWindows)
        {
            if (addon is not T)
            {
                continue;
            }

            addon.Open();
        }

        DirtyHandler.DirtyWindow();
    }

    public void Close<T>() where T : KTKAddon
    {
        foreach (KTKAddon addon in KTKWindows)
        {
            if (addon is not T)
            {
                continue;
            }

            addon.Close();
        }

        DirtyHandler.DirtyWindow();
    }

    public void Toggle<T>() where T : KTKAddon
    {
        foreach (KTKAddon addon in KTKWindows)
        {
            if (addon is not T)
            {
                continue;
            }

            addon.Toggle();
        }

        DirtyHandler.DirtyWindow();
    }

    public void Dispose()
    {
        foreach (KTKAddon window in KTKWindows)
        {
            window.Dispose(); 
        }

        KTKWindows.Clear();
    }
}
