using Dalamud.Game.Gui.ContextMenu;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.ContextMenu;

internal abstract class ContextMenuElement : IRegistryElement
{
    internal abstract void OnOpenMenu(MenuOpenedArgs args);
}
