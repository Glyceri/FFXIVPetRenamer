using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Hooking.Attributes;
using System;

namespace PetRenamer.Core.Hooking.Hooks;

//Hooks from: https://github.com/NightmareXIV/QuestAWAY/blob/master/QuestAWAY/QuestAWAY.cs#L183

//[Hook]
internal unsafe class MapHook : HookableElement
{
    [Signature("48 8B C4 53 56 48 83 EC 78", DetourName = nameof(AreaMapOnMouseMoveDetour))]
    readonly Hook<Delegates.AreaMapOnMouseMoveDelegate> areaMapMouseMoveHook = null!;

    [Signature("E8 ?? ?? ?? ?? 8B F0 3B 87", DetourName = nameof(NaviMapOnMouseMoveDetour))]
    readonly Hook<Delegates.NaviMapOnMouseMoveDelegate> naviMapMouseMoveHook = null!;

    [Signature("48 89 5C 24 ?? 55 48 83 EC 60 41 0F B6 E8 ", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<Delegates.WhatsThis> showTooltipThing = null!;

    internal override void OnInit()
    {
        areaMapMouseMoveHook?.Enable();
        naviMapMouseMoveHook?.Enable();
    }

    uint last = 0;

    char ShowTooltipDetour(IntPtr a1, uint a2, char a3)
    {
        if (a3 == 0) return showTooltipThing!.Original(a1, a2, a3);

        if(a2 != last)
        {
            last = a2;

            // Do the calculation here :D
        }

        PluginLog.Log("Show tooltip! " + a2);
        return showTooltipThing!.Original(a1, a2, a3);
    }

    IntPtr NaviMapOnMouseMoveDetour(AtkUnitBase* addonNaviMap, IntPtr unk2, IntPtr unk3)
    {
        showTooltipThing?.Enable();
        IntPtr val = naviMapMouseMoveHook!.Original(addonNaviMap, unk2, unk3);
        showTooltipThing?.Disable();
        return val;
    }

    IntPtr AreaMapOnMouseMoveDetour(AtkUnitBase* addonAreaMap, IntPtr unk2)
    {
        showTooltipThing?.Enable();
        IntPtr val = areaMapMouseMoveHook!.Original(addonAreaMap, unk2);
        showTooltipThing?.Disable();
        return val;
    }

    internal override void OnDispose()
    {
        areaMapMouseMoveHook?.Dispose();
        naviMapMouseMoveHook?.Dispose();
        showTooltipThing?.Dispose();
    }
}
