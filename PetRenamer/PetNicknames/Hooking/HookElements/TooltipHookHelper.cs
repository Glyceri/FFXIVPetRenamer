using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkTooltipManager;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TooltipHookHelper : ITooltipHookHelper
{
    readonly DalamudServices DalamudServices;

    public delegate void TooltipDelegate    (nint tooltip, AtkTooltipType tooltipType, ushort addonID, nint a4, nint a5, nint a6, bool a7, bool a8);

    event TooltipDelegate TooltipEvent = (_, _, _, _, _, _, _, _) => { };

    readonly Hook<AtkTooltipManager.Delegates.ShowTooltip> showTooltipHook = null!;

    public TooltipHookHelper(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;

        DalamudServices.Hooking.InitializeFromAttributes(this);

        showTooltipHook = DalamudServices.Hooking.HookFromAddress<AtkTooltipManager.Delegates.ShowTooltip>(AtkTooltipManager.Addresses.ShowTooltip.Value, AtkTooltipManagerShowTooltipDetour);

        showTooltipHook.Enable();
    }

    private void AtkTooltipManagerShowTooltipDetour(AtkTooltipManager* thisPtr, AtkTooltipManager.AtkTooltipType type, ushort parentId, AtkResNode* targetNode, AtkTooltipManager.AtkTooltipArgs* tooltipArgs, delegate* unmanaged[Stdcall]<float*, float*, void*> unkDelegate, bool unk7, bool unk8)
    {
        CallCallbacks((nint)thisPtr, type, parentId, (nint)targetNode, (nint)tooltipArgs, (nint)unkDelegate, unk7, unk8);

        showTooltipHook!.Original(thisPtr, type, parentId, targetNode, tooltipArgs, unkDelegate, unk7, unk8);
    }

    void CallCallbacks(nint tooltip, AtkTooltipType tooltipType, ushort addonID, nint a4, nint a5, nint a6, bool a7, bool a8)
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
        showTooltipHook.Dispose();
    }
}
