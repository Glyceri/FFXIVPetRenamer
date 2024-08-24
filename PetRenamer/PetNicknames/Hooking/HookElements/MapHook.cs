﻿using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class MapHook : HookableElement
{
    int lastIndex = 0;
    int current = 0;

    const int petIconID = 60961;
    const int alliancePetIconID = 60964;

    // IntPtr is the AtkMapAddon thing and AtkNaviMap thing, really it doesnt matter
    public delegate void NewMapDelegate(IntPtr a1);

    // 40 57 48 83 EC 60 48 8B F9 83 FA 64
    [Signature("40 57 48 81 EC B0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 90 00 00 00 ", DetourName = nameof(MiniMapDetour))]
    readonly Hook<NewMapDelegate> naviTooltip = null!;

    // "48 89 5C 24 ?? 55 48 83 EC 60 41 0F B6 E8"
    // This is the 6.58 hook
    // Everything has changed!

    [Signature("E8 ?? ?? ?? ?? 8B 8F 44 07 00 00 ", DetourName = nameof(MapDetour))]
    readonly Hook<NewMapDelegate> mapTooltipHook = null!;

    readonly IMapTooltipHook TooltipHook;

    public MapHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IMapTooltipHook tooltipHook, IPettableDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener)
    {
        TooltipHook = tooltipHook;
    }

    public override void Init()
    {
        naviTooltip?.Enable();
        mapTooltipHook?.Enable();
    }

    void MapDetour(IntPtr a1)
    {
        mapTooltipHook!.Original(a1);
        int mapIndex = (int)(*(uint*)(a1 + 1860));
        if (mapIndex == -1) return;
        MapTooltip((AtkUnitBase*)a1, mapIndex);
    }

    void MiniMapDetour(IntPtr a1)
    {
        naviTooltip.Original(a1);
        int navimapIndex = (int)(*(uint*)(a1 + 14888));
        if (navimapIndex == -1) return;

        NaviTooltip((AtkUnitBase*)a1, navimapIndex);
    }

    void NaviTooltip(AtkUnitBase* unitBase, int elementIndex)
    {
        if (lastIndex != elementIndex) lastIndex = elementIndex;
        else return;

        BaseNode node = new BaseNode(unitBase);
        if (node == null) return;
        ComponentNode mapComponentNode = node.GetComponentNode(18);
        if (mapComponentNode == null) return;
        AtkComponentNode* atkComponentNode = mapComponentNode.GetPointer();
        if (atkComponentNode == null) return;
        AtkComponentBase* atkComponentBase = atkComponentNode->Component;
        if (atkComponentBase == null) return;
        AtkUldManager manager = atkComponentBase->UldManager;

        current = 0;

        for (int i = 0; i < manager.NodeListCount; i++)
        {
            AtkResNode* curNode = manager.NodeList[i];
            if (curNode == null) continue;
            if (!curNode->IsVisible()) continue;
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
            if (textureResource->IconId != petIconID && textureResource->IconId != alliancePetIconID) continue;
            current++;
            if (i != elementIndex + manager.PartsListCount) continue;
            GetDistanceAt(current);
            return;
        }
    }

    void MapTooltip(AtkUnitBase* a1, int index)
    {
        if (lastIndex != index) lastIndex = index;
        else return;

        current = 0;
        if (index == -1) return;

        BaseNode node = new BaseNode(a1);
        ComponentNode cNode1 = node.GetComponentNode(53);
        if (cNode1 == null) return;
        AtkComponentNode* atkComponentNode = cNode1.GetPointer();
        if (atkComponentNode == null) return;
        AtkComponentBase* atkCompontentBase = atkComponentNode->Component;
        if (atkCompontentBase == null) return;
        AtkUldManager manager = atkCompontentBase->UldManager;

        for (int i = 0; i < manager.NodeListCount; i++)
        {
            AtkResNode* curNode = manager.NodeList[i];
            if (curNode == null) continue;
            if (!curNode->IsVisible()) continue;
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
            if (textureResource->IconId != petIconID && textureResource->IconId != alliancePetIconID) continue;
            current++;
            if (manager.PartsListCount + manager.ObjectCount + manager.AssetCount + i + 1 != index) continue;
            GetDistanceAt(current);
            return;
        }
    }

    void GetDistanceAt(int at)
    {
        PetServices.PetLog.Log("1");
        GroupManager* gManager = (GroupManager*)DalamudServices.PartyList.GroupManagerAddress;
        if (gManager == null) return;
        PetServices.PetLog.Log("2");
        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return;
        PetServices.PetLog.Log("3");

        BattleChara* pChara = localUser.BattleChara;
        if (pChara == null) return;
        PetServices.PetLog.Log("4");
        Vector3 playerPos = pChara->Character.GameObject.Position;

        List<IPettablePet> pets = new List<IPettablePet>();
        MakeFromMembers(gManager->MainGroup.PartyMembers, ref pets);
        MakeFromMembers(gManager->MainGroup.AllianceMembers, ref pets);
        AddPets(localUser, ref pets);     

        pets.Sort((pet1, pet2) =>
        {
            BattleChara* p1 = (BattleChara*)pet1.PetPointer;
            BattleChara* p2 = (BattleChara*)pet2.PetPointer;
            Vector3 pos1 = playerPos - (Vector3)p1->Character.GameObject.Position;
            Vector3 pos2 = playerPos - (Vector3)p2->Character.GameObject.Position;
            return pos1.Length().CompareTo(pos2.Length());
        });

        pets.Reverse();

        foreach (IPettablePet pp in pets)
        {
            PetServices.PetLog.Log(pp.Owner?.Name + " : " + pp.Name + " : " + pp.CustomName);
        }

        int index = at - 1;
        PetServices.PetLog.Log("Index: " + index);

        if (index < 0) return;
        PetServices.PetLog.Log("5");
        if (index >= pets.Count) return;
        PetServices.PetLog.Log("6");
        TooltipHook.OverridePet(pets[index]);
    }

    void MakeFromMembers(Span<PartyMember> members, ref List<IPettablePet> pets)
    {
        foreach (PartyMember member in members)
        {

            PetServices.PetLog.LogFatal(member.NameString);
            IPettableUser? user = UserList.GetUserFromContentID(member.ContentId, false);
            
            if (user == null)
            {
                PetServices.PetLog.LogFatal("User is null: " + member.ContentId);
                continue;
            }

            AddPets(user, ref pets);
        }
    }

    void AddPets(IPettableUser user, ref List<IPettablePet> pets)
    {
        foreach (IPettablePet pet in user.PettablePets)
        {
            PetServices.PetLog.Log(user.Name + " | " + pet.Name);
            if (pet is IPettableCompanion) continue;
            if (pets.Contains(pet)) continue;

            pets.Add(pet);
        }
    }


    protected override void OnDispose()
    {
        naviTooltip?.Dispose();
        mapTooltipHook?.Dispose();
    }
}
