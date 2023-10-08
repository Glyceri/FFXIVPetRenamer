﻿using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text.Payloads;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PetRenamer.Core.Chat.ChatElements;

// This check is literally here to catch pet glamour changes 1... and I literally mean 1 step early.
[Chat]
internal class PetGlamourChat : ChatElement
{
    //The next Carbuncle summoned will appear glamoured as Ruby Carbuncle.
    //The next Carbuncle summoned will appear unglamoured.

    //次に召喚する「カーバンクル」の姿を「カーバンクル・エメラルド」に変更しました。 (Changed petglam)
    //次に召喚する「カーバンクル」の姿を元に戻しました。                         (Reset petglam)

    //Karfunkel wird nächstes Mal als Smaragd-Karfunkel erscheinen.
    //Karfunkel wird nächstes Mal unverwandelt erscheinen.

    //Carbuncle aura l'apparence de Carbuncle émeraude lors de sa prochaine invocation.
    //Carbuncle reprendra son apparence d'origine lors de sa prochaine invocation.

    readonly Regex changeRegexEn = new(@"^The next (?<petname>.+) summoned will appear glamoured as (?<petname2>.+)\.$", RegexOptions.Compiled);
    readonly Regex changeRegexJp = new(@"^次に召喚する「(?<petname>.+)」の姿を「(?<petname2>.+)」に変更しました\。$", RegexOptions.Compiled);
    readonly Regex changeRegexDe = new(@"^(?<petname>.+) nächstes Mal als (?<petname2>.+) erscheinen\.$", RegexOptions.Compiled);
    readonly Regex changeRegexFr = new(@"^(?<petname>.+) aura l'apparence de (?<petname2>.+) lors de sa prochaine invocation\.$", RegexOptions.Compiled);

    readonly Regex resetRegexEn = new(@"^The next (?<petname>.+) summoned will appear unglamoured\.$", RegexOptions.Compiled);
    readonly Regex resetRegexJp = new(@"^次に召喚する「(?<petname>.+)」の姿を元に戻しました\。$", RegexOptions.Compiled);
    readonly Regex resetRegexDe = new(@"^(?<petname>.+) wird nächstes Mal unverwandelt erscheinen\.$", RegexOptions.Compiled);
    readonly Regex resetRegexFr = new(@"^(?<petname>.+) reprendra son apparence d'origine lors de sa prochaine invocation\.$", RegexOptions.Compiled);

    readonly Regex changeRegex;
    readonly Regex resetRegex;

    readonly List<(string, int)> nameToClass = new List<(string, int)>();

    public PetGlamourChat()
    {
        TextCommand command = SheetUtils.instance.GetCommand(33);

        int counter = 0;

        for (int i = 2; i < command.Description.Payloads.Count; i++)
        {
            BasePayload secondTolastPayload = command.Description.Payloads[i - 2];
            BasePayload lastPayload = command.Description.Payloads[i - 1];
            BasePayload curPayload = command.Description.Payloads[i];

            if (!secondTolastPayload.PayloadType.HasFlag(Lumina.Text.Payloads.PayloadType.UiColorFill)) continue;
            if (!lastPayload.PayloadType.HasFlag(Lumina.Text.Payloads.PayloadType.UiColorBorder)) continue;
            if (!curPayload.PayloadType.HasFlag(Lumina.Text.Payloads.PayloadType.Text)) continue;

            string payloadString = (curPayload as TextPayload)!.RawString;

            if (counter < 5)
            {
                if (counter < 4) nameToClass.Add((payloadString, -2));
                else nameToClass.Add((payloadString, -3));
            }
            else break;
            counter++;
        }

        resetRegex = PluginHandlers.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => resetRegexJp,
            ClientLanguage.English => resetRegexEn,
            ClientLanguage.German => resetRegexDe,
            ClientLanguage.French => resetRegexFr,
            _ => resetRegexEn,
        };

        changeRegex = PluginHandlers.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => changeRegexJp,
            ClientLanguage.English => changeRegexEn,
            ClientLanguage.German => changeRegexDe,
            ClientLanguage.French => changeRegexFr,
            _ => changeRegexEn,
        };
    }

    internal override bool OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (type != XivChatType.SystemMessage) return false;

        Match match = changeRegex.Match(message.TextValue);
        if (match.Success) MatchChange(match);

        Match match2 = resetRegex.Match(message.TextValue);
        if (match2.Success) MatchReset(match2);

        return false;
    }

    void MatchChange(Match match)
    {
        string basePetName = match.Groups["petname"].Value;
        string changedPetName = match.Groups["petname2"].Value;

        int classJob = GetClassJob(basePetName);
        int skeleton = -1;

        foreach (KeyValuePair<int, string> strs in RemapUtils.instance.bakedBattlePetSkeletonToName)
        {
            if (!string.Equals(strs.Value, changedPetName, System.StringComparison.InvariantCultureIgnoreCase)) continue;
            skeleton = strs.Key;
            break;
        }

        SetClassJobToSkeleton(classJob, skeleton);
    }

    void MatchReset(Match match)
    {
        string basePetName = match.Groups["petname"].Value;
        int classJob = GetClassJob(basePetName);
        SetClassJobToSkeleton(classJob, -1);
    }

    void SetClassJobToSkeleton(int classJob, int skeleton)
    {
        if (classJob == -1) return;

        PettableUser localUser = PluginLink.PettableUserHandler.LocalUser()!;
        if(localUser == null) return;

        if (classJob == -2) localUser.SerializableUser.softSmnrSkeleton = skeleton == -1 ? PluginConstants.baseSummonerSkeleton : skeleton;
        if (classJob == -3) localUser.SerializableUser.softSchlrSkeleton = skeleton == -1 ? PluginConstants.baseScholarSkeleton : skeleton;

        PluginLink.Configuration.Save();
    }

    int GetClassJob(string basePetName)
    {
        foreach ((string, int) strs in nameToClass)
        {
            if (!string.Equals(strs.Item1, basePetName, System.StringComparison.InvariantCultureIgnoreCase)) continue;
            return strs.Item2;
        }
        return -1;
    }
}
