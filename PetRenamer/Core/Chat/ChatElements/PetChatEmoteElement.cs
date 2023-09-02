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
using Dalamud.Game.Text.SeStringHandling.Payloads;
using System.Text.RegularExpressions;
using PetRenamer.Core.PettableUserSystem;
using Dalamud.Logging;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
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
            if (!RemapUtils.instance.skeletonToClass.ContainsKey(lookedUpChar2->CharacterData.ModelCharaId)) return;
            nameString = Marshal.PtrToStringUTF8((IntPtr)lookedUpChar2->GameObject.Name) ?? string.Empty;
            BattleChara* chara = PluginLink.CharacterManager!->LookupBattleCharaByObjectId((int)lookedUpChar2->GameObject!.OwnerID!);
            if (chara == null) return;
            id = RemapUtils.instance.GetPetIDFromClass(chara!->Character.CharacterData.ClassJob!);
            ownerName = Marshal.PtrToStringUTF8((IntPtr)chara->Character.GameObject.Name)!;
        }

        if (ownerName == string.Empty || id == -1 || nameString == string.Empty) return;

        string nickname = string.Empty;
        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            if (!user.UserExists) continue;
            if (user.UserName.ToLower().Normalize() != ownerName.ToLower().Normalize()) continue;

            user.SerializableUser.LoopThroughBreakable(n =>
            {
                if (n.Item1 == id)
                {
                    nickname = n.Item2;
                    return true;
                }
                return false;
            });
            break;
        }

        if (nickname == string.Empty) return;

        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload) continue;

            foreach (string str in PluginConstants.removeables)
                tPayload.Text = Regex.Replace(tPayload.Text!, str + nameString, nickname, RegexOptions.IgnoreCase);
            message.Payloads[i] = tPayload;
        }
    }
}
