using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace PetRenamer.Core.Hooking.Hooks;

// Signatures from: https://github.com/Tischel/ActionTimeline/blob/master/ActionTimeline/Helpers/TimelineManager.cs
// Signatures from: https://github.com/cairthenn/Redirect/blob/main/Redirect/GameHooks.cs
// and from https://github.com/Kouzukii/ffxiv-deathrecap/blob/master/Events/CombatEventCapture.cs
// I store these so when they inevitably change, I can just yoink them again from there.

[Hook]
internal class FlyTextHook : HookableElement
{
    [Signature("E8 ?? ?? ?? ?? 8B 8C 24 ?? ?? ?? ?? 85 C9 ", DetourName = nameof(AddToScreenLogWithLogMessageIdDetour))]
    Hook<Delegates.AddToScreenLogWithLogMessageId>? addToScreenLogWithLogMessageId = null;


    unsafe internal override void OnInit()
    {
        PluginHandlers.FlyTextGui.FlyTextCreated += OnFlyTextCreated;

        addToScreenLogWithLogMessageId?.Enable();
    }

    void OnFlyTextCreated(ref FlyTextKind kind, ref int val1, ref int val2, ref SeString text1, ref SeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {
        if (!PluginLink.Configuration.useCustomFlyoutInChat) return;

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        if (!user.UserExists) return;
        LastActionUsed lastActionUsed = PluginLink.PettableUserHandler.LastCast;

        foreach (PettableUser user1 in PluginLink.PettableUserHandler.Users)
        {
            if (!user1.UserExists) continue;
            if (user1.nintUser != lastActionUsed.castDealer) continue;
            user = user1;
            break;
        }

        if (user == null) return;
        if (!user.UserExists) return;

        List<(string, string)> validNames = new List<(string, string)>();
        foreach (int skelID in RemapUtils.instance.battlePetRemap.Keys)
        {
            string bPetname = SheetUtils.instance.GetBattlePetName(skelID) ?? string.Empty;
            if (bPetname == string.Empty) continue;
            if (!text1.ToString().Contains(bPetname) && !text2.ToString().Contains(bPetname)) continue;
            if (!RemapUtils.instance.skeletonToClass.ContainsKey(skelID)) continue;
            int jobID = RemapUtils.instance.skeletonToClass[skelID];
            string cName = user.SerializableUser.GetNameFor(jobID) ?? string.Empty;
            if (cName == string.Empty) continue;
            validNames.Add((bPetname, cName));
        }
        if (validNames.Count == 0) return;
        validNames.Sort((el1, el2) => el1.Item1.Length.CompareTo(el2.Item1.Length));

        for (int i = 0; i < text1.Payloads.Count; i++)
        {
            if (text1.Payloads[i] is not TextPayload tPayload) continue;

            foreach ((string, string) element in validNames)
            {
                if (element.Item1 == string.Empty || element.Item2 == string.Empty) continue;
                tPayload.Text = Regex.Replace(tPayload.Text!, element.Item1, element.Item2, RegexOptions.IgnoreCase);
            }
            text1.Payloads[i] = tPayload;
        }

        for (int i = 0; i < text2.Payloads.Count; i++)
        {
            if (text2.Payloads[i] is not TextPayload tPayload) continue;

            foreach ((string, string) element in validNames)
            {
                if (element.Item1 == string.Empty || element.Item2 == string.Empty) continue;
                tPayload.Text = Regex.Replace(tPayload.Text!, element.Item1, element.Item2, RegexOptions.IgnoreCase);
            }
            text2.Payloads[i] = tPayload;
        }
    }

    unsafe void AddToScreenLogWithLogMessageIdDetour(IntPtr castBoss, IntPtr castDealer, int a3, char a4, int a5, int a6, int a7, int a8)
    {
        addToScreenLogWithLogMessageId?.Original(castBoss, castDealer, a3, a4, a5, a6, a7, a8);
        PluginLink.PettableUserHandler.SetLastCast(castBoss, castDealer);
    }

    internal override void OnDispose()
    {
        PluginHandlers.FlyTextGui.FlyTextCreated -= OnFlyTextCreated;

        addToScreenLogWithLogMessageId?.Dispose();
    }
}