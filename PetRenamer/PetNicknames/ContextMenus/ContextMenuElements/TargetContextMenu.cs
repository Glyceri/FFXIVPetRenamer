using Dalamud.Game.Gui.ContextMenu;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.KTKWindowing;
using PetRenamer.PetNicknames.KTKWindowing.Addons;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class TargetContextMenu : IContextMenuElement
{
    // Null means context menu didn't come from an addon
    public string? AddonName { get; } = null;

    private readonly IPetServices       PetServices;
    private readonly IPettableUserList  UserList;
    private readonly KTKWindowHandler   KTKWindowHandler;

    public TargetContextMenu(IPetServices petServices, IPettableUserList userList, KTKWindowHandler ktkWindowHandler)
    {
        PetServices      = petServices;
        UserList         = userList;
        KTKWindowHandler = ktkWindowHandler;
    }

    public Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        IPettableUser? localUser = UserList.LocalPlayer;

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
            IPettableUser? islandUser = UserList.PettableUsers[PettableUsers.PettableUserList.IslandIndex];

            if (islandUser == null)
            {
                return null;
            }

            pet = islandUser.GetPet(target.Address);
        }

        if (pet == null)
        {
            return null;
        }

        return (a) =>
        {
            KTKWindowHandler.Open<PetRenameAddon>();
            KTKWindowHandler.GetAddon<PetRenameAddon>()?.SetPetSkeleton(pet.SkeletonID);
        };
    }
}
