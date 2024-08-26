using PetRenamer.PetNicknames.Hooking.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

internal interface IMapHook : IHookableElement
{
    List<uint> Icons { get; }
}
