using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text.Payloads;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Logging;
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

    //----------------------------------

    //Pet glamour settings
    //Eos  Ruby Carbuncle
    //Carbuncle  Ruby Carbuncle
    //Ifrit-Egi  Ruby Carbuncle
    //Titan-Egi  Topaz Carbuncle
    //Garuda-Egi  Garuda-Egi

    //ペットの見た目の設定状態
    //フェアリー・エオス  カーバンクル・ルビー
    //カーバンクル  カーバンクル・ルビー
    //イフリート・エギ  カーバンクル・ルビー
    //タイタン・エギ  カーバンクル・トパーズ
    //ガルーダ・エギ  ガルーダ・エギ

    //Momentanes Aussehen deiner Familiare:
    //Eos  Rubin-Karfunkel
    //Karfunkel  Rubin-Karfunkel
    //Ifrit-Egi  Rubin-Karfunkel
    //Titan-Egi  Topas-Karfunkel
    //Garuda-Egi  Garuda-Egi

    //Apparences de vos familiers
    //Eos  Carbuncle rubis
    //Carbuncle  Carbuncle rubis
    //Ifrit-Egi  Carbuncle rubis
    //Titan-Egi  Carbuncle topaze
    //Garuda-Egi  Garuda-Egi

    readonly Regex fullRegexEn = new(@"Pet glamour settings", RegexOptions.Compiled);
    readonly Regex fullRegexJp = new(@"ペットの見た目の設定状態", RegexOptions.Compiled);
    readonly Regex fullRegexDe = new(@"Momentanes Aussehen deiner Familiare:", RegexOptions.Compiled);
    readonly Regex fullRegexFr = new(@"Apparences de vos familiers", RegexOptions.Compiled);

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
    readonly Regex fullRegex;
    readonly Regex spacingRegex;

    readonly Regex spacingRegexNml = new(@"^(?<petname>.+)  (?<petname2>.+)");
    readonly Regex spacingRegexJp = new(@"^(?<petname>.+)  (?<petname2>.+)");

    readonly List<(string, int)> nameToClass = new List<(string, int)>();

    int nextRow = 0;

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

            if (counter < 5) nameToClass.Add((payloadString, counter));
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

        spacingRegex = PluginHandlers.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => spacingRegexJp,
            ClientLanguage.English => spacingRegexNml,
            ClientLanguage.German => spacingRegexNml,
            ClientLanguage.French => spacingRegexNml,
            _ => spacingRegexNml,
        };

        fullRegex = PluginHandlers.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => fullRegexJp,
            ClientLanguage.English => fullRegexEn,
            ClientLanguage.German => fullRegexDe,
            ClientLanguage.French => fullRegexFr,
            _ => fullRegexEn,
        };
    }

    internal override bool OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (type != XivChatType.SystemMessage) return false;

        if(nextRow > 0)
        {
            MatchChange(spacingRegex.Match(message.TextValue));
            return false;
        }

        Match match = changeRegex.Match(message.TextValue);
        if (match.Success) MatchChange(match);

        Match match2 = resetRegex.Match(message.TextValue);
        if (match2.Success) MatchReset(match2);

        Match match3 = fullRegex.Match(message.TextValue);
        if (match3.Success) MatchFull();

        return false;
    }

    void MatchFull() => nextRow = 5;

    void MatchChange(Match match)
    {
        if (nextRow > 0) nextRow--;

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
        PettableUser localUser = PluginLink.PettableUserHandler.LocalUser()!;
        if(localUser == null) return;
        if (classJob == -1 || classJob > localUser.SerializableUser.softSkeletons.Length) return;

        localUser.SerializableUser.softSkeletons[classJob] = skeleton == -1 ? PluginConstants.baseSkeletons[classJob] : skeleton;

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
