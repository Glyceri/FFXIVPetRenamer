using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.Structs;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class MapHook : HookableElement
{
    int lastIndex = 0;
    int current = 0;
    int foundCurrent = -1;

    const int petIconID = 60961;
    const int alliancePetIconID = 60964;

    readonly IMapTooltipHook TooltipHook;

    public MapHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IMapTooltipHook tooltipHook, IPettableDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener)
    {
        TooltipHook = tooltipHook;
    }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreRequestedUpdate, "_NaviMap", NaviMapUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreRequestedUpdate, "AreaMap", AreaMapUpdate);
    }

    void NaviMapUpdate(AddonEvent type, AddonArgs args) => MiniMapDetour(args.Addon);
    void AreaMapUpdate(AddonEvent type, AddonArgs args) => MapDetour(args.Addon);

    protected override void Refresh()
    {
        lastIndex = -1;
    }

    bool PrepareMap(int index)
    {
        if (lastIndex == index) return false;

        lastIndex = index;
        current = 0;
        foundCurrent = -1;

        return true;
    }

    void EndMap()
    {
        if (foundCurrent == -1) return;

        GetDistanceAt(foundCurrent);
    }

    void MapDetour(nint a1)
    {
        PetNicknamesAddonAreaMap* mapAddon = (PetNicknamesAddonAreaMap*)a1;
        if (mapAddon == null) return;

        if (mapAddon->TooltipHoveredIndex == -1) return;

        if (!PrepareMap(mapAddon->TooltipHoveredIndex)) return;

        MapTooltip((AtkUnitBase*)a1, mapAddon->TooltipHoveredIndex);

        EndMap();
    }

    void MiniMapDetour(nint a1)
    {
        PetNicknamesAddonNaviMap* naviMapAddon = (PetNicknamesAddonNaviMap*)a1;
        if (naviMapAddon == null) return;

        if (naviMapAddon->TooltipHoveredIndex == -1) return;

        if (!PrepareMap(naviMapAddon->TooltipHoveredIndex)) return;

        NaviTooltip((AtkUnitBase*)a1, naviMapAddon->TooltipHoveredIndex);

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

            if (manager.Value.PartsListCount + manager.Value.ObjectCount + manager.Value.AssetCount + i + 32 + 1 != index) continue;

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
        GroupManager* gManager = (GroupManager*)DalamudServices.PartyList.GroupManagerAddress;
        if (gManager == null) return;

        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return;

        BattleChara* pChara = localUser.BattleChara;
        if (pChara == null) return;

        DrawObject* drawObject = pChara->Character.DrawObject;
        if (drawObject == null) return;

        Vector3 playerPos = drawObject->Position;
        Vector2 flatPlayerPos = new Vector2(playerPos.X, playerPos.Z);

        List<IPettablePet> partyPets = new List<IPettablePet>();
        List<IPettablePet> alliPets  = new List<IPettablePet>();

        MakeFromMembers(gManager->MainGroup.PartyMembers,    ref partyPets);
        MakeFromMembers(gManager->MainGroup.AllianceMembers, ref alliPets);

        AddPets(localUser, ref partyPets);

        Sort(flatPlayerPos, ref partyPets);
        Sort(flatPlayerPos, ref alliPets);

        IPettablePet[] pets = [.. alliPets, .. partyPets];

        int index = at - 1;
        if (index < 0) return;

        TooltipHook.OverridePet(pets[index]);
    }

    void Sort(Vector2 flatPlayerPos, ref List<IPettablePet> pets)
    {
        pets = pets.Distinct().ToList();

        pets.Sort(
        (pet1, pet2) =>
            {
                BattleChara* p1 = (BattleChara*)pet1.Address;
                BattleChara* p2 = (BattleChara*)pet2.Address;

                Vector3 p1p = p1->Character.DrawObject != null ? p1->Character.DrawObject->Position : default;
                Vector3 p2p = p2->Character.DrawObject != null ? p2->Character.DrawObject->Position : default;

                Vector2 pos1 = flatPlayerPos - new Vector2(p1p.X, p1p.Z);
                Vector2 pos2 = flatPlayerPos - new Vector2(p2p.X, p2p.Z);

                return pos1.Length().CompareTo(pos2.Length());
            }
        );
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
            if (bPet.BattlePet == null) continue;
            if (!bPet.BattlePet->GetIsTargetable()) continue;

            pets.Add(pet);
        }
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(NaviMapUpdate);
        DalamudServices.AddonLifecycle.UnregisterListener(AreaMapUpdate);
    }
}
