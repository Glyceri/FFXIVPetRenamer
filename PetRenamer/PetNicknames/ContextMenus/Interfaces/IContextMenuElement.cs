using Dalamud.Game.Gui.ContextMenu;
using System;

namespace PetRenamer.PetNicknames.ContextMenus.Interfaces;

internal interface IContextMenuElement
{
    /// <summary>
    /// Setting this to null means you didn't expect it to come from an addon.
    /// This is totally valid.
    /// </summary>
    string? AddonName { get; }
    
    Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args);
}
