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

    private readonly PetRenameAddon PetRenameKTKWindow;

    public KTKWindowHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, ILegacyDatabase legacyDatabase, PettableDirtyHandler dirtyHandler)
    {
        PetServices     = petServices;
        DalamudServices = dalamudServices;
        UserList        = userList;
        Database        = database;
        LegacyDatabase  = legacyDatabase;
        DirtyHandler    = dirtyHandler;

        RegisterWindow(PetRenameKTKWindow = new PetRenameAddon(this, DalamudServices, PetServices, UserList, Database, DirtyHandler));
        RegisterWindow(new PetSettingsAddon(this, DalamudServices, PetServices, UserList, Database, DirtyHandler));

        PetRenameKTKWindow.Open();

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

    public void Open<T>() where T : KTKAddon
    {
        foreach (KTKAddon addon in KTKWindows)
        {
            if (addon is not T)
            {
                continue;
            }

            _ = DalamudServices.Framework.RunOnFrameworkThread(addon.Open);
        }
    }

    public void Close<T>() where T : KTKAddon
    {
        foreach (KTKAddon addon in KTKWindows)
        {
            if (addon is not T)
            {
                continue;
            }

            _ = DalamudServices.Framework.RunOnFrameworkThread(addon.Close);
        }
    }

    public void Toggle<T>() where T : KTKAddon
    {
        foreach (KTKAddon addon in KTKWindows)
        {
            if (addon is not T)
            {
                continue;
            }

            _ = DalamudServices.Framework.RunOnFrameworkThread(addon.Toggle);
        }
    }

    public void Dispose()
    {
        foreach (KTKAddon window in KTKWindows)
        {
            _ = DalamudServices.Framework.RunOnFrameworkThread(window.Dispose);
        }

        KTKWindows.Clear();
    }
}
