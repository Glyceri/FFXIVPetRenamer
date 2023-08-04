using Dalamud.Game;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.Updatable
{
    internal abstract class Updatable : IDisposableRegistryElement
    {
        public void Dispose() => OnDispose();
        protected virtual void OnDispose() { }
        public abstract unsafe void Update(Framework frameWork);
        public virtual unsafe void LateUpdate(Framework frameWork) { }
    }
}
