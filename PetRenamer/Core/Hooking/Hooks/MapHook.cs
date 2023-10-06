using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Logging;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class MapHook : HookableElement
{
    int lastIndex = 0;
    int current = 0;

    const int playerIconID = 60443;
    const int petIconID = 60961;
    const int partyPlayerIconID = 60421;

    [Signature("40 57 48 83 EC 60 48 8B F9 83 FA 64", DetourName = nameof(NaviTooltip))]
    readonly Hook<Delegates.NaviMapTooltip> naviTooltip = null!;

    [Signature("48 89 5C 24 ?? 55 48 83 EC 60 41 0F B6 E8 ", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<Delegates.AreaMapTooltipDelegate> showTooltipThing = null!;

    unsafe char NaviTooltip(AtkUnitBase* unitBase, int elementIndex)
    {
        TooltipHelper.lastWasMap = true;
        if (lastIndex != elementIndex) lastIndex = elementIndex;
        else return naviTooltip!.Original(unitBase, elementIndex);

        BaseNode node = new BaseNode(unitBase);
        if (node == null) return naviTooltip!.Original(unitBase, elementIndex);
        ComponentNode mapComponentNode = node.GetComponentNode(18);
        if (mapComponentNode == null) return naviTooltip!.Original(unitBase, elementIndex);
        AtkComponentNode* atkComponentNode = mapComponentNode.GetPointer();
        if (atkComponentNode == null) return naviTooltip!.Original(unitBase, elementIndex);
        AtkComponentBase* atkComponentBase = atkComponentNode->Component;
        if (atkComponentBase == null) return naviTooltip!.Original(unitBase, elementIndex);
        AtkUldManager manager = atkComponentBase->UldManager;

        current = 0;

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
            if (textureResource->IconID != petIconID) continue;
            current++;
            if (i != elementIndex + manager.PartsListCount) continue;
            GetDistanceAt(current);
            return naviTooltip!.Original(unitBase, elementIndex);
        }
        TooltipHelper.nextUser = null!;
        return naviTooltip!.Original(unitBase, elementIndex);
    }

    unsafe char ShowTooltipDetour(AtkUnitBase* a1, uint a2, char a3)
    {
        TooltipHelper.lastWasMap = true;
        if (lastIndex != a2) lastIndex = (int)a2;
        else return showTooltipThing!.Original(a1, a2, a3);

        current = 0;
        int index = (int)a2;

        BaseNode node = new BaseNode(a1);
        ComponentNode cNode1 = node.GetComponentNode(53);
        if (cNode1 == null) return showTooltipThing!.Original(a1, a2, a3);
        AtkComponentNode* atkComponentNode = cNode1.GetPointer();
        if (atkComponentNode == null) return showTooltipThing!.Original(a1, a2, a3);
        AtkComponentBase* atkCompontentBase = atkComponentNode->Component;
        if (atkCompontentBase == null) return showTooltipThing!.Original(a1, a2, a3);
        AtkUldManager manager = atkCompontentBase->UldManager;

        for (int i = 0; i < manager.NodeListCount; i++)
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
            if (textureResource->IconID != petIconID) continue;
            current++;
            if (manager.PartsListCount + manager.ObjectCount + manager.AssetCount + i + 1 != a2) continue;
            GetDistanceAt(current);
            return showTooltipThing!.Original(a1, a2, a3);
        }
        TooltipHelper.nextUser = null!;
        return showTooltipThing!.Original(a1, a2, a3);
    }

    unsafe void GetDistanceAt(int at)
    {
        GroupManager* gManager = (GroupManager*)PluginHandlers.PartyList.GroupManagerAddress;
        if (gManager == null) return;

        List<nint> pets = new List<nint>();
        BattleChara* pChara = ((BattleChara*)PluginLink.PettableUserHandler.LocalUser()!.nintUser);
        if (pChara == null) return;
        Vector3 playerPos = pChara->Character.GameObject.Position;

        foreach (PartyMember member in gManager->PartyMembersSpan)
        {
            BattleChara* player = PluginLink.CharacterManager->LookupBattleCharaByObjectId(member.ObjectID);
            if (player == null) continue;
            BattleChara* chocobo = PluginLink.CharacterManager->LookupBuddyByOwnerObject(player);
            BattleChara* battlePet = PluginLink.CharacterManager->LookupPetByOwnerObject(player);
            if (battlePet != null) pets.Add((nint)battlePet);
            if (chocobo != null) pets.Add((nint)chocobo);
        }

        BattleChara* chocobo2 = PluginLink.CharacterManager->LookupBuddyByOwnerObject(pChara);
        BattleChara* battlePet2 = PluginLink.CharacterManager->LookupPetByOwnerObject(pChara);
        if (battlePet2 != null && !pets.Contains((nint)battlePet2)) pets.Add((nint)battlePet2);
        if (chocobo2 != null && !pets.Contains((nint)chocobo2)) pets.Add((nint)chocobo2);

        pets.Sort((pet1, pet2) =>
        {
            BattleChara* p1 = (BattleChara*)pet1;
            BattleChara* p2 = (BattleChara*)pet2;
            Vector3 pos1 = playerPos - (Vector3)p1->Character.GameObject.Position;
            Vector3 pos2 = playerPos - (Vector3)p2->Character.GameObject.Position;
            return pos1.Length().CompareTo(pos2.Length());
        });
        TooltipHelper.nextUser = PluginLink.PettableUserHandler.GetUser(pets[at - 1]);
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
