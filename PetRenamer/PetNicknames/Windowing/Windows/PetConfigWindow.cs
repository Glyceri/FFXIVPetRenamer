using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetConfigWindow : PetWindow
{
    private readonly Dictionary<string, bool> ThirdPartySupported = new Dictionary<string, bool>()
    {
        { "Penumbra", false },
    };
    
    public PetConfigWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices) 
        : base(windowHandler, dalamudServices, petServices, "Pet Settings")
    {
        PetServices.PluginWatcher.RegisterListener(OnPluginChanged);
    }
    
    public override bool ShowQuickButtons
        => true;
    
    public override bool HasModeToggle
        => false;

    protected override Vector2 MinSize
        => new Vector2(400, 200);
    
    protected override Vector2 MaxSize
        => new Vector2(400, 1200);
    
    protected override Vector2 DefaultSize 
        => new Vector2(400, 500);
    
    private static readonly string[] _listIconTypes   = ["ListIconType.Both", "ListIconType.Sharing", "ListIconType.ListOnly"];
    private static readonly string[] _iconMenuTypes   = ["MenuType.Action", "MenuType.Notebook", "MenuType.Item"];
    private static readonly string[] _colourDisplay   = ["ColourDisplay.Everyone", "ColourDisplay.OnlyMyself", "ColourDisplay.NoColours"];
    private static readonly string[] _languageOptions = ["Language.Default", "Language.English", "Language.German", "Language.French", "Language.Japanese", "Language.Dutch", "Language.Chinese"];
    
    protected override void OnDraw()
    {
        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.GeneralSettings")))
        {
            ImGui.Spacing();
            
            DrawBasicToggle(Translator.GetLine("Config.ProfilePictures"), ref PetServices.Configuration.downloadProfilePictures);
            
            DrawEnumMenu(Translator.GetLine("Config.ColourSettings"), _colourDisplay, ref PetServices.Configuration.SelectedColourMode);
            
            ImGui.Spacing();
        }

        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.UISettings")))
        {
            ImGui.Spacing();
            
            if (DrawEnumMenu(Translator.GetLine("Config.Language"),   _languageOptions, ref PetServices.Configuration.currentLanguage))
            {
                Translator.UpdateLanguage();
            }
            
            ImGui.Separator();
            
            DrawBasicToggle(Translator.GetLine("Config.Kofi"),              ref PetServices.Configuration.showKofiButton);
            DrawBasicToggle(Translator.GetLine("Config.Toggle"),            ref PetServices.Configuration.quickButtonsToggle);
            
            ImGui.Separator();
            
            DrawBasicToggle(Translator.GetLine("Config.OldBarStyle"),       ref PetServices.Configuration.oldBarStyleLayout);
            
            ImGui.Separator();
            
            ImGui.Spacing();

            DrawBasicToggle(Translator.GetLine("Config.ShowNotification"),  ref PetServices.Configuration.showNotifications);

            ImGui.BeginDisabled(!PetServices.Configuration.showNotifications);

            DrawBasicToggle(Translator.GetLine("Config.IslandWarning"),     ref PetServices.Configuration.showIslandWarning);

            ImGui.EndDisabled();
            
            ImGui.Separator();
            
            ImGui.Spacing();

            DrawMenu(Translator.GetLine("Config.ListButtonType"), _listIconTypes,   ref PetServices.Configuration.listButtonLayout);
            DrawMenu(Translator.GetLine("Config.IconType"),       _iconMenuTypes,   ref PetServices.Configuration.minionIconType);
                
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
            
            DrawBasicToggle (Translator.GetLine("Config.PartyCutoff"),  ref PetServices.Configuration.allowPartySummonCutoff);
            DrawInfoHover(Translator.GetLine("Config.PartyCutoff.Help"));
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

        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.Debug")))
        {
            ImGui.Spacing();
            
            bool keyComboPressed = ImGui.IsKeyDown(ImGuiKey.LeftCtrl) && ImGui.IsKeyDown(ImGuiKey.LeftShift);

            ImGui.BeginDisabled(!keyComboPressed && !PetServices.Configuration.debugModeActive);

            DrawBasicToggle(Translator.GetLine("Config.DebugMode"),      ref PetServices.Configuration.debugModeActive);
            DrawInfoHover(Translator.GetLine("Config.DebugMode.Help"));
            DrawBasicToggle(Translator.GetLine("Config.OpenDebugMode"),  ref PetServices.Configuration.openDebugWindowOnStart);
            DrawInfoHover(Translator.GetLine("Config.DebugMode.Help"));
            DrawBasicToggle(Translator.GetLine("Config.DebugChatcode"),  ref PetServices.Configuration.debugShowChatCode);
            DrawInfoHover(Translator.GetLine("Config.DebugMode.Help"));
            
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

        if (ImGui.BeginCombo(title,  Translator.GetLine(elements[configurationInt]), ImGuiComboFlags.PopupAlignLeft))
        {
            for (int i = 0; i < elements.Length; i++)
            {
                bool elementIsCurrent = i == configurationInt;

                if (ImGui.Selectable(Translator.GetLine(elements[i]), elementIsCurrent, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    configurationInt = i;

                    SavePlugin();
                }
            }

            ImGui.EndCombo();
        }
    }
    
    private bool DrawEnumMenu<T>(string title, string[] elements, ref T enumValue, float width = 150)
        where T : struct, Enum
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
        
        bool changed = false;
        
        if (ImGui.BeginCombo(title, Translator.GetLine(elements[enumValue.GetEnumIndex()]), ImGuiComboFlags.PopupAlignLeft))
        {
            for (int i = 0; i < elements.Length; i++)
            {
                bool elementIsCurrent = i == enumValue.GetEnumIndex();

                if (ImGui.Selectable(Translator.GetLine(elements[i]), elementIsCurrent, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    enumValue = (T)Enum.ToObject(typeof(T), i);

                    SavePlugin();
                    
                    changed = true;
                }
            }

            ImGui.EndCombo();
        }
        
        return changed;
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
    
    private void DrawInfoHover(string infoText)
    {
        ImGui.SameLine();
        
        ImGui.BeginDisabled();
        ImGui.PushFont(UiBuilder.IconFont);
        
        ImGui.Text($"{FontAwesomeIcon.Question.ToIconString()}");

        ImGui.PopFont();
        ImGui.EndDisabled();

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.BeginDisabled(false);
            ImGui.SetTooltip(infoText);
            ImGui.EndDisabled();
        }
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
        PetServices.DirtyCaller.DirtyConfig(PetServices.Configuration);
        PetServices.UserList.Recalculate();
        PetServices.Configuration.Save();
    }

    protected override void OnDispose()
    {
        PetServices.PluginWatcher.DeregisterListener(OnPluginChanged);
    }
}
