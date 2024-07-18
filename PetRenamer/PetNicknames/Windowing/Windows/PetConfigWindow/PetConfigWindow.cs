using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetConfigWindow;

internal class PetConfigWindow : PetWindow
{
    protected override string ID { get; } = "Configuration";
    protected override Vector2 MinSize { get; } = new Vector2(400, 200);
    protected override Vector2 MaxSize { get; } = new Vector2(400, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(400, 500);
    protected override bool HasModeToggle { get; } = false;
    protected override string Title { get; } = "Configuration";
    protected override bool HasExtraButtons { get; } = true;

    public PetConfigWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "Configuration")
    {

        IsOpen = true;

        ContentNode.ChildNodes = [
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
    }

    public override void OnDraw()
    {
        
    }
}
