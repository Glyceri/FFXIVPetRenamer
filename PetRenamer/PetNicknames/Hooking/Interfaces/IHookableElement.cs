using PetRenamer.PetNicknames.Services;
using System;

namespace PetRenamer.PetNicknames.Hooking.Interfaces;

internal interface IHookableElement : IDisposable
{
    DalamudServices DalamudServices { get; }
    void Init();
}
