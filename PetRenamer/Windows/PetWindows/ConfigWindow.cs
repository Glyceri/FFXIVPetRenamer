using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.MappyIPC;
using PetRenamer.Windows.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
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

    const string starterUnselected = "  ○   ";
    const string starter = "  ◉   ";
    const string spacing = "      ";
    float currentHeight = 0;
    bool canDraw = true;
    readonly Dictionary<string, (bool, float)> toggles = new Dictionary<string, (bool, float)>();
    string currentTitle = string.Empty;

    public override void OnDraw()
    {
        anythingIllegals = AnyIllegalsGoingOn();

        if ((anythingIllegals && !PluginLink.Configuration.understoodWarningThirdPartySettings) || DebugMode) DrawWarningThing();

        if (BeginElementBox("UI Settings"))
        {
            DrawConfigElement(ref PluginLink.Configuration.activeElement, PluginLink.ToolbarAnimator.registeredIdentifiers, "Toolbar Animation", new string[] { "Select which toolbar animation you would like to play." }, "Toolbar Animation.", PluginLink.ToolbarAnimator.RegisterActiveAnimation);
            //DrawConfigElement(ref PluginLink.Configuration.allowSnow, "Allow Snow on Toolbar", new string[] { "Shows snow on the toolbar." }, "Allow Snow on Toolbar.");
            DrawConfigElement(ref PluginLink.Configuration.anonymousMode, "Anonymous mode", new string[] { "Hides player names and replaces them with initials.", "Disables profile pictures." }, "Anonymous mode.");
            DrawConfigElement(ref PluginLink.Configuration.newUseCustomTheme, "Use Custom Theme", new string[] { "You Can Make Your Own Theme, Click Here To Enable That Feature.", "Open using [/pettheme] [/miniontheme]" }, "Use Custom Theme [/pettheme]", OnChange: (value) => SetTheme() );
            DrawConfigElement(ref PluginLink.Configuration.displayImages, "Display Images", "Display Images or Replace them with a Flat Colour?", "Display Images"); 
            DrawConfigElement(ref PluginLink.Configuration.spaceOutSettings, "Space Out Settings", "Spaces Out Settings making it Easier to Read.");
            DrawConfigElement(ref PluginLink.Configuration.startSettingsOpen, "Start with all the settings unfolded", "Upon Starting the Plugin, Automatically Unfold all Setting Panels.");
            DrawConfigElement(ref PluginLink.Configuration.quickButtonsToggle, "Quick Buttons Toggle Instead of Open", "The Quick Buttons in the Top Bar Toggle Instead of Open.");
            //DrawConfigElement(ref PluginLink.Configuration.showKofiButton, "Show Ko-fi button");
            EndElementBox();
        }
        if (BeginElementBox("Global Settings"))
        {
            DrawConfigElement(ref PluginLink.Configuration.displayCustomNames, "Display Custom Nicknames", new string[] { "Completely Enables or Disables Custom Nicknames.", "Prevents Most parts of the Plugin from Working!" });
            DrawConfigElement(ref PluginLink.Configuration.automaticallySwitchPetmode, "Automatically Switch Pet Mode", "Upon Summoning a Minion or Battle Pet, Automatically Switch Pet Mode?");
            DrawConfigElement(ref PluginLink.Configuration.downloadProfilePictures, "Automatically Download Profile Pictures", "Upon Importing a User (or yourself). Automatically Download their Profile Picture?");
            EndElementBox();
        }

        if ((anythingIllegals && PluginLink.Configuration.understoodWarningThirdPartySettings) || DebugMode)
        {
            if (BeginElementBox("Third Party Settings", false))
            {
                if (IPCMappy.MappyAvailable)
                    DrawConfigElement(ref PluginLink.Configuration.enableMappyIntegration, "Enable Mappy Integration", new string[] { "Allows Pet Nicknames to display Custom Names on Mappy Pets" });
                EndElementBox();
            }
        }
    }

    public void DrawWarningThing()
    {
        if (BeginElementBox("Third Party WARNING", true))
        {
            DrawConfigElement(ref PluginLink.Configuration.understoodWarningThirdPartySettings, "I UNDERSTAND!", new string[] { "Integration with Third Party Plugins may cause issues that are BEYOND MY CONTROL!", "Some third party settings will drastically lower performance.", "I tested every interaction well, but use at your own risk." }, "READ THE WARNING!");
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
            EndElementBox();
        }
    }

    void AddNewLine()
    {
        if (!canDraw) return;
        currentHeight += ImGui.GetTextLineHeight() + ItemSpacingY;
        ImGui.Text("");
    }

    void Header(string title, bool doNewLine = true, string tooltip = "")
    {
        if (!canDraw) return;
        currentHeight += BarSize + ItemSpacingY;
        NewLabel(title + $"##title{internalCounter++}", new Vector2(FillingWidthStepped(), BarSize));
        if (tooltip == string.Empty) SetTooltipHovered(title);
        else SetTooltipHovered(tooltip);
        if (doNewLine) AddNewLine();
    }

    public override void OnDrawSharing()
    {
        if (BeginElementBox("Sharing Settings"))
        {
            DrawConfigElement(ref PluginLink.Configuration.alwaysOpenAdvancedMode, "Always open Advanced Mode", "Holding [L-Shift] when Exporting Opens Advanced Mode. Is this Enabled, Advanced Mode will Always Open.");
            EndElementBox();
        }
    }

    void DrawPerformanceSettings()
    {
        if (BeginElementBox("Advanced Performance Settings", false, "Advanced Performance Settings\n[THESE ALL REQUIRE A PLUGIN RESTART]"))
        {
            Header("[THESE ALL REQUIRE A PLUGIN RESTART]", false, "(have you tried turning it off and on again)");
            EndElementBox();
        }
    }

    public override void OnLateDraw()
    {
        //DrawPerformanceSettings();
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
        if (IPCMappy.MappyAvailable) return true;

        return false;
    }

    public bool HasReadWarning => PluginLink.Configuration.understoodWarningThirdPartySettings;
    public bool DebugMode => PluginLink.Configuration.debugMode;

    bool BeginElementBox(string title, bool forceOpen = false, string tooltip = "")
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
        if (tooltip == string.Empty) SetTooltipHovered(title);
        else SetTooltipHovered(tooltip);
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

    void DrawConfigElement(ref string chosenElement, string[] elements, string title, string Description, string Tooltip = "", Action<string> OnChange = null!) => DrawConfigElement(ref chosenElement, elements, title, new string[] { Description }, Tooltip, OnChange);
    void DrawConfigElement(ref string chosenElement, string[] elements, string title, string[] Description, string Tooltip = "", Action<string> OnChange = null!)
    {
        if (!canDraw) return;
        float complete = 0;
        if (IsUnsafe.IsNullRef(ref chosenElement)) return;
        bool alwaysFalseValue = false;
        complete += DrawCheckbox(ref alwaysFalseValue, title, (val) => { });
        if (Tooltip == string.Empty || Tooltip == null) Tooltip = title;
        SetTooltipHovered(Tooltip);
        complete += DrawDescriptionBox(Description);
        foreach (string element in elements)
        {
            complete += DrawText(element, element == chosenElement ? spacing + starter : spacing + starterUnselected);
            if (ImGui.IsItemClicked() && chosenElement != element)
            {
                chosenElement = element;
                PluginLink.Configuration.Save();
                OnChange?.Invoke(chosenElement);    
            }
        }
        currentHeight += complete;  
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

    float DrawDescriptionBox(string[] Description, string starterString = starter)
    {
        float size = 0;

        foreach (string str in Description)
        {
            if (Description == null || str == string.Empty) continue;
            size += DrawText(str, starterString);
        }

        if (size != 0 && PluginLink.Configuration.spaceOutSettings)
        {
            size += ImGui.GetTextLineHeight() + ItemSpacingY;
            ImGui.Text("");
        }
        
        return size;
    }

    float DrawText(string str, string starterString)
    {
        string newstr = starterString + str;
        ImGui.PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        ImGui.TextWrapped(newstr);
        ImGui.PopStyleColor();
        float size = ImGui.CalcTextSize(newstr, true, ImGui.GetItemRectSize().X).Y + ItemSpacingY;
        SetTooltipHovered(str);
        return size;
    }
}