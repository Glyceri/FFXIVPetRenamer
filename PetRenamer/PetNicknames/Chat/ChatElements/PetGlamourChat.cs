using Dalamud.Game;
using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using System.Text.RegularExpressions;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

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

    // LogKind = 57

    private readonly Regex fullRegexEn = new(@"Pet glamour settings", RegexOptions.Compiled);
    private readonly Regex fullRegexJp = new(@"ペットの見た目の設定状態", RegexOptions.Compiled);
    private readonly Regex fullRegexDe = new(@"Momentanes Aussehen deiner Familiare:", RegexOptions.Compiled);
    private readonly Regex fullRegexFr = new(@"Apparences de vos familiers", RegexOptions.Compiled);

    // Change is in LogMessage at 3840

    private readonly Regex changeRegexEn = new(@"^The next (?<petname>.+) summoned will appear glamoured as (?<petname2>.+)\.$", RegexOptions.Compiled);
    private readonly Regex changeRegexJp = new(@"^次に召喚する「(?<petname>.+)」の姿を「(?<petname2>.+)」に変更しました\。$", RegexOptions.Compiled);
    private readonly Regex changeRegexDe = new(@"^(?<petname>.+) wird nächstes Mal als (?<petname2>.+) erscheinen\.$", RegexOptions.Compiled);
    private readonly Regex changeRegexFr = new(@"^(?<petname>.+) aura l'apparence de (?<petname2>.+) lors de sa prochaine invocation\.$", RegexOptions.Compiled);

    // Reset is in LogMessage at index 3841

    private readonly Regex resetRegexEn = new(@"^The next (?<petname>.+) summoned will appear unglamoured\.$", RegexOptions.Compiled);
    private readonly Regex resetRegexJp = new(@"^次に召喚する「(?<petname>.+)」の姿を元に戻しました\。$", RegexOptions.Compiled);
    private readonly Regex resetRegexDe = new(@"^(?<petname>.+) wird nächstes Mal unverwandelt erscheinen\.$", RegexOptions.Compiled);
    private readonly Regex resetRegexFr = new(@"^(?<petname>.+) reprendra son apparence d'origine lors de sa prochaine invocation\.$", RegexOptions.Compiled);

    private readonly Regex changeRegex;
    private readonly Regex resetRegex;
    private readonly Regex fullRegex;
    private readonly Regex spacingRegex = new(@"^(?<petname>.+)  (?<petname2>.+)");

    private int nextRow = 0;

    private readonly DalamudServices    DalamudServices;
    private readonly IPettableUserList  UserList;
    private readonly IPetServices       PetServices;

    public PetGlamourChat(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList)
    {
        DalamudServices = dalamudServices;
        UserList        = userList;
        PetServices     = petServices;

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

    protected override void OnRestrictedChatMessage(IHandleableChatMessage chatMessage)
    {
        if(nextRow > 0)
        {
            MatchChange(spacingRegex.Match(chatMessage.Message.TextValue));
            
            return;
        }

        Match match = changeRegex.Match(chatMessage.Message.TextValue);
        
        if (match.Success)
        {
            MatchRemap(match);
        }

        Match match2 = resetRegex.Match(chatMessage.Message.TextValue);
        
        if (match2.Success)
        {
            MatchReset(match2);
        }

        Match match3 = fullRegex.Match(chatMessage.Message.TextValue);
        
        if (match3.Success)
        {
            MatchFull();
        }
    }

    private void MatchFull() 
        => nextRow = 5;

    private void MatchChange(Match match)
    {
        if (nextRow > 0)
        {
            nextRow--;
        }

        MatchRemap(match);
    }

    private void MatchRemap(Match match)
    {
        string basePetName    = match.Groups["petname"].Value;
        string changedPetName = match.Groups["petname2"].Value;

        Remap(basePetName, changedPetName);
    }

    private void Remap(string basePetName, string changedPetName)
    {
        int? classJob = GetClassJob(basePetName);
        
        if (classJob == null)
        {
            return;
        }

        IPetSheetData? sheetData = PetServices.PetSheets.GetPetFromName(changedPetName);
        
        if (sheetData == null)
        {
            return;
        }

        IPettableUser? localUser = UserList.LocalPlayer;
        
        if (localUser == null)
        {
            return;
        }

        localUser.DataBaseEntry.SetSoftSkeleton(classJob.Value, sheetData.Model);
    }

    private void MatchReset(Match match)
    {
        string basePetName = match.Groups["petname"].Value;

        int? classJob = GetClassJob(basePetName);

        if (classJob == null)
        {
            return;
        }

        IPettableUser? localUser = UserList.LocalPlayer;

        if (localUser == null)
        {
            return;
        }

        PetSkeleton baseSkeleton = PluginConstants.BaseSkeletons[classJob.Value];

        localUser.DataBaseEntry.SetSoftSkeleton(classJob.Value, baseSkeleton);
    }

    private int? GetClassJob(string basePetName) 
        => PetServices.PetSheets.NameToSoftSkeletonIndex(basePetName);
}
