using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal unsafe class TargetTextHook : SimpleTextHook
{
    Func<IPettablePet?>? callGetPet = null;
    Func<IPettableUser?>? callGetUser = null;

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
        IPettablePet? pet = callGetPet?.Invoke();
        if (pet == null) return false; 

        IPetSheetData? petData = pet.PetData;
        if (petData == null) return false;

        string? customName = pet.CustomName;
        if (customName == null) return false;

        SetText(textNode, text, customName, petData);
        return true;
    }

    protected override IPettableUser? GetUser() => callGetUser?.Invoke();
}
