using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LegacyHandlerUpdatable : Updatable
{
    public override void Update(ref IFramework frameWork, ref IPlayerCharacter player)
    {
        if (PluginLink.LegacyCompatibilityHandler.OnUpdate(ref frameWork, ref player)) return;
        PluginLink.UpdatableHandler.RegisterRemovable(this);
    }
}

