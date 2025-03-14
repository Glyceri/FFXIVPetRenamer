﻿using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services;
using System;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal unsafe class ActionTooltipTextHook : SimpleTextHook
{
    uint backgroundNodePos;
    AtkNineGridNode* bgNode;

    IPetSheetData? currentData = null;

    public override void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool allowColours, bool isSoft = false)
    {
        base.Setup(services, userList, petServices, dirtyListener, AddonName, textPos, allowedCallback, allowColours, isSoft);
        services.AddonLifecycle.UnregisterListener(AddonEvent.PostRequestedUpdate, HandleUpdate);
        services.AddonLifecycle.RegisterListener(AddonEvent.PreDraw, AddonName, HandleUpdate);
    }

    public void Register(uint backgroundNodePos = uint.MaxValue)
    {
        this.backgroundNodePos = backgroundNodePos;
        SetUnfaulty();
    }

    protected override unsafe AtkTextNode* GetTextNode(in BaseNode bNode)
    {
        if (backgroundNodePos != uint.MaxValue)
        {
            bgNode = bNode.GetNode<AtkNineGridNode>(backgroundNodePos);
        }
        return base.GetTextNode(in bNode);
    }

    protected override unsafe void SetText(AtkTextNode* textNode, string text, string customName, IPetSheetData pPet)
    {
        base.SetText(textNode, text, customName, pPet);
        if (bgNode == null) return;
        if (textNode == null) return;
        textNode->ResizeNodeForCurrentText();
        bgNode->AtkResNode.SetWidth((ushort)(textNode->AtkResNode.Width + 18));
    }

    public void SetPetSheetData(IPetSheetData? petSheetData) => currentData = petSheetData;

    protected override IPetSheetData? GetPetData(string text, in IPettableUser user) => currentData;

    public override void OnDispose()
    {
        Services.AddonLifecycle.UnregisterListener(AddonEvent.PreDraw, HandleUpdate);
    }
}
