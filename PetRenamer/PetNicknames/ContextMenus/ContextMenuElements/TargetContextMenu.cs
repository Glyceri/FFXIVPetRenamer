using Dalamud.Game.Gui.ContextMenu;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class TargetContextMenu : IContextMenuElement
{
    private readonly IPetServices   PetServices;
    private readonly IWindowHandler WindowHandler;

    public TargetContextMenu(IPetServices petServices, IWindowHandler windowHandler)
    {
        PetServices     = petServices;
        WindowHandler   = windowHandler;
    }
    
    public string? AddonName
        => null;

    public Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        IPettableUser? localUser = PetServices.UserList.LocalPlayer;

        if (localUser == null)
        {
            return null;
        }

        IPettableEntity? target = PetServices.TargetManager.LeadingTarget;

        if (target == null)
        {
            return null;
        }
        
        IPettablePet? pet = localUser.GetPet(target.Address);

        if (pet == null)
        {
            IPettableUser? islandUser = PetServices.UserList[IUserList.IslandIndex];

            if (islandUser == null)
            {
                return null;
            }

            if (islandUser.DataBaseEntry.ContentId != PetServices.UserList.LocalPlayer?.DataBaseEntry.ContentId)
            {
                return null;
            }
            
            pet = islandUser.GetPet(target.Address);
        }

        if (pet == null)
        {
            return null;
        }

        if (PetServices.PetSheets.GetPet(pet.SkeletonId) == null)
        {
            return null;
        }

        return _ =>
        {
            WindowHandler.GetWindow<PetRenameWindow>()?.SetRenameWindow(pet.SkeletonId);
        };
    }
}
