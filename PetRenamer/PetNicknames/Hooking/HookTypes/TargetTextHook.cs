using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using System;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal unsafe class TargetTextHook : SimpleTextHook
{
    private Func<IPettableEntity?>? callGetPet = null;

    private IPettablePet? currentActivePet;

    public TargetTextHook RegsterTarget(Func<IPettableEntity?> getPet)
    {
        callGetPet = getPet;

        SetUnfaulty();

        return this;
    }

    protected override bool OnTextNode(AtkTextNode* textNode, string text)
    {
        if (!IsSoft) return NotSoftTextNode(textNode, text);

        return base.OnTextNode(textNode, text);
    }

    private bool NotSoftTextNode(AtkTextNode* textNode, string text)
    {
        IPettableEntity? currentEntity = callGetPet?.Invoke();
        if (currentEntity == null) return false;
        if (currentEntity is not IPettablePet pet) return false;

        currentActivePet = pet;
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

    protected override IPettableUser? GetUser() => null;
}
