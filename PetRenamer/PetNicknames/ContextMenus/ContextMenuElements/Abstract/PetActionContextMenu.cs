using Dalamud.Game.Gui.ContextMenu;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements.Abstract;

internal abstract class PetActionContextMenu : IContextMenuElement
{
    public abstract string? AddonName { get; }

    private readonly IPetServices       PetServices;
    private readonly IPettableUserList  UserList;
    private readonly IWindowHandler     WindowHandler;
    private readonly IHoverService      HoverService;

    public PetActionContextMenu(IPetServices petServices, IPettableUserList userList, IWindowHandler windowHandler)
    {
        PetServices       = petServices;
        UserList          = userList;
        WindowHandler     = windowHandler;
        HoverService      = petServices.HoverService;
    }

    public Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        IPettableUser? localUser = UserList.LocalPlayer;

        if (localUser == null)
        {
            return null;
        }

        IPetSheetData? petData = HoverService.CurrentlyHoveredPet;

        if (petData == null)
        {
            return null;
        }

        return (a) =>
        {
            WindowHandler.GetWindow<PetRenameWindow>()?.SetRenameWindow(petData.Model, true);
        };
    }
}
