using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetConfigWindow : PetWindow
{
    protected override Vector2 MinSize { get; } = new Vector2(400, 200);
    protected override Vector2 MaxSize { get; } = new Vector2(400, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(400, 500);
    protected override bool HasModeToggle { get; } = true;

    public PetConfigWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "Pet Config Window", ImGuiWindowFlags.None)
    {
        
    }

    protected override void OnDraw()
    {
        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.GeneralSettings")))
        {
            if (ImGui.Checkbox(Translator.GetLine("Config.ProfilePictures"), ref Configuration.downloadProfilePictures)) Configuration.Save();
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.UISettings")))
        {
            if (ImGui.Checkbox(Translator.GetLine("Config.Kofi"), ref Configuration.showKofiButton)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.Toggle"), ref Configuration.quickButtonsToggle)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.IslandWarning"), ref Configuration.showIslandWarning)) Configuration.Save();
            
            // Why for the life of me do I not know a better way to do this?
            if (ImGui.BeginMenu($"Icon Type##Menu_{WindowHandler.InternalCounter}"))
            {
                if (ImGui.MenuItem($"Action##Menu_{WindowHandler.InternalCounter}"))
                {
                    Configuration.minionIconType = 0;
                    Configuration.Save();
                }
                if (ImGui.MenuItem($"Notebook##Menu_{WindowHandler.InternalCounter}"))
                {
                    Configuration.minionIconType = 1;
                    Configuration.Save();
                }
                if (ImGui.MenuItem($"Item##Menu_{WindowHandler.InternalCounter}"))
                {
                    Configuration.minionIconType = 2;
                    Configuration.Save();
                }

                ImGui.EndMenu();
            }

            // Why for the life of me do I not know a better way to do this?
            if (ImGui.BeginMenu($"List Button Type##Menu_{WindowHandler.InternalCounter}"))
            {
                if (ImGui.MenuItem($"Both##Menu_{WindowHandler.InternalCounter}"))
                {
                    Configuration.listButtonLayout = 0;
                    Configuration.Save();
                }
                if (ImGui.MenuItem($"Sharing Only##Menu_{WindowHandler.InternalCounter}"))
                {
                    Configuration.listButtonLayout = 1;
                    Configuration.Save();
                }
                if (ImGui.MenuItem($"List Only##Menu_{WindowHandler.InternalCounter}"))
                {
                    Configuration.listButtonLayout = 2;
                    Configuration.Save();
                }

                ImGui.EndMenu();
            }
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.NativeSettings")))
        {
            if (ImGui.Checkbox(Translator.GetLine("Config.Nameplate"), ref Configuration.showOnNameplates)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.Castbar"), ref Configuration.showOnCastbars)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.BattleChat"), ref Configuration.showInBattleChat)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.Emote"), ref Configuration.showOnEmotes)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.Tooltip"), ref Configuration.showOnTooltip)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.Flyout"), ref Configuration.showOnFlyout)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.Notebook"), ref Configuration.showNamesInMinionBook)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.ActionLog"), ref Configuration.showNamesInActionLog)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.Targetbar"), ref Configuration.showOnTargetBars)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.Partylist"), ref Configuration.showOnPartyList)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.IslandPets"), ref Configuration.showOnIslandPets)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.ContextMenu"), ref Configuration.useContextMenus)) Configuration.Save();
        }

       
        if (ImGui.CollapsingHeader(Translator.GetLine("Debug")))
        {
            bool keyComboPressed = ImGui.IsKeyDown(ImGuiKey.LeftCtrl) && ImGui.IsKeyDown(ImGuiKey.LeftShift);

            ImGui.BeginDisabled(!keyComboPressed && !Configuration.debugModeActive);
            if (ImGui.Checkbox("Enable Debug Mode.", ref Configuration.debugModeActive)) Configuration.Save();
            if (ImGui.Checkbox("Open Debug Window On Start.", ref Configuration.openDebugWindowOnStart)) Configuration.Save();
            if (ImGui.Checkbox("Show chat code.", ref Configuration.debugShowChatCode)) Configuration.Save();
            ImGui.EndDisabled();
        }
    }
}
