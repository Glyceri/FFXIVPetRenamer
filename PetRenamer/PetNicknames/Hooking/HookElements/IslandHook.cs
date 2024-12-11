using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class IslandHook : HookableElement, IIslandHook
{
    /*      German:
     *      Zu deiner Insel fahren?
     *      Zur Insel von <firstname lastname> übersetzen?
     * 
     *      English:
     *      Travel to your island?
     *      Travel to firstname lastname's island?
     *      
     *      French:
     *      Voulez-vous aller sur votre île ?
     *      Voulez-vous visitez l'île de <firstname lastname> ?
     *      
     *      Japanese:
     *      あなたの島へ向かいますか？
     *      
     */

    public bool IsOnIsland { get; private set; } = false;
    public string? VisitingFor { get; private set; } = null;
    public uint? VisitingHomeworld { get; private set; } = null;
    public bool IslandStatusChanged { get; private set; } = false;

    bool lastWasOnIsland = false;

    readonly Regex fullRegexEn = new(@"Travel to your island\?", RegexOptions.Compiled);
    readonly Regex fullRegexJp = new(@"あなたの島へ向かいますか？", RegexOptions.Compiled);
    readonly Regex fullRegexDe = new(@"Zu deiner Insel fahren\?", RegexOptions.Compiled);
    readonly Regex fullRegexFr = new(@"Voulez-vous aller sur votre île \?", RegexOptions.Compiled);

    readonly Regex visitRegexEn = new(@"^Travel to (?<firstname>\w+) (?<lastname>\w+)'s island\?$", RegexOptions.Compiled);
    readonly Regex visitRegexJp = new(@"^(?<firstname>\w+)\s(?<lastname>\w+)\s+の島へ移動します。よろしいですか？$", RegexOptions.Compiled);
    readonly Regex visitRegexDe = new(@"^Zur Insel von (?<firstname>\w+) (?<lastname>\w+) übersetzen\?$", RegexOptions.Compiled);
    readonly Regex visitRegexFr = new(@"^Voulez-vous visitez l'île de (?<firstname>\w+) (?<lastname>\w+) \?$", RegexOptions.Compiled);

    readonly Regex activeRegex;
    readonly Regex activeVisitRegex;

    public IslandHook(in DalamudServices services, in IPettableUserList userList, in IPetServices petServices, in IPettableDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener)
    {
        activeRegex = DalamudServices.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => fullRegexJp,
            ClientLanguage.English => fullRegexEn,
            ClientLanguage.German => fullRegexDe,
            ClientLanguage.French => fullRegexFr,
            _ => fullRegexEn,
        };

        activeVisitRegex = DalamudServices.ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => visitRegexJp,
            ClientLanguage.English => visitRegexEn,
            ClientLanguage.German => visitRegexDe,
            ClientLanguage.French => visitRegexFr,
            _ => visitRegexEn,
        };
    }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "SelectYesno", LifeCycleUpdate);
    }

    void LifeCycleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => Update((AtkUnitBase*)addonArgs.Addon);

    public void Update()
    {
        IslandStatusChanged = false;
        IsOnIsland = MJIManager.Instance()->IsPlayerInSanctuary == 1;

        if (lastWasOnIsland != IsOnIsland)
        {
            lastWasOnIsland = IsOnIsland;
            IslandStatusChanged = true;
            OnIslandStatusChanged();
        }
    }

    void OnIslandStatusChanged()
    {
        if (!IsOnIsland)
        {
            VisitingFor = null;
            VisitingHomeworld = null;
        }
        else
        {
            if ((VisitingFor == null || VisitingHomeworld == null) && PetServices.Configuration.showIslandWarning)
            {
                DalamudServices.NotificationManager.AddNotification(new Notification()
                {
                    Type = NotificationType.Warning,
                    Content = "Pet Nicknames was unable to resolve the owner of this island. Please rejoin this island.",
                });
            }
        }
    }

    void Update(AtkUnitBase* addon)
    {
        if (addon == null) return;
        if (!addon->IsVisible) return;

        BaseNode yesNoBox = new BaseNode(addon);
        if (yesNoBox == null) return;

        AtkTextNode* tNode = yesNoBox.GetNode<AtkTextNode>(2);
        if (tNode == null) return;

        string text = tNode->NodeText.ToString();
        if (text.IsNullOrWhitespace()) return;

        ParseText(text);
    }

    void ParseText(string text)
    {
        Match match = activeRegex.Match(text);
        if (match.Success)
        {
            HandleSolo();
            return;
        }

        Match match2 = activeVisitRegex.Match(text);
        if (match2.Success)
        {
            HandleOther(match2);
            return;
        }
    }

    void HandleSolo()
    {
        IPlayerCharacter? localPlayer = DalamudServices.ClientState.LocalPlayer;
        if (localPlayer == null) return;

        string name = localPlayer.Name.TextValue;
        uint curWorld = localPlayer.HomeWorld.ValueNullable?.RowId ?? 0;

        SetFor(name, curWorld);
    }

    void HandleOther(Match match)
    {
        IPlayerCharacter? localPlayer = DalamudServices.ClientState.LocalPlayer;
        if (localPlayer == null) return;

        string firstname = match.Groups["firstname"].Value;
        string lastname = match.Groups["lastname"].Value;
        uint curWorld = localPlayer.CurrentWorld.ValueNullable?.RowId ?? 0;

        SetFor(firstname, lastname, curWorld);
    }

    void SetFor(string firstname, string lastname, uint homeworld) => SetFor($"{firstname} {lastname}", homeworld);
    void SetFor(string name, uint homeworld)
    {
        VisitingFor = name;
        VisitingHomeworld = homeworld;

        PetServices.PetLog.LogVerbose("Visiting For: " + VisitingFor + ", at Homeworld: " + VisitingHomeworld);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
    }
}
