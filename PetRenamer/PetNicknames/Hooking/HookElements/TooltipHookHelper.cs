using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkTooltipManager;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TooltipHookHelper : ITooltipHookHelper
{
    readonly DalamudServices DalamudServices;

    // The dalamud TooltipType enum is NOT accurate it seems
    public delegate int AccurateShowTooltip (nint tooltip, AtkTooltipType tooltipType, ushort addonID, nint a4, nint a5, nint a6, ushort a7, ushort a8);
    public delegate void TooltipDelegate    (nint tooltip, AtkTooltipType tooltipType, ushort addonID, nint a4, nint a5, nint a6, ushort a7, ushort a8);

    event TooltipDelegate TooltipEvent = (_, _, _, _, _, _, _, _) => { };

    [Signature("E8 ?? ?? ?? ?? 33 D2 EB 02 ?? ?? ?? ?? ?? ?? ??", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<AccurateShowTooltip> showTooltip = null!;

    public TooltipHookHelper(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;

        DalamudServices.Hooking.InitializeFromAttributes(this);

        showTooltip.Enable();
    }

    int ShowTooltipDetour(nint tooltip, AtkTooltipType tooltipType, ushort addonID, nint a4, nint a5, nint a6, ushort a7, ushort a8)
    {
        CallCallbacks(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);

        return showTooltip!.Original(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);
    }

    void CallCallbacks(nint tooltip, AtkTooltipType tooltipType, ushort addonID, nint a4, nint a5, nint a6, ushort a7, ushort a8)
    {
        TooltipEvent?.Invoke(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);
    }

    public void RegisterCallback(TooltipDelegate callback)
    {
        DeregisterCallback(callback);

        TooltipEvent += callback;
    }

    public void DeregisterCallback(TooltipDelegate callback)
    {
        TooltipEvent -= callback;
    }

    public void Dispose()
    {
        showTooltip.Dispose();
    }
}
