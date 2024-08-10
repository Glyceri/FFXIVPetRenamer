using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Gui.ContextMenu;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class TargetContextMenu : IContextMenuElement
{
    // Null means context menu didn't come from an addon
    public string? AddonName { get; } = null;

    readonly DalamudServices DalamudServices;
    readonly IPettableUserList UserList;
    readonly IWindowHandler WindowHandler;

    public TargetContextMenu(in DalamudServices dalamudServices, in IPettableUserList userList, in IWindowHandler windowHandler)
    {
        DalamudServices = dalamudServices;
        UserList = userList;
        WindowHandler = windowHandler;
    }

    public Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return null;

        IGameObject? target = DalamudServices.TargetManager.Target;
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
