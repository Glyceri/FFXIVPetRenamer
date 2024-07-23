using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;
using Una.Drawing;
using System.Numerics;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using Dalamud.Interface;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetConfigWindow;

internal class PetConfigWindow : PetWindow
{
    protected override string ID { get; } = "Configuration";
    protected override Vector2 MinSize { get; } = new Vector2(400, 200);
    protected override Vector2 MaxSize { get; } = new Vector2(400, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(400, 500);
    protected override bool HasModeToggle { get; } = false;
    protected override string Title { get; } = Translator.GetLine("Config.Title");
    protected override bool HasExtraButtons { get; } = true;

    readonly SettingsHolderNode UISettingsNode;
    readonly SettingsHolderNode GeneralSettingsNode;
    readonly SettingsHolderNode PetSettingsNode;

    readonly QuickSquareButton PaletteButton;

    public PetConfigWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "Configuration")
    {
        ContentNode.Overflow = false;

        ContentNode.Style = new Style()
        {
            ScrollbarTrackColor = new Color(0, 0, 0, 0),
            ScrollbarThumbColor = new Color(224, 183, 18, 50),
            ScrollbarThumbHoverColor = new Color(224, 183, 18, 200),
            ScrollbarThumbActiveColor = new Color(237, 197, 33, 255),
        };

        ContentNode.ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Padding = new EdgeSize(3),
                    Gap = 6,

                },
                ChildNodes = [
                    GeneralSettingsNode = new SettingsHolderNode(in Configuration, Translator.GetLine("Config.Header.GeneralSettings")),
                    UISettingsNode = new SettingsHolderNode(in Configuration, Translator.GetLine("Config.Header.UISettings")),
                    PetSettingsNode = new SettingsHolderNode(in Configuration, Translator.GetLine("Config.Header.NativeSettings")),
                ]
            }
        ];

        PetSettingsNode.ContentNode.ChildNodes = [
            new ToggleConfig(in Configuration, Translator.GetLine("Config.Nameplate"), Configuration.showOnNameplates, (value) => Configuration.showOnNameplates = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.Castbar"), Configuration.showOnCastbars, (value) => Configuration.showOnCastbars = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.BattleChat"), Configuration.showInBattleChat, (value) => Configuration.showInBattleChat = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.Emote"), Configuration.showOnEmotes, (value) => Configuration.showOnEmotes = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.Tooltip"), Configuration.showOnTooltip, (value) => Configuration.showOnTooltip = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.Notebook"), Configuration.showNamesInMinionBook, (value) => Configuration.showNamesInMinionBook = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.ActionLog"), Configuration.showNamesInActionLog, (value) => Configuration.showNamesInActionLog = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.Targetbar"), Configuration.showOnTargetBars, (value) => Configuration.showOnTargetBars = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.Partylist"), Configuration.showOnPartyList, (value) => Configuration.showOnPartyList = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.ContextMenu"), Configuration.useContextMenus, (value) => Configuration.useContextMenus = value),
        ];

        GeneralSettingsNode.ContentNode.ChildNodes = [
             new ToggleConfig(in Configuration, Translator.GetLine("Config.ProfilePictures"), Configuration.downloadProfilePictures, (value) => Configuration.downloadProfilePictures = value),
             new LanguageSettingsBar(in Configuration),
        ];

        UISettingsNode.ContentNode.ChildNodes = [
            new UIScaleSettingsBar(in Configuration),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.Toggle"), Configuration.quickButtonsToggle, (value) => { Configuration.quickButtonsToggle = value; }),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.Kofi"), Configuration.showKofiButton, (value) => { Configuration.showKofiButton = value; WindowHandler.SetKofiMode(Configuration.showKofiButton); }),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.TransparentBackground"), Configuration.transparentBackground, (value) => Configuration.transparentBackground = value),
            new ToggleConfig(in Configuration, Translator.GetLine("Config.UIFlare"), Configuration.uiFlare, (value) => Configuration.uiFlare = value),
            PaletteButton = new QuickSquareButton()
            {
                NodeValue = FontAwesomeIcon.Palette.ToIconString(),
                Style = new Style() { Size = new Size(32, 15), },
            }
        ];


        PaletteButton.OnClick += () => WindowHandler.Open<ColourEditorWindow.ColourEditorWindow>();
    }

    public override void OnDraw()
    {

    }
}
