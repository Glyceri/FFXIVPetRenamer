using PetRenamer.Core.AutoRegistry.Interfaces;
using System;

namespace PetRenamer.Core.EventSystem;

public abstract class EventElement : IDisposableRegistryElement
{
    public void Dispose()
    {
        OnDispose();
    }
    public virtual void OnDispose() { }
}
