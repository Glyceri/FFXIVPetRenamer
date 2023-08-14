using Dalamud.ContextMenu;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.ContextMenu;

internal abstract class ContextMenuElement : IRegistryElement
{
    internal abstract void OnOpenMenu(GameObjectContextMenuOpenArgs args);
}
