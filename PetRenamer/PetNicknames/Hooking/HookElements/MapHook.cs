using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class MapHook : HookableElement, IMapHook
{
    int lastIndex = 0;
    int current = 0;
    int foundCurrent = -1;

    const int petIconID = 60961;
    const int alliancePetIconID = 60964;

    // IntPtr is the AtkMapAddon thing and AtkNaviMap thing, really it doesnt matter
    public delegate void NewMapDelegate(IntPtr a1);

    // 40 57 48 83 EC 60 48 8B F9 83 FA 64
    [Signature("40 57 48 81 EC B0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 90 00 00 00", DetourName = nameof(MiniMapDetour))]
    readonly Hook<NewMapDelegate> naviTooltip = null!;

    // "48 89 5C 24 ?? 55 48 83 EC 60 41 0F B6 E8"
    // This is the 6.58 hook
    // Everything has changed!

    [Signature("E8 ?? ?? ?? ?? 8B 8F 44 07 00 00", DetourName = nameof(MapDetour))]
    readonly Hook<NewMapDelegate> mapTooltipHook = null!;

    readonly IMapTooltipHook TooltipHook;

    public List<uint> Icons { get; } = new List<uint>();

    public MapHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IMapTooltipHook tooltipHook, IPettableDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener)
    {
        TooltipHook = tooltipHook;
    }

    public override void Init()
    {
        naviTooltip?.Enable();
        mapTooltipHook?.Enable();
    }

    protected override void Refresh()
    {
        lastIndex = -1;
    }

    bool PrepareMap(int index)
    {
        if (lastIndex != index) lastIndex = index;
        else return false;

        current = 0;
        foundCurrent = -1;
        Icons.Clear();

        return true;
    }

    void EndMap()
    {
        if (foundCurrent == -1) return;

        GetDistanceAt(foundCurrent);
    }

    void MapDetour(IntPtr a1)
    {
        mapTooltipHook!.Original(a1);

        int mapIndex = (int)(*(uint*)(a1 + 1860));
        if (mapIndex == -1) return;

        if (!PrepareMap(mapIndex)) return;

        MapTooltip((AtkUnitBase*)a1, mapIndex);

        EndMap();
    }

    void MiniMapDetour(IntPtr a1)
    {
        naviTooltip.Original(a1);

        int navimapIndex = (int)(*(uint*)(a1 + 14888));
        if (navimapIndex == -1) return;

        if (!PrepareMap(navimapIndex)) return;

        NaviTooltip((AtkUnitBase*)a1, navimapIndex);

        EndMap();
    }

    void NaviTooltip(AtkUnitBase* unitBase, int elementIndex)
    {
        AtkUldManager? manager = GetUldManager(unitBase, 18);
        if (manager == null) return;

        for (int i = 0; i < manager.Value.NodeListCount; i++)
        {
            if (!GetResources(3, manager.Value.NodeList[i], out AtkTextureResource* textureResource)) continue;
            if (!HandleTextureResource(textureResource)) continue;

            if (i != elementIndex + manager.Value.PartsListCount) continue;

            CurrentIsIndex();
        }
    }

    void MapTooltip(AtkUnitBase* a1, int index)
    {
        AtkUldManager? manager = GetUldManager(a1, 53);
        if (manager == null) return;

        for (int i = 0; i < manager.Value.NodeListCount; i++)
        {
            if (!GetResources(5, manager.Value.NodeList[i], out AtkTextureResource* textureResource)) continue;
            if (!HandleTextureResource(textureResource)) continue;

            if (manager.Value.PartsListCount + manager.Value.ObjectCount + manager.Value.AssetCount + i + 1 != index) continue;

            CurrentIsIndex();
        }
    }

    AtkUldManager? GetUldManager(AtkUnitBase* unitBase, uint slot)
    {
        BaseNode node = new BaseNode(unitBase);

        ComponentNode cNode1 = node.GetComponentNode(slot);
        if (cNode1 == null) return null;

        AtkComponentNode* atkComponentNode = cNode1.GetPointer();
        if (atkComponentNode == null) return null;

        AtkComponentBase* atkCompontentBase = atkComponentNode->Component;
        if (atkCompontentBase == null) return null;

        AtkUldManager manager = atkCompontentBase->UldManager;
        return manager;
    }

    bool GetResources(uint imageID, AtkResNode* curNode, out AtkTextureResource* textureResource)
    {
        textureResource = null;

        if (curNode == null) return false;
        if (!curNode->IsVisible()) return false;
        AtkComponentNode* cNode = curNode->GetAsAtkComponentNode();
        if (cNode == null) return false;
        AtkComponentBase* cBase = cNode->Component;
        if (cBase == null) return false;
        AtkResNode* resNode = cBase->GetImageNodeById(imageID);
        if (resNode == null) return false;
        AtkImageNode* imgNode = resNode->GetAsAtkImageNode();
        if (imgNode == null) return false;
        AtkUldPartsList* partsList = imgNode->PartsList;
        if (partsList == null) return false;
        AtkUldPart* parts = partsList->Parts;
        if (parts == null) return false;
        AtkUldAsset* asset = parts->UldAsset;
        if (asset == null) return false;
        AtkTexture texture = asset->AtkTexture;
        textureResource = texture.Resource;
        if (textureResource == null) return false;

        return true;
    }

    bool HandleTextureResource(AtkTextureResource* textureResource)
    {
        if (textureResource->IconId != petIconID && textureResource->IconId != alliancePetIconID) return false;

        Icons.Add(textureResource->IconId);

        current++;
        return true;
    }

    void CurrentIsIndex()
    {
        if (foundCurrent != -1) return;

        foundCurrent = current;
    }

    void GetDistanceAt(int at)
    {
        PetServices.PetLog.Log("Get distance at: " + at);
        GroupManager* gManager = (GroupManager*)DalamudServices.PartyList.GroupManagerAddress;
        if (gManager == null) return;
        PetServices.PetLog.Log("2");
        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return;
        PetServices.PetLog.Log("3");

        BattleChara* pChara = localUser.BattleChara;
        if (pChara == null) return;
        PetServices.PetLog.Log("4");
        Vector3 playerPos = pChara->Character.DrawObject->Position;
        Vector2 flatPlayerPos = new Vector2(playerPos.X, playerPos.Z);

        List<IPettablePet> partyPets = new List<IPettablePet>();
        List<IPettablePet> alliPets = new List<IPettablePet>();


        MakeFromMembers(gManager->MainGroup.PartyMembers, ref partyPets);
        MakeFromMembers(gManager->MainGroup.AllianceMembers, ref alliPets);
        AddPets(localUser, ref partyPets);

        Sort(flatPlayerPos, ref partyPets);
        Sort(flatPlayerPos, ref alliPets);

        List<IPettablePet> pets = [.. alliPets, .. partyPets,];

        PetServices.PetLog.Log("-----");
        foreach (IPettablePet pet in pets)
        {
            PetServices.PetLog.Log(pet.Name + " : " + pet.Owner?.Name);
        }
        PetServices.PetLog.Log("-----");


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

    void Sort(Vector2 flatPlayerPos, ref List<IPettablePet> pets)
    {
        pets = pets.Distinct().ToList();
        pets.Sort((pet1, pet2) =>
        {
            BattleChara* p1 = (BattleChara*)pet1.PetPointer;
            BattleChara* p2 = (BattleChara*)pet2.PetPointer;
            Vector3 p1p = p1->Character.DrawObject->Position;
            Vector3 p2p = p2->Character.DrawObject->Position;
            Vector2 pos1 = flatPlayerPos - new Vector2(p1p.X, p1p.Z);
            Vector2 pos2 = flatPlayerPos - new Vector2(p2p.X, p2p.Z);
            return pos1.Length().CompareTo(pos2.Length());
        });
    }

    void MakeFromMembers(Span<PartyMember> members, ref List<IPettablePet> pets)
    {
        foreach (PartyMember member in members)
        {
            IPettableUser? user = UserList.GetUserFromContentID(member.ContentId);
            if (user == null) continue;

            AddPets(user, ref pets);
        }
    }

    void AddPets(IPettableUser user, ref List<IPettablePet> pets)
    {
        foreach (IPettablePet pet in user.PettablePets)
        {
            if (pet is not IPettableBattlePet bPet) continue;
            if (pets.Contains(pet)) continue;
            if (!bPet.BattlePet->GetIsTargetable()) continue;

            pets.Add(pet);
        }
    }

    protected override void OnDispose()
    {
        naviTooltip?.Dispose();
        mapTooltipHook?.Dispose();
    }
}
