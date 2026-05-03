using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetConfigWindow : PetWindow
{
    protected override Vector2  MinSize         { get; } = new Vector2(400, 200);
    protected override Vector2  MaxSize         { get; } = new Vector2(400, 1200);
    protected override Vector2  DefaultSize     { get; } = new Vector2(400, 500);

    protected override bool     HasModeToggle   { get; } = true;

    private static readonly string[] _listIconTypes = ["Both", "Sharing", "List Only"];
    private static readonly string[] _iconMenuTypes = ["Action", "Notebook", "Item"];
    private static readonly string[] _colourDisplay = ["Everyone", "Only Myself", "No Colours"];

    private readonly Dictionary<string, bool> ThirdPartySupported = new Dictionary<string, bool>()
    {
        { "Penumbra", false },
    };

    private readonly IPluginWatcher PluginWatcher;

    public PetConfigWindow(WindowHandler windowHandler, DalamudServices dalamudServices, Configuration configuration, IPluginWatcher pluginWatcher) 
        : base(windowHandler, dalamudServices, configuration, "Pet Config Window")
    {
        PluginWatcher = pluginWatcher;

        PluginWatcher.RegisterListener(OnPluginChanged);
        
        IsOpen = true;
    }

    protected override void OnDraw()
    {
        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.GeneralSettings")))
        {
            DrawBasicToggle(Translator.GetLine("Config.ProfilePictures"), ref Configuration.downloadProfilePictures);
            
            DrawEnumMenu("Name Colours", _colourDisplay, ref Configuration.SelectedColourMode);
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.UISettings")))
        {
            DrawBasicToggle(Translator.GetLine("Config.Kofi"),              ref Configuration.showKofiButton);
            DrawBasicToggle(Translator.GetLine("Config.Toggle"),            ref Configuration.quickButtonsToggle);

            ImGui.NewLine();

            DrawBasicToggle(Translator.GetLine("Config.ShowNotification"),  ref Configuration.showNotifications);

            ImGui.BeginDisabled(!Configuration.showNotifications);

            DrawBasicToggle(Translator.GetLine("Config.IslandWarning"),     ref Configuration.showIslandWarning);

            ImGui.EndDisabled();

            ImGui.NewLine();

            DrawMenu("List Button Type", _listIconTypes, ref Configuration.listButtonLayout);
            DrawMenu("Icon Type",        _iconMenuTypes, ref Configuration.minionIconType);
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.NativeSettings")))
        {
            DrawColourConfig(Translator.GetLine("Config.Nameplate"),    ref Configuration.ShowOnNameplatesColour);
            DrawColourConfig(Translator.GetLine("Config.Castbar"),      ref Configuration.ShowOnCastbarsColour);
            DrawColourConfig(Translator.GetLine("Config.BattleChat"),   ref Configuration.ShowInBattleChatColour);
            DrawColourConfig(Translator.GetLine("Config.Emote"),        ref Configuration.ShowOnEmotesColour);
            DrawColourConfig(Translator.GetLine("Config.Tooltip"),      ref Configuration.ShowOnTooltipColour);
            DrawColourConfig(Translator.GetLine("Config.Flyout"),       ref Configuration.ShowOnFlyoutColour);
            DrawColourConfig(Translator.GetLine("Config.Notebook"),     ref Configuration.ShowNamesInMinionBookColour);
            DrawColourConfig(Translator.GetLine("Config.ActionLog"),    ref Configuration.ShowNamesInActionLogColour);
            DrawColourConfig(Translator.GetLine("Config.Targetbar"),    ref Configuration.ShowOnTargetBarsColour);
            DrawColourConfig(Translator.GetLine("Config.Partylist"),    ref Configuration.ShowOnPartyListColour);
            
            DrawBasicToggle (Translator.GetLine("Config.IslandPets"),   ref Configuration.showOnIslandPets);
            DrawBasicToggle (Translator.GetLine("Config.ContextMenu"),  ref Configuration.useContextMenus);
        }

        if (DrawThirdPartyHeader("Penumbra"))
        {
            DrawBasicToggle(Translator.GetLine("Config.Penumbra.AttachToPCP"), ref Configuration.attachToPCP);
            DrawBasicToggle(Translator.GetLine("Config.Penumbra.ReadFromPCP"), ref Configuration.readFromPCP);
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Debug")))
        {
            bool keyComboPressed = ImGui.IsKeyDown(ImGuiKey.LeftCtrl) && ImGui.IsKeyDown(ImGuiKey.LeftShift);

            ImGui.BeginDisabled(!keyComboPressed && !Configuration.debugModeActive);

            DrawBasicToggle("Enable Debug Mode.",           ref Configuration.debugModeActive);
            DrawBasicToggle("Open Debug Window On Start.",  ref Configuration.openDebugWindowOnStart);
            DrawBasicToggle("Show chat code.",              ref Configuration.debugShowChatCode);

            ImGui.EndDisabled();
        }
    }

    private void DrawMenu(string title, string[] elements, ref int configurationInt, float width = 150)
    {
        if (configurationInt < 0 || configurationInt >= elements.Length)
        {
            configurationInt = 0;
        }

        if (width <= 0)
        {
            width = ImGui.GetContentRegionAvail().X;
        }
        else
        {
            width = width * WindowHandler.GlobalScale;
        }

        ImGui.SetNextItemWidth(width);

        if (ImGui.BeginCombo(title, elements[configurationInt], ImGuiComboFlags.PopupAlignLeft))
        {
            for (int i = 0; i < elements.Length; i++)
            {
                bool elementIsCurrent = i == configurationInt;

                if (ImGui.Selectable(elements[i], elementIsCurrent, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    configurationInt = i;

                    Configuration.Save();
                }
            }

            ImGui.EndCombo();
        }
    }
    
    private void DrawEnumMenu(string title, string[] elements, ref Configuration.ColourMode enumValue, float width = 150) 
    {
        if (width <= 0)
        {
            width = ImGui.GetContentRegionAvail().X;
        }
        else
        {
            width = width * WindowHandler.GlobalScale;
        }
        
        ImGui.SetNextItemWidth(width);

        if (ImGui.BeginCombo(title, elements[(int)enumValue], ImGuiComboFlags.PopupAlignLeft))
        {
            for (int i = 0; i < elements.Length; i++)
            {
                bool elementIsCurrent = i == (int)enumValue;

                if (ImGui.Selectable(elements[i], elementIsCurrent, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    enumValue = (Configuration.ColourMode)i;

                    Configuration.Save();
                }
            }

            ImGui.EndCombo();
        }
    }

    private bool DrawThirdPartyHeader(string internalName, string? displayTitle = null)
    {
        if (!ThirdPartySupported.TryGetValue(internalName, out bool supported))
        {
            return false;
        }

        if (!supported)
        {
            return false;
        }

        string actualDisplayTitle = displayTitle ?? internalName;

        return ImGui.CollapsingHeader(actualDisplayTitle);
    }

    private void DrawBasicToggle(string title, ref bool value)
    {
        if (!ImGui.Checkbox(title, ref value))
        {
            return;
        }

        Configuration.Save();
    }
    
    private void DrawColourConfig(string title, ref Configuration.ColourConfig colourConfig)
    {
        ImGui.Spacing();
        
        DrawBasicToggle(title, ref colourConfig.Enabled);

        ImGui.BeginDisabled(!colourConfig.Enabled);
        
        DrawBasicToggle(Translator.GetLine("Config.Label.OverrideColour") + $"###BOOL{title}", ref colourConfig.OverrideColourMode);
        
        ImGui.SameLine();
        
        ImGui.BeginDisabled(!colourConfig.OverrideColourMode);
        
        DrawEnumMenu(Translator.GetLine("Config.Label.OverrideColourLabel") + $"###COLOUR{title}", _colourDisplay, ref colourConfig.ColourMode);
        
        ImGui.EndDisabled();
        
        ImGui.EndDisabled();
        
        ImGui.Separator();
    }

    private void OnPluginChanged(string[] internalPlugins)
    {
        // Reset the whole list
        foreach (string key in ThirdPartySupported.Keys)
        {
            ThirdPartySupported[key] = false;
        }

        // Flood the third parties
        foreach (string plugin in internalPlugins)
        {
            if (!ThirdPartySupported.ContainsKey(plugin))
            {
                continue;
            }

            ThirdPartySupported[plugin] = true;
        }
    }

    protected override void OnDispose()
    {
        PluginWatcher.DeregisterListener(OnPluginChanged);
    }
}
