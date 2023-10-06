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
        ComponentNode cNode1 = node.GetComponentNode(53);
        if (cNode1 == null) return showTooltipThing!.Original(a1, a2, a3);
        AtkComponentNode* atkComponentNode = cNode1.GetPointer();
        if (atkComponentNode == null) return showTooltipThing!.Original(a1, a2, a3);
        AtkComponentBase* atkCompontentBase = atkComponentNode->Component;
        if (atkCompontentBase == null) return showTooltipThing!.Original(a1, a2, a3);
        AtkUldManager manager = atkCompontentBase->UldManager;

        for(int i = 0; i < manager.NodeListCount; i++)
        {
            AtkResNode* curNode = manager.NodeList[i];
            if (curNode == null) continue;
            if (!curNode->IsVisible) continue;
            AtkComponentNode* cNode = curNode->GetAsAtkComponentNode();
            if (cNode == null) continue;
            AtkComponentBase* cBase = cNode->Component;
            if (cBase == null) continue;
            AtkResNode* resNode = cBase->GetImageNodeById(5);
            if (resNode == null) continue;
            AtkImageNode* imgNode = resNode->GetAsAtkImageNode();
            if (imgNode == null) continue;
            AtkUldPartsList* partsList = imgNode->PartsList;
            if (partsList == null) continue;
            AtkUldPart* parts = partsList->Parts;
            if (parts == null) continue;
            AtkUldAsset* asset = parts->UldAsset;
            if (asset == null) continue;
            AtkTexture texture = asset->AtkTexture;
            AtkTextureResource* textureResource = texture.Resource;
            if (textureResource == null) continue;
            if (textureResource->IconID == petIconID)
            {

            }
        }
        return showTooltipThing!.Original(a1, a2, a3);
    }

    const int playerIconID = 60443;
    const int petIconID = 60961;
    const int partyPlayerIconID = 60421;

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
