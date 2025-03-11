using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TooltipHookHelper : ITooltipHookHelper
{
    readonly DalamudServices DalamudServices;

    // The dalamud TooltipType enum is NOT accurate it seems
    public delegate int AccurateShowTooltip(IntPtr tooltip, byte tooltipType, ushort addonID, IntPtr a4, IntPtr a5, IntPtr a6, ushort a7, ushort a8);
    public delegate void TooltipDelegate(IntPtr tooltip, byte tooltipType, ushort addonID, IntPtr a4, IntPtr a5, IntPtr a6, ushort a7, ushort a8);

    event TooltipDelegate? tooltipEvent = null;

    [Signature("E8 ?? ?? ?? ?? 33 D2 EB 02 ?? ?? ?? ?? ?? ?? ??", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<AccurateShowTooltip> showTooltip = null!;

    public TooltipHookHelper(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;

        DalamudServices.Hooking.InitializeFromAttributes(this);

        showTooltip.Enable();
    }

    int ShowTooltipDetour(IntPtr tooltip, byte tooltipType, ushort addonID, IntPtr a4, IntPtr a5, IntPtr a6, ushort a7, ushort a8)
    {
        CallCallbacks(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);
        return showTooltip!.Original(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);
    }

    void CallCallbacks(IntPtr tooltip, byte tooltipType, ushort addonID, IntPtr a4, IntPtr a5, IntPtr a6, ushort a7, ushort a8)
    {
        tooltipEvent?.Invoke(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);
    }

    public void RegisterCallback(TooltipDelegate callback)
    {
        DeregisterCallback(callback);
        tooltipEvent += callback;
    }

    public void DeregisterCallback(TooltipDelegate callback)
    {
        tooltipEvent -= callback;
    }

    public void Dispose()
    {
        tooltipEvent = null;
        showTooltip.Dispose();
    }
}
