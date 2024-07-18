using Dalamud.Game.Gui.ContextMenu;
using System;

namespace PetRenamer.PetNicknames.ContextMenus.Interfaces;

internal interface IContextMenuElement
{
    string? AddonName { get; }
    Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args);
}
