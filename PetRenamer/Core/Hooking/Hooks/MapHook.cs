using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;

namespace PetRenamer.Core.Hooking.Hooks;

//I can also proudly say, these hooks are MY OWN :DDDDDDDDDDDD

[Hook]
internal unsafe class MapHook : HookableElement
{
    [Signature("40 57 48 83 EC 60 48 8B F9 83 FA 64", DetourName = nameof(NaviTooltip))]
    readonly Hook<Delegates.NaviMapTooltip> naviTooltip = null!;

    [Signature("48 89 5C 24 ?? 55 48 83 EC 60 41 0F B6 E8 ", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<Delegates.AreaMapTooltipDelegate> showTooltipThing = null!;

    internal override void OnInit()
    {
        showTooltipThing?.Enable();
        naviTooltip?.Enable();
    }

    char NaviTooltip(AtkUnitBase* unitBase, int elementIndex)
    {
        if (!PluginLink.Configuration.displayCustomNames || !PluginLink.Configuration.allowTooltipsBattlePets) 
            return naviTooltip!.Original(unitBase, elementIndex);
        TooltipHelper.lastTooltipWasMap = true;

        if (elementIndex == last) return naviTooltip!.Original(unitBase, elementIndex);
        if (TooltipHelper.TickList)
        {
            TooltipHelper.TickList = false;
            return naviTooltip!.Original(unitBase, elementIndex);
        }
        TooltipHelper.TickList = true;
        last = (uint)elementIndex;

        if (TooltipHelper.partyListInfos.Count == 0) return naviTooltip!.Original(unitBase, elementIndex);

        BaseNode node = new BaseNode(unitBase);
        if (node == null) return naviTooltip!.Original(unitBase, elementIndex);
        ComponentNode mapComponentNode = node.GetComponentNode(18);
        if (mapComponentNode == null) return naviTooltip!.Original(unitBase, elementIndex);
        AtkComponentNode* atkComponentNode = mapComponentNode.GetPointer();
        if (atkComponentNode == null) return naviTooltip!.Original(unitBase, elementIndex);
        AtkComponentBase* atkComponentBase = atkComponentNode->Component;
        if (atkComponentBase == null) return naviTooltip!.Original(unitBase, elementIndex);
        AtkUldManager manager = atkComponentBase->UldManager;

        elementIndex += manager.PartsListCount;
        int startIndex = -1;

        for (int i = 0; i < manager.NodeListCount; i++)
        {
            AtkResNode* curNode = manager.NodeList[i];
            if (curNode == null) continue;
            if (!curNode->IsVisible) continue;
            AtkComponentNode* cNode = curNode->GetAsAtkComponentNode();
            if (cNode == null) continue;
            AtkComponentBase* cBase = cNode->Component;
            if (cBase == null) continue;
            AtkResNode* resNode = cBase->GetImageNodeById(3);
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
            if (textureResource->IconID == playerIconID)
            {
                startIndex = i;
                break;
            }            
        }

        if (startIndex == -1) return naviTooltip!.Original(unitBase, elementIndex);
        TooltipHelper.nextUser = null!;

        int actualCurrent = 0;
        int minusMe = 0;

        for (int i = 0; i < TooltipHelper.partyListInfos.Count; i++)
        {
            if (TooltipHelper.partyListInfos[i].hasPet)
            {
                actualCurrent++;
                int calculation = startIndex + i + actualCurrent + minusMe;
                if (calculation < 0) calculation = 0;
                if (calculation == elementIndex)
                {
                    PettableUser user = PluginLink.PettableUserHandler.GetUser(TooltipHelper.partyListInfos[i].UserName)!;
                    if (user == null) break;
                    TooltipHelper.SetNextUp(user);
                    break;
                }
            }

            if (TooltipHelper.partyListInfos[i].hasChocobo)
            {
                actualCurrent++; 
                int calculation = startIndex + i + actualCurrent + minusMe;
                if (calculation < 0) calculation = 0;
                if (calculation == elementIndex) break;
            }
            if (i == 0) minusMe--;
        }
        return naviTooltip!.Original(unitBase, elementIndex);
    }

    uint last = 0;

    const int playerIconID = 60443;
    const int petIconID = 60961;
    const int partyPlayerIconID = 60421;

    char ShowTooltipDetour(AtkUnitBase* areaMap, uint elementIndex, char a3)
    {
        if (!PluginLink.Configuration.displayCustomNames || !PluginLink.Configuration.allowTooltipsBattlePets)
            return showTooltipThing!.Original(areaMap, elementIndex, a3);
        TooltipHelper.lastTooltipWasMap = true;
        if (elementIndex == last) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        if (TooltipHelper.TickList)
        {
            TooltipHelper.TickList = false;
            return showTooltipThing!.Original(areaMap, elementIndex, a3);
        }
        TooltipHelper.TickList = true;
        last = elementIndex;

        if (TooltipHelper.partyListInfos.Count == 0) return showTooltipThing!.Original(areaMap, elementIndex, a3);

        BaseNode node = new BaseNode(areaMap);
        if (node == null) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        ComponentNode mapComponentNode = node.GetComponentNode(53);
        if (mapComponentNode == null) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        AtkComponentNode* atkComponentNode = mapComponentNode.GetPointer();
        if (atkComponentNode == null) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        AtkComponentBase* atkComponentBase = atkComponentNode->Component;
        if (atkComponentBase == null) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        AtkUldManager manager = atkComponentBase->UldManager;

        int startIndex = -1;

        for (int i = manager.NodeListCount - 1; i >= 0; i--)
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
            if (textureResource->IconID == playerIconID)
            {
                startIndex = i + manager.PartsListCount + manager.ObjectCount + manager.AssetCount + 1;
                break;
            }
        }

        if (startIndex == -1) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        TooltipHelper.nextUser = null!;

        int actualCurrent = 0;
        int minusMe = 0;

        for (int i = 0; i < TooltipHelper.partyListInfos.Count; i++)
        {
            if (TooltipHelper.partyListInfos[i].hasPet)
            {
                actualCurrent++;
                int calculation = startIndex + i + actualCurrent + minusMe;
                if (calculation < 0) calculation = 0;
                if (calculation == elementIndex)
                {
                    PettableUser user = PluginLink.PettableUserHandler.GetUser(TooltipHelper.partyListInfos[i].UserName)!;
                    if (user == null) break;
                    TooltipHelper.SetNextUp(user);
                    break;
                }
            }

            if (TooltipHelper.partyListInfos[i].hasChocobo)
            {
                actualCurrent++;
                int calculation = startIndex + i + actualCurrent + minusMe;
                if (calculation < 0) calculation = 0;
                if (calculation == elementIndex) break;
            }

            if (i == 0) { minusMe--; }
        }

        return showTooltipThing!.Original(areaMap, elementIndex, a3);
    }

    internal override void OnDispose()
    {
        showTooltipThing?.Dispose(); 
        naviTooltip?.Dispose();
    }
}
