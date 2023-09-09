using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Logging;
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
    [Signature("48 89 5C 24 ?? 55 48 83 EC 60 41 0F B6 E8 ", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<Delegates.AreaMapTooltipDelegate> showTooltipThing = null!;

    internal override void OnInit()
    {
        showTooltipThing?.Enable();
    }

    uint last = 0;

    const int playerIconID = 60443;
    const int petIconID = 60961;
    const int partyPlayerIconID = 60421;

    char ShowTooltipDetour(AtkUnitBase* areaMap, uint elementIndex, char a3)
    {
        TooltipHelper.lastTooltipWasMap = true;
        if (elementIndex == last) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        last = elementIndex;
        
        if (TooltipHelper.partyListInfos.Count == 0) return showTooltipThing!.Original(areaMap, elementIndex, a3);

        BaseNode node = new BaseNode(areaMap);
        if (node == null) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        ComponentNode mapComponentNode = node.GetComponentNode(44);
        if (mapComponentNode == null) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        AtkComponentNode* atkComponentNode = mapComponentNode.GetPointer();
        if (atkComponentNode == null) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        AtkComponentBase* atkComponentBase = atkComponentNode->Component;
        if (atkComponentBase == null) return showTooltipThing!.Original(areaMap, elementIndex, a3);
        AtkUldManager manager = atkComponentBase->UldManager;
        
        int startIndex = -1;

        for (int i = manager.NodeListCount - 1 ; i >= 0; i--)
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

        for(int i = 0; i < TooltipHelper.partyListInfos.Count; i++)
        {
            if (TooltipHelper.partyListInfos[i].hasPet)
            {
                actualCurrent++;
                if (startIndex + i + actualCurrent == elementIndex)
                {
                    PettableUser user = PluginLink.PettableUserHandler.GetUser(TooltipHelper.partyListInfos[i].UserName)!;
                    if (user == null) { PluginLog.Log("User null!"); break; }
                    if (!user.HasBattlePet) break;
                    TooltipHelper.SetNextUp(user);
                    break;
                }
            }

            if (TooltipHelper.partyListInfos[i].hasChocobo)
            {
                actualCurrent++;
                if (startIndex + i + actualCurrent == elementIndex) break;
            }
            if (i == 0) actualCurrent--;
        }

        return showTooltipThing!.Original(areaMap, elementIndex, a3);
    }

    internal override void OnDispose()
    {
        showTooltipThing?.Dispose();
    }
}
