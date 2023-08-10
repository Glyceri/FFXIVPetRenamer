﻿using Dalamud.Game;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Updatable.Updatables;
using PetRenamer.Utilization.UtilsModule;
using System;
using FFCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal unsafe class PartyListHook : HookableElement
{
    
    internal override void OnUpdate(Framework framework)
    {
        if (PluginHandlers.ClientState.LocalPlayer == null) return;
        HandleTargetBar();
        HandleFocusBar();
        //HandlePartyList();
    }
    
    void HandlePartyList()
    {
        try
        {
            Dalamud.Game.ClientState.Objects.Enums.ObjectKind targetObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc;
            BaseNode resNode = new BaseNode("_PartyList");
            if (resNode == null) return;
            ComponentNode baseComponentNode = resNode.GetComponentNode(19);
            if (baseComponentNode == null) return;
            AtkTextNode* textNode = baseComponentNode.GetNode<AtkTextNode>(14);
            if (textNode == null) return;
            textNode->AtkResNode.ToggleVisibility(false);
            GameObject* gObj = &PluginLink.CharacterManager->LookupPetByOwnerObject((BattleChara*)GameObjectManager.GetGameObjectByIndex(PluginHandlers.ClientState.LocalPlayer!.ObjectIndex))->Character.GameObject;
            SetNicknameForGameObject(ref textNode, ref gObj, targetObjectKind);
        }
        catch (Exception ex) { PluginLog.Log(ex.ToString()); }
    }


    void HandleTargetBar()
    {
        try
        {
            if (PluginHandlers.TargetManager.Target == null) return;
            Dalamud.Game.ClientState.Objects.Enums.ObjectKind targetObjectKind = PluginHandlers.TargetManager.Target.ObjectKind;
            BaseNode resNode = new BaseNode("_TargetInfoMainTarget");
            if (resNode == null) return;
            AtkTextNode* textNode = resNode.GetNode<AtkTextNode>(10);
            if(textNode == null) return;
            if (PluginHandlers.TargetManager.Target.Name.ToString() != textNode->NodeText.ToString()) return;
            GameObject* gObj = GameObjectManager.GetGameObjectByIndex(PluginHandlers.TargetManager.Target.ObjectIndex);
            SetNicknameForGameObject(ref textNode, ref gObj, targetObjectKind);

            AtkTextNode* textNode2 = resNode.GetNode<AtkTextNode>(7);
            if (textNode2 == null) return;
            if (PluginHandlers.TargetManager.Target.Name.ToString() != textNode2->NodeText.ToString()) return;
            GameObject* gObj2 = GameObjectManager.GetGameObjectByIndex(PluginHandlers.TargetManager.Target.ObjectIndex);
            SetNicknameForGameObject(ref textNode2, ref gObj2, targetObjectKind);
        }
        catch (Exception ex) { PluginLog.Log(ex.ToString()); }
    }

    void HandleFocusBar()
    {
        try
        {
            if (PluginHandlers.TargetManager.FocusTarget == null) return;
            Dalamud.Game.ClientState.Objects.Enums.ObjectKind targetObjectKind = PluginHandlers.TargetManager.FocusTarget.ObjectKind;
            BaseNode resNode = new BaseNode("_FocusTargetInfo");
            if (resNode == null) return;
            AtkTextNode* textNode = resNode.GetNode<AtkTextNode>(10);
            if (textNode == null) return;
            if (!textNode->NodeText.ToString().Contains(PluginHandlers.TargetManager.FocusTarget.Name.ToString())) return;
            GameObject* gObj = GameObjectManager.GetGameObjectByIndex(PluginHandlers.TargetManager.FocusTarget.ObjectIndex);
            SetNicknameForGameObject(ref textNode, ref gObj, targetObjectKind);
        }
        catch (Exception ex) { PluginLog.Log(ex.ToString()); }
    }


    void SetNicknameForGameObject(ref AtkTextNode* textNode, ref GameObject* gObj, Dalamud.Game.ClientState.Objects.Enums.ObjectKind targetObjectKind)
    {
        foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters)
        {
            int curID = -1;
            if (targetObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Companion)
            {
                if (!character.HasCompanion()) continue;
                if (character.GetCompanionID() != ((FFCharacter*)gObj)->CharacterData.ModelSkeletonId) continue;
                curID = character.GetCompanionID();
            }
            else if (targetObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc)
            {
                if (!character.HasBattlePet()) continue;
                if (!RemapUtils.instance.battlePetRemap.ContainsKey(((BattleChara*)gObj)->Character.CharacterData.ModelCharaId)) continue;
                if (character.GetBattlePetObjectID() != ((BattleChara*)gObj)->Character.GameObject.ObjectID) continue;
                curID = (int)character.GetBattlePetID();
            }
            if (curID == -1) continue;
            SetNickname(curID, character, ref textNode);
        }
    }

    void SetNickname(int id, FoundPlayerCharacter character, ref AtkTextNode* textNode)
    {
        SerializableNickname? nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, id);
        if (nickname == null) return;
        if (nickname.Name == string.Empty) return;
        textNode->NodeText.SetString(nickname.Name);
    }
}
