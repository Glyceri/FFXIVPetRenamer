using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using FFCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using TargetObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Dalamud.Logging;
using Dalamud.Memory;
using System;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal unsafe class TargetBarHooking : HookableElement
{
    private Hook<Delegates.AddonUpdate>? addonupdatehook = null;
    private Hook<Delegates.AddonUpdate>? addonupdatehook2 = null;

    AtkUnitBase* baseElement;
    AtkUnitBase* baseElement2;

    internal override void OnUpdate(Framework framework)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (PluginHandlers.ClientState.LocalPlayer == null) return;
        HandleTargetElements();

        baseElement = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName("_TargetInfoMainTarget");

        addonupdatehook ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(baseElement->AtkEventListener.vfunc[42]), Update);
        addonupdatehook?.Enable();

        baseElement2 = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName("_FocusTargetInfo");

        addonupdatehook2 ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(baseElement2->AtkEventListener.vfunc[42]), Update2);
        addonupdatehook2?.Enable();
    }

    DGameObject target = null!;
    DGameObject targetOfTarget = null!;
    DGameObject focusTarget = null!;

    void HandleTargetElements()
    {
        target = PluginHandlers.TargetManager.Target!;
        if (PluginHandlers.TargetManager.SoftTarget != null) target = PluginHandlers.TargetManager.SoftTarget;
        if (target != null) targetOfTarget = target.TargetObject!;
        focusTarget = PluginHandlers.TargetManager.FocusTarget!;
    }

    byte Update(AtkUnitBase* baseD)
    {
        HandleTargetBar();
        HandleTargetOfTargetBar();
        return addonupdatehook!.Original(baseD);
    }

    byte Update2(AtkUnitBase* baseD)
    {
        HandleFocusBar();
        return addonupdatehook2!.Original(baseD);
    }

    void HandleFocusBar()
    {
        if (focusTarget == null) return;
        try
        {
            TargetObjectKind targetObjectKind = focusTarget.ObjectKind;
            BaseNode resNode = new BaseNode("_FocusTargetInfo");
            if (resNode == null) return;
            AtkTextNode* textNode = resNode.GetNode<AtkTextNode>(10);
            if (textNode == null) return;
            if (!textNode->NodeText.ToString().Contains(focusTarget.Name.ToString())) return;
            GameObject* gObj = GameObjectManager.GetGameObjectByIndex(focusTarget.ObjectIndex);
            SetFor(textNode, gObj);
        }
        catch (Exception ex) { PluginLog.Log(ex.ToString()); }
    }

    void HandleTargetOfTargetBar()
    {
        if (target == null || targetOfTarget == null) return;
        try
        {
            TargetObjectKind targetObjectKind = targetOfTarget.ObjectKind;
            BaseNode resNode = new BaseNode("_TargetInfoMainTarget");
            if (resNode == null) return;
            AtkTextNode* textNode2 = resNode.GetNode<AtkTextNode>(7);
            if (textNode2 == null) return;
            string nameString = targetOfTarget.Name.ToString();
            int index = targetOfTarget.ObjectIndex;

            if (targetObjectKind == TargetObjectKind.Player)
            {
                ulong targetID2 = PluginLink.CharacterManager->LookupBattleCharaByObjectId((int)target.ObjectId)->Character.GetTargetId();
                if (!targetID2.ToString("X").StartsWith("4")) return;

                targetObjectKind = TargetObjectKind.Companion;

                FFCharacter* lookedUpChar2 = (FFCharacter*)PluginLink.CharacterManager->LookupBattleCharaByObjectId((int)targetID2);
                GameObject* gObj = (GameObject*)lookedUpChar2->Companion.CompanionObject;
                nameString = MemoryHelper.ReadSeString((nint)gObj->Name, 64).ToString();
                index = gObj->ObjectIndex;
            }

            if (!textNode2->NodeText.ToString().Contains(nameString)) return;
            GameObject* gObj2 = GameObjectManager.GetGameObjectByIndex(index);
            SetFor(textNode2, gObj2);
        }
        catch (Exception ex) { PluginLog.Log(ex.ToString()); }
    }

    void HandleTargetBar()
    {
        if (target == null) return;
        TargetObjectKind targetObjectKind = target.ObjectKind;
        BaseNode resNode = new BaseNode("_TargetInfoMainTarget");
        if (resNode == null) return;
        AtkTextNode* textNode = resNode.GetNode<AtkTextNode>(10);
        if (textNode == null) return;
        if (target.Name.ToString() != textNode->NodeText.ToString()) return;
        GameObject* gObj = GameObjectManager.GetGameObjectByIndex(target.ObjectIndex);
        SetFor(textNode, gObj);
    }

    void SetFor(AtkTextNode* textNode, GameObject* gObj)
    {
        PluginLink.PettableUserHandler.LoopThroughBreakable(user =>
        {
            if (user.nintCompanion == (nint)gObj)
            {
                if (user.CustomCompanionName != string.Empty)
                    textNode->NodeText.SetString(user.CustomCompanionName);
                return true;
            }
            if (user.nintBattlePet == (nint)gObj)
            {
                if (user.BattlePetCustomName != string.Empty)
                    textNode->NodeText.SetString(user.BattlePetCustomName);
                return true;
            }
            return false;
        });
    }

    internal override void OnDispose()
    {
        addonupdatehook?.Disable();
        addonupdatehook?.Dispose();

        addonupdatehook2?.Disable();
        addonupdatehook2?.Dispose();
    }
}
