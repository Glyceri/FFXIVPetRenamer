using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using PetRenamer.Windows.Attributes;
using IsUnsafe = System.Runtime.CompilerServices.Unsafe;

namespace PetRenamer.Windows.PetWindows;

[ConfigPetWindow]
[PersistentPetWindow]
[ModeTogglePetWindow]
public class ConfigWindow : PetWindow
{
    Vector2 baseSize = new Vector2(500, 400);

    public ConfigWindow() : base("Pet Nicknames Settings")
    {
        Size = baseSize;
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = baseSize,
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    bool anythingIllegals = false;

    const string starter = "  ◉   ";
    float currentHeight = 0;
    bool canDraw = true;
    readonly Dictionary<string, (bool, float)> toggles = new Dictionary<string, (bool, float)>();
    string currentTitle = string.Empty;

    public override void OnDraw()
    {
        anythingIllegals = AnyIllegalsGoingOn();

        if ((anythingIllegals && !PluginLink.Configuration.understoodWarningThirdPartySettings) || DebugMode)
        {
            if (BeginElementBox("Third Party WARNING", true))
            {
                DrawConfigElement(ref PluginLink.Configuration.understoodWarningThirdPartySettings, "I UNDERSTAND!", new string[] { "Do NOT send feedback or issues for these settings on discord or via the official [Send Feedback] button!", "ONLY Github Issues are ALLOWED!", "Enables Settings Related to Other Plugins (Main Repo or Not)!" }, "READ THE WARNING!");
                EndElementBox();
            }
        }

        if (BeginElementBox("UI Settings"))
        {
            DrawConfigElement(ref PluginLink.Configuration.newUseCustomTheme, "Use Custom Theme", new string[] { "You Can Make Your Own Theme, Click Here To Enable That Feature.", "Open using [/pettheme] [/miniontheme]" }, "Use Custom Theme [/pettheme]", OnChange: (value) => SetTheme() );
            DrawConfigElement(ref PluginLink.Configuration.displayImages, "Display Images", "Display Images or Replace them with a Flat Colour?", "Display Images"); 
            DrawConfigElement(ref PluginLink.Configuration.spaceOutSettings, "Space Out Settings", "Spaces Out Settings making it Easier to Read.");
            DrawConfigElement(ref PluginLink.Configuration.startSettingsOpen, "Start with all the settings unfolded", "Upon Starting the Plugin, Automatically Unfold all Setting Panels.");
            DrawConfigElement(ref PluginLink.Configuration.quickButtonsToggle, "Quick Buttons Toggle Instead of Open", "The Quick Buttons in the Top Bar Toggle Instead of Open.");
            EndElementBox();
        }
        if (BeginElementBox("Global Settings"))
        {
            DrawConfigElement(ref PluginLink.Configuration.displayCustomNames, "Display Custom Nicknames", new string[] { "Completely Enables or Disables Custom Nicknames.", "Prevents Most parts of the Plugin from Working!" });
            DrawConfigElement(ref PluginLink.Configuration.automaticallySwitchPetmode, "Automatically Switch Pet Mode", "Upon Summoning a Minion or Battle Pet, Automatically Switch Pet Mode?");
            DrawConfigElement(ref PluginLink.Configuration.downloadProfilePictures, "Automatically Download Profile Pictures", "Upon Importing a User (or yourself). Automatically Download their Profile Picture?");
            EndElementBox();
        }
    }

    public override void OnDrawNormal()
    {
        if (BeginElementBox("Minion Settings"))
        {
            DrawConfigElement(ref PluginLink.Configuration.useContextMenuOnMinions, "Allow Context Menus for Minions", "Rightclicking on a Minion will add the [Give Nickname] Option.");
            DrawConfigElement(ref PluginLink.Configuration.allowTooltipsOnMinions, "Allow Tooltips for Minions", "Display Minion Nicknames in Tooltips.", "Allow Tooltips for Minions");
            DrawConfigElement(ref PluginLink.Configuration.replaceEmotesOnMinions, "Allow Custom Nicknames in Emotes for Minions", "Replace a Minions in-game Name with your Custom Nickname.");
            DrawConfigElement(ref PluginLink.Configuration.showNamesInMinionBook, "Show Nicknames in the Minion Journal", "Shows your Custom Nicknames in the Minion Journal.");

            if (PenumbraValid)
            {
                Header("[Penumbra]");
                DrawConfigElement(ref PluginLink.Configuration.redrawMinionOnSpawn, "Redraw Minion", new string[] { "Redraw a Minion upon a Name Change (Fixes nameplate bugs).", "Requires [Penumbra]" });
            }

            EndElementBox();
        }
    }

    public override void OnDrawBattlePet()
    {
        if (BeginElementBox("Battle Pet Settings"))
        {
            DrawConfigElement(ref PluginLink.Configuration.useContextMenuOnBattlePets, "Allow Context Menus for Battle Pets", "Rightclicking on a Battle Pet will add the [Give Nickname] Option.");
            DrawConfigElement(ref PluginLink.Configuration.allowTooltipsBattlePets, "Allow Tooltips for Battle Pets", "Display Battle Pet Nicknames in Tooltips.");
            DrawConfigElement(ref PluginLink.Configuration.replaceEmotesBattlePets, "Allow Custom Nicknames in Emotes for Battle Pets", "Replace a Battle Pet in-game Name with your Custom Nickname.");
            DrawConfigElement(ref PluginLink.Configuration.useCustomPetNamesInBattleChat, "Allow Custom Nicknames in the Battle Log for Battle Pets", "Replace a Battle Pet in-game Name with your Custom Nickname.");
            DrawConfigElement(ref PluginLink.Configuration.allowCastBarPet, "Show Battle Pet Nickname on Cast Bars", "Shows your Custom Nicknames on Cast bars.");
            DrawConfigElement(ref PluginLink.Configuration.useCustomFlyoutPet, "Show Battle Pet Nickname on Flyout Text", "Shows your Custom Nicknames on Flyout Text.");
            
            if (PenumbraValid)
            {
                Header("[Penumbra]");
                DrawConfigElement(ref PluginLink.Configuration.redrawBattlePetOnSpawn, "Redraw Battle Pet", new string[] { "Redraw a Battle Pet upon a Name Change (Fixes nameplate bugs).", "Requires [Penumbra]" });
            }

            EndElementBox();
        }
    }

    void AddNewLine()
    {
        if (!canDraw) return;
        currentHeight += ImGui.GetTextLineHeight() + ItemSpacingY;
        ImGui.Text("");
    }

    void Header(string title)
    {
        if (!canDraw) return;
        AddNewLine();
        currentHeight += BarSize + (ItemSpacingY * 2);
        NewLabel(title + $"##title{internalCounter++}", new Vector2(FillingWidthStepped(), BarSize));
    }

    public override void OnDrawSharing()
    {
        if (BeginElementBox("Sharing Settings"))
        {
            DrawConfigElement(ref PluginLink.Configuration.alwaysOpenAdvancedMode, "Always open Advanced Mode", "Holding [L-Shift] when Exporting Opens Advanced Mode. Is this Enabled, Advanced Mode will Always Open.");
            EndElementBox();
        }
    }

    public override void OnLateDraw()
    {
        if (ImGui.IsKeyDown(ImGuiKey.LeftShift) || DebugMode)
        {
            if (BeginElementBox("Debug Mode"))
            {
                DrawConfigElement(ref PluginLink.Configuration.debugMode, "Debug Mode", "Toggles Debug Mode");
                if(PluginLink.Configuration.debugMode)
                    DrawConfigElement(ref PluginLink.Configuration.autoOpenDebug, "Automatically open Debug Window", "Automatically open Debug Window on Plugin Startup.");
                EndElementBox();
            }
        }
    }

    public bool AnyIllegalsGoingOn()
    {
        if (PluginLink.Configuration.debugMode) return true;
        if (ImGui.IsKeyDown(ImGuiKey.LeftShift)) return true;

        // Comment out this line to unsupport all the Third Party support
        if (PenumbraIPCProvider.PenumbraEnabled()) return true;

        return false;
    }

    public bool PenumbraValid => (HasReadWarning && PenumbraIPCProvider.PenumbraEnabled()) || DebugMode;
    public bool HasReadWarning => PluginLink.Configuration.understoodWarningThirdPartySettings;
    public bool DebugMode => PluginLink.Configuration.debugMode;

    bool BeginElementBox(string title, bool forceOpen = false)
    {
        currentTitle = title;
        if (!toggles.ContainsKey(title))
            toggles.Add(title, (forceOpen ? forceOpen : PluginLink.Configuration.startSettingsOpen, 0));      
        (bool, float) curToggle = toggles[title];
        currentHeight = curToggle.Item2;
        bool outcome = BeginListBox($"##{title}[config]{internalCounter++}", new Vector2(ContentAvailableX, currentHeight));
        if (!outcome) return false;
        if (!forceOpen)
        {
            if (Button(toggles[title].Item1 ? "v" : ">", new Vector2(BarSize, BarSize)))
                curToggle.Item1 ^= true;
            canDraw = toggles[title].Item1;
            SameLine();
        }
        NewLabel(title + $"##title{internalCounter++}", new Vector2(ContentAvailableX, BarSize));
        currentHeight = BarSize + (ItemSpacingY * 2);
        toggles[title] = curToggle;
        return outcome;
    }

    void EndElementBox()
    {
        ImGui.EndListBox();
        canDraw = true;
        toggles[currentTitle] = (toggles[currentTitle].Item1, currentHeight);
        currentHeight = 0;
        if (PluginLink.Configuration.spaceOutSettings) NewLine();
    }

    void DrawConfigElement(ref bool value, string Title, string Description = "", string Tooltip = "", Action<bool> OnChange = null!) => DrawConfigElement(ref value, Title, new string[] { Description }, Tooltip, OnChange);
    void DrawConfigElement(ref bool value, string Title, string[] Description, string Tooltip = "", Action<bool> OnChange = null!)
    {
        if (!canDraw) return;
        float complete = 0;
        if (IsUnsafe.IsNullRef(ref value)) return;
        complete += DrawCheckbox(ref value, Title, OnChange);
        if (Tooltip == string.Empty || Tooltip == null) Tooltip = Title;
        SetTooltipHovered(Tooltip);
        complete += DrawDescriptionBox(Description);
        currentHeight += complete;
    }

    float DrawCheckbox(ref bool value, string Title, Action<bool> OnChange)
    {
        if (Checkbox(Title, ref value)) { PluginLink.Configuration.Save(); OnChange?.Invoke(value); }
        return ImGui.GetFrameHeight() + ItemSpacingY;
    }

    float DrawDescriptionBox(string[] Description)
    {
        float size = 0;
        
        foreach (string str in Description)
        {
            string newstr = starter + str;
            if (Description == null || newstr == starter) continue;
            ImGui.PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
            ImGui.TextWrapped(newstr);
            ImGui.PopStyleColor();
            size += ImGui.CalcTextSize(newstr, true, ImGui.GetItemRectSize().X).Y + ItemSpacingY;
            SetTooltipHovered(str);
        }

        if (size != 0 && PluginLink.Configuration.spaceOutSettings)
        {
            size += ImGui.GetTextLineHeight() + ItemSpacingY;
            ImGui.Text("");
        }
        
        return size;
    }
}