using Dalamud;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text.Payloads;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using static FFXIVClientStructs.FFXIV.Client.Game.UI.MapMarkerData.Delegates;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

// This check is literally here to catch pet glamour changes 1... and I literally mean 1 step early.
internal class PetGlamourChat : RestrictedChatElement
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
    readonly Regex changeRegexDe = new(@"^(?<petname>.+) wird nächstes Mal als (?<petname2>.+) erscheinen\.$", RegexOptions.Compiled);
    readonly Regex changeRegexFr = new(@"^(?<petname>.+) aura l'apparence de (?<petname2>.+) lors de sa prochaine invocation\.$", RegexOptions.Compiled);

    readonly Regex resetRegexEn = new(@"^The next (?<petname>.+) summoned will appear unglamoured\.$", RegexOptions.Compiled);
    readonly Regex resetRegexJp = new(@"^次に召喚する「(?<petname>.+)」の姿を元に戻しました\。$", RegexOptions.Compiled);
    readonly Regex resetRegexDe = new(@"^(?<petname>.+) wird nächstes Mal unverwandelt erscheinen\.$", RegexOptions.Compiled);
    readonly Regex resetRegexFr = new(@"^(?<petname>.+) reprendra son apparence d'origine lors de sa prochaine invocation\.$", RegexOptions.Compiled);

    readonly Regex changeRegex;
    readonly Regex resetRegex;
    readonly Regex fullRegex;
    readonly Regex spacingRegex = new(@"^(?<petname>.+)  (?<petname2>.+)");

    int nextRow = 0;

    DalamudServices DalamudServices { get; init; }
    IPettableUserList UserList { get; init; }
    IPetServices PetServices { get; init; }

    public PetGlamourChat(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList)
    {
        DalamudServices = dalamudServices;
        UserList = userList;
        PetServices = petServices;

        RegisterChat(XivChatType.SystemMessage);

        resetRegex = DalamudServices.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => resetRegexJp,
            ClientLanguage.English => resetRegexEn,
            ClientLanguage.German => resetRegexDe,
            ClientLanguage.French => resetRegexFr,
            _ => resetRegexEn,
        };

        changeRegex = DalamudServices.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => changeRegexJp,
            ClientLanguage.English => changeRegexEn,
            ClientLanguage.German => changeRegexDe,
            ClientLanguage.French => changeRegexFr,
            _ => changeRegexEn,
        };

        fullRegex = DalamudServices.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => fullRegexJp,
            ClientLanguage.English => fullRegexEn,
            ClientLanguage.German => fullRegexDe,
            ClientLanguage.French => fullRegexFr,
            _ => fullRegexEn,
        };
    }

    internal override void OnRestrictedChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if(nextRow > 0)
        {
            MatchChange(spacingRegex.Match(message.TextValue));
            return;
        }

        Match match = changeRegex.Match(message.TextValue);
        if (match.Success) MatchRemap(match);

        Match match2 = resetRegex.Match(message.TextValue);
        if (match2.Success) MatchReset(match2);

        Match match3 = fullRegex.Match(message.TextValue);
        if (match3.Success) MatchFull();
    }

    void MatchFull() => nextRow = 5;

    void MatchChange(Match match)
    {
        if (nextRow > 0) nextRow--;

        string basePetName = match.Groups["petname"].Value;
        string changedPetName = match.Groups["petname2"].Value;

        Remap(basePetName, changedPetName);
    }

    void MatchRemap(Match match)
    {
        string basePetName = match.Groups["petname"].Value;
        string changedPetName = match.Groups["petname2"].Value;

        Remap(basePetName, changedPetName);
    }

    void Remap(string basePetName, string changedPetName)
    {
        int? classJob = GetClassJob(basePetName);
        if (classJob == null) return;

        PetSheetData? sheetData = PetServices.PetSheets.GetPetFromName(changedPetName);
        if (sheetData == null) return;

        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return;
        localUser.DataBaseEntry.SoftSkeletons[classJob.Value] = sheetData.Value.Model;
    }

    void MatchReset(Match match)
    {
        string basePetName = match.Groups["petname"].Value;

        int? classJob = GetClassJob(basePetName);
        if (classJob == null) return;

        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return;

        int baseSkeleton = PluginConstants.BaseSkeletons[classJob.Value];
        localUser.DataBaseEntry.SoftSkeletons[classJob.Value] = baseSkeleton;
    }

    int? GetClassJob(string basePetName) => PetServices.PetSheets.NameToSoftSkeletonIndex(basePetName);
}
