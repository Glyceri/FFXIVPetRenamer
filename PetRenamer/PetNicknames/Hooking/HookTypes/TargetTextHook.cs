using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal class TargetTextHook : SimpleTextHook
{
    Func<IPettablePet?>? callGetUser = null;

    public void RegsterTarget(Func<IPettablePet?> getPet)
    {
        callGetUser = getPet;
        SetUnfaulty();
    }

    protected override unsafe bool OnTextNode(AtkTextNode* textNode, string text)
    {
        IPettablePet? pet = callGetUser?.Invoke();
        if (pet == null) return false;

        PetSheetData? petData = pet.PetData;
        if (petData == null) return false;

        string? customName = pet.CustomName;
        if (customName == null) return false;

        SetText(textNode, text, customName, petData.Value);
        return true;
    }
}
