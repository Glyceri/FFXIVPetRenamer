using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using System;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal unsafe class MapTooltipTextHook : SimpleTextHook
{
    uint backgroundNodePos;
    bool blocked = false;
    AtkNineGridNode* bgNode;

    IPettablePet? currentPet = null;

    public override void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false)
    {
        base.Setup(services, userList, petServices, dirtyListener, AddonName, textPos, allowedCallback, isSoft);
        services.AddonLifecycle.UnregisterListener(AddonEvent.PostRequestedUpdate, HandleUpdate);
        services.AddonLifecycle.RegisterListener(AddonEvent.PreDraw, AddonName, HandleUpdate);
    }

    public void Register(uint backgroundNodePos)
    {
        this.backgroundNodePos = backgroundNodePos;
        SetUnfaulty();
    }

    public void SetBlockedState(bool isBlocked)
    {
        blocked = isBlocked;
    }

    public void SetPet(IPettablePet? pettablePet)
    {
        currentPet = pettablePet;
        SetDirty();
    }

    protected override bool BlockedCheck() => blocked || base.BlockedCheck();

    protected override unsafe AtkTextNode* GetTextNode(in BaseNode bNode)
    {
        if (backgroundNodePos != uint.MaxValue)
        {
            bgNode = bNode.GetNode<AtkNineGridNode>(backgroundNodePos);
        }
        return base.GetTextNode(in bNode);
    }

    protected override IPetSheetData? GetPetData(string text, in IPettableUser user)
    {
        if (currentPet?.Owner != user) return null;
        return currentPet?.PetData;
    }

    protected override unsafe void SetText(AtkTextNode* textNode, string text, string customName, IPetSheetData pPet)
    {
        base.SetText(textNode, text, customName, pPet);
        if (bgNode == null) return;
        if (textNode == null) return;
        textNode->ResizeNodeForCurrentText();
        bgNode->AtkResNode.SetWidth((ushort)(textNode->AtkResNode.Width + 18));
    }

    protected override IPettableUser? GetUser() => currentPet?.Owner;

    public override void OnDispose()
    {
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PreDraw, HandleUpdate);
    }
}
