using Dalamud.Game;
using Dalamud.Hooking;
using FFCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Updatable.Updatables;
using PetRenamer.Utilization.UtilsModule;
using Dalamud.Logging;
using System;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal unsafe class PartyListHook : HookableElement
{
    Hook<Delegates.AddonUpdate> onDrawHook = null!;
    Hook<Delegates.AddonUpdate> onDrawHook2 = null!;
    Hook<Delegates.AddonUpdate> onDrawHook3 = null!;
    AddonPartyList* partyList;
    AtkUnitBase* ui;
    AtkUnitBase* ui2;

    internal override void OnUpdate(Framework framework)
    {
        if (PluginHandlers.ClientState.LocalPlayer == null) return; 
        partyList = (AddonPartyList*)PluginHandlers.GameGui.GetAddonByName("_PartyList");
        ui = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName("_TargetInfoMainTarget");
        ui2 = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName("_FocusTargetInfo");

        if (onDrawHook == null && partyList != null)
        {
            onDrawHook ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(partyList->AtkUnitBase.AtkEventListener.vfunc[41]), OnUpdate);
            onDrawHook?.Enable();
        }

        if (onDrawHook2 == null && ui != null)
        {
            onDrawHook2 ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(ui->AtkEventListener.vfunc[41]), OnUpdate2);
            onDrawHook2?.Enable();
        }

        if (onDrawHook3 == null && ui != null)
        {
            onDrawHook3 ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(ui2->AtkEventListener.vfunc[41]), OnUpdate3);
            onDrawHook3?.Enable();
        }
    }

    byte OnUpdate3(AtkUnitBase* addon)
    {
        if (PluginHandlers.TargetManager.FocusTarget == null) return onDrawHook3!.Original(addon);
        Dalamud.Game.ClientState.Objects.Enums.ObjectKind targetObjectKind = PluginHandlers.TargetManager.FocusTarget.ObjectKind;
        if (targetObjectKind != Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Companion && targetObjectKind != Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc) return onDrawHook3!.Original(addon);


        try
        {
            if (addon == null) return onDrawHook3!.Original(addon);
            if (addon->UldManager.NodeList == null) return onDrawHook3!.Original(addon);
            if (addon->UldManager.NodeListCount < 11) return onDrawHook3!.Original(addon);

            AtkTextNode* textNode = (AtkTextNode*)addon->UldManager.NodeList[10];
            if (textNode->FontSize == 0) return onDrawHook3!.Original(addon);
            if (textNode->SelectStart != 0) return onDrawHook3!.Original(addon);

            if (textNode->NodeText.ToString().Contains(PluginHandlers.TargetManager.FocusTarget.Name.ToString()))
            {
               
                GameObject* gObj = GameObjectManager.GetGameObjectByIndex(PluginHandlers.TargetManager.FocusTarget.ObjectIndex);
                foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters)
                {
                    if (targetObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Companion)
                    {
                        if (!character.HasCompanion()) continue;
                        if (character.GetCompanionID() == ((FFCharacter*)gObj)->CharacterData.ModelSkeletonId)
                        {
                            SerializableNickname? nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, character.GetCompanionID());
                            if (nickname == null) continue;
                            if (nickname.Name == string.Empty) continue;
                            textNode->SetText(nickname.Name);
                        }
                    }
                    else
                    {
                        if (!character.HasBattlePet()) continue;
                        if (!RemapUtils.instance.battlePetRemap.ContainsKey(((FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)gObj)->Character.CharacterData.ModelCharaId)) return onDrawHook3!.Original(addon);
                        if (character.GetBattlePetObjectID() == ((FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)gObj)->Character.GameObject.ObjectID)
                        {
                            SerializableNickname? nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, character.GetBattlePetID());
                            if (nickname == null) continue;
                            if (nickname.Name == string.Empty) continue;
                            textNode->SetText(nickname.Name);
                        }
                    }
                }
            }
            return onDrawHook3!.Original(addon);
        }
        catch (Exception e)
        {
            PluginLog.Log(e.ToString());
            return onDrawHook3!.Original(addon);
        }
    }

    byte OnUpdate2(AtkUnitBase* addon)
    {
        if (PluginHandlers.TargetManager.Target == null) return onDrawHook2!.Original(addon);
        Dalamud.Game.ClientState.Objects.Enums.ObjectKind targetObjectKind = PluginHandlers.TargetManager.Target.ObjectKind;

        try
        {
            if (addon == null) return onDrawHook2!.Original(addon);
            if (addon->UldManager.NodeList == null) return onDrawHook2!.Original(addon);
            if (addon->UldManager.NodeListCount < 9) return onDrawHook2!.Original(addon);

            var gaugeBar = (AtkComponentNode*)addon->UldManager.NodeList[5];

            var textNode = (AtkTextNode*)addon->UldManager.NodeList[8];

            if (textNode->FontSize == 0) return onDrawHook2!.Original(addon);
            if (textNode->SelectStart != 0) return onDrawHook2!.Original(addon);

            if (PluginHandlers.TargetManager.Target.Name.ToString() == textNode->NodeText.ToString())
            {
                GameObject* gObj = GameObjectManager.GetGameObjectByIndex(PluginHandlers.TargetManager.Target.ObjectIndex);
                foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters)
                {
                    if (targetObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Companion)
                    {
                        if (!character.HasCompanion()) continue;
                        if (character.GetCompanionID() == ((FFCharacter*)gObj)->CharacterData.ModelSkeletonId)
                        {
                            SerializableNickname? nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, character.GetCompanionID());
                            if (nickname == null) continue;
                            if (nickname.Name == string.Empty) continue;
                            textNode->NodeText.SetString(nickname.Name);
                        }
                    }
                    else if (targetObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc)
                    {
                        if (!character.HasBattlePet()) continue;
                        if (!RemapUtils.instance.battlePetRemap.ContainsKey(((FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)gObj)->Character.CharacterData.ModelCharaId)) return onDrawHook2!.Original(addon);
                        if (character.GetBattlePetObjectID() == ((FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)gObj)->Character.GameObject.ObjectID)
                        {
                            SerializableNickname? nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, character.GetBattlePetID());
                            if (nickname == null) continue;
                            if (nickname.Name == string.Empty) continue;
                            textNode->NodeText.SetString(nickname.Name);
                        }
                    }
                }
            }



            if (PluginHandlers.TargetManager.Target == null) return onDrawHook2!.Original(addon);
            if (PluginHandlers.TargetManager.Target.TargetObject == null) return onDrawHook2!.Original(addon);
            Dalamud.Game.ClientState.Objects.Enums.ObjectKind targetObjectKind2 = PluginHandlers.TargetManager.Target.TargetObject.ObjectKind;

            if (addon->UldManager.NodeListCount < 13) return onDrawHook2!.Original(addon);
            AtkTextNode* textNode2 = (AtkTextNode*)addon->UldManager.NodeList[12];

            if (textNode2->FontSize == 0) return onDrawHook2!.Original(addon);
            if (textNode2->SelectStart != 0) return onDrawHook2!.Original(addon);

            if (textNode2->NodeText.ToString().Contains(PluginHandlers.TargetManager.Target.TargetObject.Name.ToString()))
            {
                GameObject* gObj = GameObjectManager.GetGameObjectByIndex(PluginHandlers.TargetManager.Target.TargetObject.ObjectIndex);
                foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters)
                {
                    if (targetObjectKind2 == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Companion)
                    {
                        if (!character.HasCompanion()) continue;
                        if (character.GetCompanionID() == ((FFCharacter*)gObj)->CharacterData.ModelSkeletonId)
                        {
                            SerializableNickname? nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, character.GetCompanionID());
                            if (nickname == null) continue;
                            if (nickname.Name == string.Empty) continue;
                            textNode2->NodeText.SetString(nickname.Name);
                        }
                    }
                    else if (targetObjectKind2 == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc)
                    {
                        if (!character.HasBattlePet()) continue;
                        if (!RemapUtils.instance.battlePetRemap.ContainsKey(((FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)gObj)->Character.CharacterData.ModelCharaId)) return onDrawHook2!.Original(addon);
                        if (character.GetBattlePetObjectID() == ((FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)gObj)->Character.GameObject.ObjectID)
                        {
                            SerializableNickname? nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, character.GetBattlePetID());
                            if (nickname == null) continue;
                            if (nickname.Name == string.Empty) continue;
                            textNode2->NodeText.SetString(nickname.Name);
                        }
                    }
                }
            }
            return onDrawHook2!.Original(addon);
        }
        catch(Exception e) {
            PluginLog.Log(e.Message);
            return onDrawHook2!.Original(addon); 
        }
    }

    byte OnUpdate(AtkUnitBase* addon)
    {
        if (PluginHandlers.ClientState.LocalPlayer == null) return onDrawHook!.Original(addon);
        AddonPartyList* partyList = (AddonPartyList*)addon;
        SerializableNickname? nickname = NicknameUtils.instance.GetLocalNicknameV2(RemapUtils.instance.GetPetIDFromClass((int)PluginHandlers.ClientState.LocalPlayer!.ClassJob.Id));
        if (nickname == null) return onDrawHook!.Original(addon);
        if (nickname.Name.Length == 0) return onDrawHook!.Original(addon);
        if (partyList->PetCount >= 1)
            partyList->Pet.Name->SetText(nickname.Name);
        return onDrawHook!.Original(addon);
    }


    internal override void OnDispose()
    {
        onDrawHook?.Disable();
        onDrawHook?.Dispose();
        onDrawHook = null!;

        onDrawHook2?.Disable();
        onDrawHook2?.Dispose();
        onDrawHook2 = null!;

        onDrawHook3?.Disable();
        onDrawHook3?.Dispose();
        onDrawHook3 = null!;
    }
}
