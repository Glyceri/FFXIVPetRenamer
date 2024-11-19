using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using System;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal unsafe class TargetTextHook : SimpleTextHook
{
    Func<IPettablePet?>? callGetPet = null;
    Func<IPettableUser?>? callGetUser = null;

    IPettablePet? currentActivePet;

    public void RegsterTarget(Func<IPettablePet?> getPet, Func<IPettableUser?>? callGetUser = null)
    {
        this.callGetUser = callGetUser;
        callGetPet = getPet;
        SetUnfaulty();
    }

    protected override bool OnTextNode(AtkTextNode* textNode, string text)
    {
        if (!IsSoft) return NotSoftTextNode(textNode, text);
        return base.OnTextNode(textNode, text);
    }

    bool NotSoftTextNode(AtkTextNode* textNode, string text)
    {
        currentActivePet = callGetPet?.Invoke();
        if (currentActivePet == null) return false; 

        IPetSheetData? petData = currentActivePet.PetData;
        if (petData == null) return false;

        string? customName = currentActivePet.CustomName;
        if (customName == null) return false;

        SetText(textNode, text, customName, petData);
        return true;
    }

    protected override void GetColours(out Vector3? edgeColour, out Vector3? textColour)
    {
        edgeColour = null;
        textColour = null;

        currentActivePet?.GetDrawColours(out edgeColour, out textColour);
    }

    protected override IPettableUser? GetUser() => callGetUser?.Invoke();
}
