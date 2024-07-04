using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal unsafe class TooltipHook : SimpleTextHook
{
    uint backgroundNodePos;

    AtkNineGridNode* bgNode;

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
