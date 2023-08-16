using Dalamud.Game;
using Dalamud.Logging;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System;
using FFCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using TargetObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal unsafe class TargetBarUpdatable : Updatable
{
    DGameObject target = null!;
    DGameObject targetOfTarget = null!;
    DGameObject focusTarget = null!;

    public override void Update(Framework frameWork)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (PluginHandlers.ClientState.LocalPlayer == null) return;
        HandleTargetElements();
        HandleTargetBar();
        HandleTargetOfTargetBar();
        HandleFocusBar();
    }

    void HandleTargetElements()
    {
        target = PluginHandlers.TargetManager.Target!;
        if (PluginHandlers.TargetManager.SoftTarget != null) target = PluginHandlers.TargetManager.SoftTarget;
        if (target != null) targetOfTarget = target.TargetObject!;
        focusTarget = PluginHandlers.TargetManager.FocusTarget!;
    }

    void HandleTargetBar()
    {
        if (target == null) return;
        try
        {
            TargetObjectKind targetObjectKind = target.ObjectKind;
            BaseNode resNode = new BaseNode("_TargetInfoMainTarget");
            if (resNode == null) return;
            AtkTextNode* textNode = resNode.GetNode<AtkTextNode>(10);
            if (textNode == null) return;
            if (target.Name.ToString() != textNode->NodeText.ToString()) return;
            GameObject* gObj = GameObjectManager.GetGameObjectByIndex(target.ObjectIndex);
            SerializableNickname name = NicknameUtils.instance.GetFromGameObjectPtr(gObj, EnumUtils.instance.FromTargetType(targetObjectKind));
            if (name?.Valid() ?? false)
                textNode->NodeText.SetString(name.Name);
        }
        catch (Exception ex) { PluginLog.Log(ex.ToString()); }
    }

    void HandleTargetOfTargetBar()
    {
        if (target == null) return;
        if (targetOfTarget == null) return;
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
            SerializableNickname name = NicknameUtils.instance.GetFromGameObjectPtr(gObj2, EnumUtils.instance.FromTargetType(targetObjectKind));
            if (name?.Valid() ?? false)
                textNode2->NodeText.SetString(name.Name);
        }
        catch (Exception ex) { PluginLog.Log(ex.ToString()); }
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
            SerializableNickname name = NicknameUtils.instance.GetFromGameObjectPtr(gObj, EnumUtils.instance.FromTargetType(targetObjectKind));
            if (name?.Valid() ?? false)
                textNode->NodeText.SetString(name.Name);
        }
        catch (Exception ex) { PluginLog.Log(ex.ToString()); }
    }
}
