using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.Updatable
{
    internal abstract class Updatable : IDisposableRegistryElement
    {
        public void Dispose() => OnDispose();
        protected virtual void OnDispose() { }
        public abstract unsafe void Update(ref IFramework frameWork, ref IPlayerCharacter player);
    }
}
