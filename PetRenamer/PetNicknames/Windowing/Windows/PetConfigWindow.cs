﻿using Dalamud.Interface.Utility;
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

    string[] listIconTypes = ["Both", "Sharing", "List Only"];
    string[] iconMenuTypes = ["Action", "Notebook", "Item"];
    string[] colourDisplay = ["Everyone", "Only Myself", "No Colours"];

    public PetConfigWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "Pet Config Window", ImGuiWindowFlags.None)
    {
        
    }

    protected override void OnDraw()
    {
        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.GeneralSettings")))
        {
            if (ImGui.Checkbox(Translator.GetLine("Config.ProfilePictures"), ref Configuration.downloadProfilePictures)) Configuration.Save();

            DrawMenu("Name Colours", colourDisplay, ref Configuration.showColours);
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.UISettings")))
        {
            if (ImGui.Checkbox(Translator.GetLine("Config.Kofi"), ref Configuration.showKofiButton)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.Toggle"), ref Configuration.quickButtonsToggle)) Configuration.Save();
            if (ImGui.Checkbox(Translator.GetLine("Config.IslandWarning"), ref Configuration.showIslandWarning)) Configuration.Save();

            DrawMenu("List Button Type", listIconTypes, ref Configuration.listButtonLayout);
            DrawMenu("Icon Type", iconMenuTypes, ref Configuration.minionIconType);
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

    void DrawMenu(string title, string[] elements, ref int configurationInt, float width = 150)
    {
        if (configurationInt < 0 || configurationInt >= elements.Length)
        {
            configurationInt = 0;
        }

        if (width <= 0) width = ImGui.GetContentRegionAvail().X;
        else width = width * ImGuiHelpers.GlobalScale;

        ImGui.SetNextItemWidth(width);

        if (ImGui.BeginCombo(title, elements[configurationInt], ImGuiComboFlags.PopupAlignLeft))
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (ImGui.Selectable(elements[i], i == configurationInt, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    configurationInt = i;
                    Configuration.Save();
                }
            }

            ImGui.EndCombo();
        }
    }
}
