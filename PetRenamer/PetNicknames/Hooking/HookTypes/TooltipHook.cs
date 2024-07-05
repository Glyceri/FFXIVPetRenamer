using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal unsafe class TooltipHook : SimpleTextHook
{
    uint backgroundNodePos;

    AtkNineGridNode* bgNode;

    public override void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false)
    {
        base.Setup(services, userList, petServices, AddonName, textPos, allowedCallback, isSoft);
        services.AddonLifecycle.UnregisterListener(AddonEvent.PostRequestedUpdate, HandleUpdate);
        services.AddonLifecycle.RegisterListener(AddonEvent.PreDraw, AddonName, HandleUpdate);
    }

    public void Register(uint backgroundNodePos)
    {
        this.backgroundNodePos = backgroundNodePos;
        SetUnfaulty();
    }

    protected override unsafe AtkTextNode* GetTextNode(ref BaseNode bNode)
    {
        bgNode = bNode.GetNode<AtkNineGridNode>(backgroundNodePos);
        return base.GetTextNode(ref bNode);
    }

    protected override unsafe void SetText(AtkTextNode* textNode, string text, string customName, PetSheetData pPet)
    {
        base.SetText(textNode, text, customName, pPet);
        if (bgNode == null) return;
        textNode->ResizeNodeForCurrentText();
        bgNode->AtkResNode.SetWidth((ushort)(textNode->AtkResNode.Width + 18));
    }

}
