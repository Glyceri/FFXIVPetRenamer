using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
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
    
    public PetConfigWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices) 
        : base(windowHandler, dalamudServices, petServices, "Pet Config Window")
    {
        PetServices.PluginWatcher.RegisterListener(OnPluginChanged);
    }

    protected override void OnDraw()
    {
        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.GeneralSettings")))
        {
            ImGui.Spacing();
            
            DrawBasicToggle(Translator.GetLine("Config.ProfilePictures"), ref PetServices.Configuration.downloadProfilePictures);
            
            DrawEnumMenu("Name Colours", _colourDisplay, ref PetServices.Configuration.SelectedColourMode);
            
            ImGui.Spacing();
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.UISettings")))
        {
            ImGui.Spacing();
            DrawBasicToggle(Translator.GetLine("Config.Kofi"),              ref PetServices.Configuration.showKofiButton);
            DrawBasicToggle(Translator.GetLine("Config.Toggle"),            ref PetServices.Configuration.quickButtonsToggle);
            
            ImGui.Separator();
            
            ImGui.Spacing();

            DrawBasicToggle(Translator.GetLine("Config.ShowNotification"),  ref PetServices.Configuration.showNotifications);

            ImGui.BeginDisabled(!PetServices.Configuration.showNotifications);

            DrawBasicToggle(Translator.GetLine("Config.IslandWarning"),     ref PetServices.Configuration.showIslandWarning);

            ImGui.EndDisabled();
            
            ImGui.Separator();
            
            ImGui.Spacing();

            DrawMenu("List Button Type", _listIconTypes, ref PetServices.Configuration.listButtonLayout);
            DrawMenu("Icon Type",        _iconMenuTypes, ref PetServices.Configuration.minionIconType);
            
            ImGui.Spacing();
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.NativeSettings")))
        {
            DrawColourConfig(Translator.GetLine("Config.Nameplate"),    ref PetServices.Configuration.ShowOnNameplatesColour);
            DrawColourConfig(Translator.GetLine("Config.Castbar"),      ref PetServices.Configuration.ShowOnCastbarsColour);
            DrawColourConfig(Translator.GetLine("Config.BattleChat"),   ref PetServices.Configuration.ShowInBattleChatColour);
            DrawColourConfig(Translator.GetLine("Config.Emote"),        ref PetServices.Configuration.ShowOnEmotesColour);
            DrawColourConfig(Translator.GetLine("Config.Tooltip"),      ref PetServices.Configuration.ShowOnTooltipColour);
            DrawColourConfig(Translator.GetLine("Config.Flyout"),       ref PetServices.Configuration.ShowOnFlyoutColour);
            DrawColourConfig(Translator.GetLine("Config.Notebook"),     ref PetServices.Configuration.ShowNamesInMinionBookColour);
            DrawColourConfig(Translator.GetLine("Config.ActionLog"),    ref PetServices.Configuration.ShowNamesInActionLogColour);
            DrawColourConfig(Translator.GetLine("Config.Targetbar"),    ref PetServices.Configuration.ShowOnTargetBarsColour);
            DrawColourConfig(Translator.GetLine("Config.Partylist"),    ref PetServices.Configuration.ShowOnPartyListColour);
            
            DrawBasicToggle (Translator.GetLine("Config.IslandPets"),   ref PetServices.Configuration.showOnIslandPets);
            DrawBasicToggle (Translator.GetLine("Config.ContextMenu"),  ref PetServices.Configuration.useContextMenus);
            
            ImGui.Spacing();
        }

        if (DrawThirdPartyHeader("Penumbra"))
        {
            ImGui.Spacing();
            
            DrawBasicToggle(Translator.GetLine("Config.Penumbra.AttachToPCP"), ref PetServices.Configuration.attachToPCP);
            DrawBasicToggle(Translator.GetLine("Config.Penumbra.ReadFromPCP"), ref PetServices.Configuration.readFromPCP);
            
            ImGui.Spacing();
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Debug")))
        {
            ImGui.Spacing();
            
            bool keyComboPressed = ImGui.IsKeyDown(ImGuiKey.LeftCtrl) && ImGui.IsKeyDown(ImGuiKey.LeftShift);

            ImGui.BeginDisabled(!keyComboPressed && !PetServices.Configuration.debugModeActive);

            DrawBasicToggle("Enable Debug Mode.",           ref PetServices.Configuration.debugModeActive);
            DrawBasicToggle("Open Debug Window On Start.",  ref PetServices.Configuration.openDebugWindowOnStart);
            DrawBasicToggle("Show chat code.",              ref PetServices.Configuration.debugShowChatCode);

            ImGui.EndDisabled();
            
            ImGui.Spacing();
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

                    SavePlugin();
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

                    SavePlugin();
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

        SavePlugin();
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
    
    private void SavePlugin()
    {
        PetServices.UserList.Recalculate();
        PetServices.Configuration.Save();
    }

    protected override void OnDispose()
    {
        PetServices.PluginWatcher.DeregisterListener(OnPluginChanged);
    }
}
