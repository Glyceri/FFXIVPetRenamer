using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.ContextMenu.Attributes;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.ContextMenu;

internal class ContextMenuHandler : RegistryBase<ContextMenuElement, ContextMenuAttribute>
{
    protected override void OnElementCreation(ContextMenuElement element) =>
        PluginHandlers.ContextMenu.OnMenuOpened += element.OnOpenMenu;
    
    protected override void OnElementDestroyed(ContextMenuElement element) =>
        PluginHandlers.ContextMenu.OnMenuOpened -= element.OnOpenMenu;
}
