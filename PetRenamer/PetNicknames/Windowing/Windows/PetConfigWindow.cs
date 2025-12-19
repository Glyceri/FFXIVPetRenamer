using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
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

    private readonly IPluginWatcher PluginWatcher;

    public PetConfigWindow(WindowHandler windowHandler, DalamudServices dalamudServices, Configuration configuration, IPluginWatcher pluginWatcher) : base(windowHandler, dalamudServices, configuration, "Pet Config Window", ImGuiWindowFlags.None)
    {
        PluginWatcher = pluginWatcher;

        PluginWatcher.RegisterListener(OnPluginChanged);
    }

    protected override void OnDraw()
    {
        if (ImGui.CollapsingHeader(Translator.GetLine("Config.Header.GeneralSettings")))
        {
            DrawBasicToggle(Translator.GetLine("Config.ProfilePictures"), ref Configuration.downloadProfilePictures);

            DrawMenu("Name Colours", _colourDisplay, ref Configuration.showColours);
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
            DrawBasicToggle(Translator.GetLine("Config.Nameplate"),     ref Configuration.showOnNameplates);
            DrawBasicToggle(Translator.GetLine("Config.Castbar"),       ref Configuration.showOnCastbars);
            DrawBasicToggle(Translator.GetLine("Config.BattleChat"),    ref Configuration.showInBattleChat);
            DrawBasicToggle(Translator.GetLine("Config.Emote"),         ref Configuration.showOnEmotes);
            DrawBasicToggle(Translator.GetLine("Config.Tooltip"),       ref Configuration.showOnTooltip);
            DrawBasicToggle(Translator.GetLine("Config.Flyout"),        ref Configuration.showOnFlyout);
            DrawBasicToggle(Translator.GetLine("Config.Notebook"),      ref Configuration.showNamesInMinionBook);
            DrawBasicToggle(Translator.GetLine("Config.ActionLog"),     ref Configuration.showNamesInActionLog);
            DrawBasicToggle(Translator.GetLine("Config.Targetbar"),     ref Configuration.showOnTargetBars);
            DrawBasicToggle(Translator.GetLine("Config.Partylist"),     ref Configuration.showOnPartyList);
            DrawBasicToggle(Translator.GetLine("Config.IslandPets"),    ref Configuration.showOnIslandPets);
            DrawBasicToggle(Translator.GetLine("Config.ContextMenu"),   ref Configuration.useContextMenus);
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
