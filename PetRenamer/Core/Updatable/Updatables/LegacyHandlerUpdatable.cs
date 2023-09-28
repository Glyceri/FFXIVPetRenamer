using Dalamud.Game;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LegacyHandlerUpdatable : Updatable
{
    public override void Update(IFramework frameWork) => PluginLink.LegacyCompatibilityHandler?.OnUpdate(frameWork);
}

