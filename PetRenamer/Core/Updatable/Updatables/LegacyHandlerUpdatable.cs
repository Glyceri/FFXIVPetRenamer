using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

//[Updatable]
internal class LegacyHandlerUpdatable : Updatable
{
    public override void Update(Framework frameWork) => PluginLink.LegacyCompatibilityHandler?.OnUpdate(frameWork);
}

