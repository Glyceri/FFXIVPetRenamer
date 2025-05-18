using Dalamud.Game.Gui.ContextMenu;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class TargetContextMenu : IContextMenuElement
{
    // Null means context menu didn't come from an addon
    public string? AddonName { get; } = null;

    readonly IPetServices       PetServices;
    readonly IPettableUserList  UserList;
    readonly IWindowHandler     WindowHandler;

    public TargetContextMenu(IPetServices petServices, IPettableUserList userList, IWindowHandler windowHandler)
    {
        PetServices     = petServices;
        UserList        = userList;
        WindowHandler   = windowHandler;
    }

    public Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return null;

        IPettableEntity? target = PetServices.TargetManager.Target;
        if (target == null) return null;

        IPettablePet? pet = localUser.GetPet(target.Address);
        if (pet == null)
        {
            IPettableUser? islandUser = UserList.PettableUsers[PettableUsers.PettableUserList.IslandIndex];
            if (islandUser == null) return null;

            pet = islandUser.GetPet(target.Address);
        }

        if (pet == null) return null;

        return (a) => WindowHandler.GetWindow<PetRenameWindow>()?.SetRenameWindow(pet.SkeletonID, true);
    }
}
