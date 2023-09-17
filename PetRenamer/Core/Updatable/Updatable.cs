using Dalamud.Game;
using PetRenamer.Core.Attributes;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.Updatable
{
    internal abstract class Updatable : IDisposableRegistryElement, IInitializable
    {
        public void Dispose() => OnDispose();
        protected virtual void OnDispose() { }
        public abstract unsafe void Update(Framework frameWork);
        protected virtual void OnInitialize() { }
        public void Initialize() => OnInitialize();
    }
}
