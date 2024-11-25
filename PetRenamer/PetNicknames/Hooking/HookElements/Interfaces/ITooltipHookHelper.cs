using System;
using static PetRenamer.PetNicknames.Hooking.HookElements.TooltipHookHelper;

namespace PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

internal interface ITooltipHookHelper : IDisposable
{
    void RegisterCallback(TooltipDelegate callback);
    void DeregisterCallback(TooltipDelegate callback);
}