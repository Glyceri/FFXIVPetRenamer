using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.ContextMenu.Attributes;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.ContextMenu;

internal class ContextMenuHandler : RegistryBase<ContextMenuElement, ContextMenuAttribute>
{
    protected override void OnElementCreation(ContextMenuElement element) =>
        PluginLink.DalamudContextMenu.OnOpenGameObjectContextMenu += element.OnOpenMenu;
    
    protected override void OnElementDestroyed(ContextMenuElement element) =>
        PluginLink.DalamudContextMenu.OnOpenGameObjectContextMenu -= element.OnOpenMenu;
}
