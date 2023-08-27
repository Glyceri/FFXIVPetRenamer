using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using System.Runtime.InteropServices;
using System;
using FFCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Core.Updatable.Updatables;
using PetRenamer.Core.Serialization;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using System.Text.RegularExpressions;

namespace PetRenamer.Core.Chat.ChatElements;

//[Chat]
internal unsafe class PetChatEmoteElement : ChatElement
{
    internal override void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PluginLink.Configuration.replaceEmotes || !PluginLink.Configuration.displayCustomNames) return;
        if (type != XivChatType.StandardEmote && type != XivChatType.CustomEmote) return;
        BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(sender.ToString(), true);
        if (bChara == null) return;
        ulong target = bChara->Character.GetTargetId();
        uint softTarget = bChara->Character.PlayerTargetObjectID;
        if (softTarget != 0)
            target = softTarget;

        string nameString = string.Empty;
        int id = -1;
        string ownerName = string.Empty;

        FFCharacter* lookedUpChar2 = (FFCharacter*)PluginLink.CharacterManager->LookupBattleCharaByObjectId((int)target);
        if (lookedUpChar2 == null) return;
        GameObject* gObj = (GameObject*)lookedUpChar2->Companion.CompanionObject;
        if (gObj != null)
        {
            nameString = Marshal.PtrToStringUTF8((IntPtr)gObj->Name) ?? string.Empty;
            id = lookedUpChar2->Companion.CompanionObject->Character.CharacterData.ModelSkeletonId;
            ownerName = Marshal.PtrToStringUTF8((IntPtr)lookedUpChar2->GameObject.Name)!;
        }
        else
        {
            nameString = Marshal.PtrToStringUTF8((IntPtr)lookedUpChar2->GameObject.Name) ?? string.Empty;
            BattleChara* chara = PluginLink.CharacterManager!->LookupBattleCharaByObjectId((int)lookedUpChar2->GameObject!.OwnerID!);
            id = RemapUtils.instance.GetPetIDFromClass(chara!->Character.CharacterData.ClassJob!);
            ownerName = Marshal.PtrToStringUTF8((IntPtr)chara->Character.GameObject.Name)!;
        }

        if (ownerName == string.Empty || id == -1 || nameString == string.Empty) return;

        foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters)
        {
            if (character.ownName != ownerName) continue;

            SerializableNickname nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, id);
            if (nickname == null) continue;
            if (!nickname.Valid()) continue;
            for (int i = 0; i < message.Payloads.Count; i++)
            {
                if (message.Payloads[i] is TextPayload tPayload)
                {
                    foreach (string str in PluginConstants.removeables)
                        tPayload.Text = Regex.Replace(tPayload.Text!, str + nameString, nickname.Name, RegexOptions.IgnoreCase);
                    message.Payloads[i] = tPayload;
                }
            }
            break;
        }
    }
}
