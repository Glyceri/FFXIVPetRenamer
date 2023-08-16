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
    public override void Update(Framework frameWork)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (PluginHandlers.ClientState.LocalPlayer == null) return;
        HandleTargetBar();
        HandleTargetOfTargetBar();
        HandleFocusBar();
    }

    void HandleTargetBar()
    {
        try
        {
            DGameObject target = PluginHandlers.TargetManager.Target!;
            if(PluginHandlers.TargetManager.SoftTarget != null)
                target = PluginHandlers.TargetManager.SoftTarget;
            if (target == null) return;
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
        try
        {
            if (PluginHandlers.TargetManager.Target == null) return;
            if (PluginHandlers.TargetManager.Target.TargetObject == null) return;
            DGameObject? targetObject = PluginHandlers.TargetManager.Target;
            DGameObject? targetOfTargetObject = PluginHandlers.TargetManager.Target.TargetObject;
            TargetObjectKind targetObjectKind = targetOfTargetObject.ObjectKind;
            BaseNode resNode = new BaseNode("_TargetInfoMainTarget");
            if (resNode == null) return;
            AtkTextNode* textNode2 = resNode.GetNode<AtkTextNode>(7);
            if (textNode2 == null) return;
            string nameString = targetOfTargetObject.Name.ToString();
            int index = targetOfTargetObject.ObjectIndex;

            if (targetObjectKind == TargetObjectKind.Player)
            {
                ulong targetID2 = PluginLink.CharacterManager->LookupBattleCharaByObjectId((int)targetObject.ObjectId)->Character.GetTargetId();
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
        try
        {
            if (PluginHandlers.TargetManager.FocusTarget == null) return;
            TargetObjectKind targetObjectKind = PluginHandlers.TargetManager.FocusTarget.ObjectKind;
            BaseNode resNode = new BaseNode("_FocusTargetInfo");
            if (resNode == null) return;
            AtkTextNode* textNode = resNode.GetNode<AtkTextNode>(10);
            if (textNode == null) return;
            if (!textNode->NodeText.ToString().Contains(PluginHandlers.TargetManager.FocusTarget.Name.ToString())) return;
            GameObject* gObj = GameObjectManager.GetGameObjectByIndex(PluginHandlers.TargetManager.FocusTarget.ObjectIndex);
            SerializableNickname name = NicknameUtils.instance.GetFromGameObjectPtr(gObj, EnumUtils.instance.FromTargetType(targetObjectKind));
            if (name?.Valid() ?? false)
                textNode->NodeText.SetString(name.Name);
        }
        catch (Exception ex) { PluginLog.Log(ex.ToString()); }
    }
}
