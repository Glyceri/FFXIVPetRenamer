using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Logging;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class MapHook : HookableElement
{

    [Signature("40 57 48 83 EC 60 48 8B F9 83 FA 64", DetourName = nameof(NaviTooltip))]
    readonly Hook<Delegates.NaviMapTooltip> naviTooltip = null!;

    [Signature("48 89 5C 24 ?? 55 48 83 EC 60 41 0F B6 E8 ", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<Delegates.AreaMapTooltipDelegate> showTooltipThing = null!;

    unsafe char NaviTooltip(AtkUnitBase* tooltip, int a2)
    {
        TooltipHelper.lastWasMap = true;
        int index = a2;
        return naviTooltip!.Original(tooltip, a2);
    }

    unsafe char ShowTooltipDetour(AtkUnitBase* a1, uint a2, char a3)
    {
        TooltipHelper.lastWasMap = true;
        int index = (int)a2;

        BaseNode node = new BaseNode("AreaMap");
        ComponentNode cNode = node.GetComponentNode(53);
        if (cNode == null) return showTooltipThing!.Original(a1, a2, a3);
        AtkComponentBase* atkComponentBase = (AtkComponentBase*)cNode.GetPointer();
        if (atkComponentBase == null) return showTooltipThing!.Original(a1, a2, a3);
        AtkUldManager manager = atkComponentBase->UldManager;

        for(int i = 0; i < manager.NodeListCount; i++)
        {
             AtkResNode* resNode = manager.NodeList[i];
            if (resNode == null) continue;
            if (!resNode->IsVisible) continue;
            
        }
        return showTooltipThing!.Original(a1, a2, a3);
    }



    internal override void OnInit()
    {
        showTooltipThing?.Enable();
        naviTooltip?.Enable();
    }

    internal override void OnDispose()
    {
        naviTooltip?.Dispose();
        showTooltipThing?.Dispose();
    }
}
