using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
internal unsafe class BattleChatElement : ChatElement
{
    internal override void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        PluginLog.Log($"On Chat: {type} : {senderId} : {sender} : {isHandled} : {message.ToString()}");
        if (!PluginLink.Configuration.useCustomNamesInChat) return;
        if (Enum.IsDefined(typeof(XivChatType), type) && type != XivChatType.SystemMessage) return;


        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        if (!user.UserExists) return;
        LastActionUsed lastActionUsed = PluginLink.PettableUserHandler.LastCast;

        //if (type != (XivChatType)2219 && type != (XivChatType)2105 && type != (XivChatType)2106 && type != XivChatType.SystemMessage)
        {
            if (PluginLink.PettableUserHandler.lastActionType == LastActionType.ActionUsed)
                lastActionUsed = PluginLink.PettableUserHandler.LastUsedAction;
            //else lastActionUsed = PluginLink.PettableUserHandler.LastCast;

            if (!message.ToString().Contains(lastActionUsed.actionName ?? string.Empty)) return;

            foreach (PettableUser user1 in PluginLink.PettableUserHandler.Users)
            {
                if (!user1.UserExists) continue;
                if (user1.User->Character.GameObject.ObjectID != lastActionUsed.userID) continue;

                user = user1;
                break;
            }
        }

        if (user == null) return;
        if (!user.UserExists) return;

        List<(string, string)> validNames = new List<(string, string)>();
        foreach (int skelID in RemapUtils.instance.battlePetRemap.Keys)
        {
            string bPetname = SheetUtils.instance.GetBattlePetName(skelID) ?? string.Empty;
            if (bPetname == string.Empty) continue;
            if (!message.ToString().Contains(bPetname)) continue;
            if (!RemapUtils.instance.skeletonToClass.ContainsKey(skelID)) continue;
            int jobID = RemapUtils.instance.skeletonToClass[skelID];
            string cName = user.SerializableUser.GetNameFor(jobID) ?? string.Empty;
            if (cName == string.Empty) continue;
            validNames.Add((bPetname, cName));
        }
        if (validNames.Count == 0) return;
        validNames.Sort((el1, el2) => el1.Item1.Length.CompareTo(el2.Item1.Length));

        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload) continue;

            foreach ((string, string) element in validNames)
            {
                if (element.Item1 == string.Empty || element.Item2 == string.Empty) continue;
                tPayload.Text = Regex.Replace(tPayload.Text!, element.Item1, element.Item2, RegexOptions.IgnoreCase);
            }
            message.Payloads[i] = tPayload;
        }
    }
}
