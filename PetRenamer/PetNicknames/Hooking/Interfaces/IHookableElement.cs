using System;

namespace PetRenamer.PetNicknames.Hooking.Interfaces;

internal interface IHookableElement : IDisposable
{
    void Init();
}
