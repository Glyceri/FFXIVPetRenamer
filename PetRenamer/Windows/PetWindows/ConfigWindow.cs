using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using PetRenamer.Windows.Attributes;
using static PetRenamer.Windows.PetWindowStyling;
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

    internal static float currentHeight = 0;
    internal static bool canDraw = true;
    internal static readonly Dictionary<string, (bool, float)> toggles = new Dictionary<string, (bool, float)>();
    internal static string currentTitle = string.Empty;

    public override void OnDraw()
    {
        anythingIllegals = AnyIllegalsGoingOn();

        if ((anythingIllegals && !PluginLink.Configuration.understoodWarningThirdPartySettings) || DebugMode)
        {
            if (BeginElementBox("Third Party WARNING", true))
            {
                new ConfigElement(ref PluginLink.Configuration.understoodWarningThirdPartySettings, "I UNDERSTAND!", new string[] { "Do NOT send feedback or issues for these settings on discord or via the official [Send Feedback] button!", "ONLY Github Issues are ALLOWED!", "Enables Settings Related to Other Plugins (Main Repo or Not)!" }, "READ THE WARNING!");
                EndElementBox();
            }
        }

        if (BeginElementBox("UI Settings"))
        {
            new ConfigElement(ref PluginLink.Configuration.spaceOutSettings, "Space Out Settings", "Spaces Out Settings making it Easier to Read.", "Space Out Settings");
            new ConfigElement(ref PluginLink.Configuration.startSettingsOpen, "Start with all the settings open", "Upon Starting the Plugin, Automatically Open all Setting Panels.", "Start with all the settings open");
            new ConfigElement(ref PluginLink.Configuration.quickButtonsToggle, "Quick Buttons Toggle Instead of Open", "The Quick Buttons in the Top Bar Toggle Instead of Open.", "Quick Buttons Toggle Instead of Open");
            EndElementBox();
        }
        if (BeginElementBox("Global Settings"))
        {
            new ConfigElement(ref PluginLink.Configuration.displayCustomNames, "Display Custom Nicknames", new string[] { "Completely Enables or Disables Custom Nicknames.", "Prevents Most parts of the Plugin from Working!" }, "Display Custom Nicknames");
            new ConfigElement(ref PluginLink.Configuration.displayImages, "Display Images", "Display Images or Replace them with a Flat Colour?", "Display Images");
            new ConfigElement(ref PluginLink.Configuration.automaticallySwitchPetmode, "Automatically Switch Pet Mode", "Upon Summoning a Minion or Battle Pet, Automatically Switch Pet Mode?", "Automatically Switch Pet Mode");
            new ConfigElement(ref PluginLink.Configuration.downloadProfilePictures, "Automatically Download Profile Pictures", "Upon Importing a User (or yourself). Automatically Download their Profile Picture?", "Automatically Download Profile Pictures");
            EndElementBox();
        }
    }

    public override void OnDrawNormal()
    {
        if (BeginElementBox("Minion Settings"))
        {
            new ConfigElement(ref PluginLink.Configuration.useContextMenuOnMinions, "Allow Context Menus for Minions", "Rightclicking on a Minion will add the [Give Nickname] Option.", "Allow Context Menus for Minions");
            new ConfigElement(ref PluginLink.Configuration.allowTooltipsOnMinions, "Allow Tooltips for Minions", "Display Minion Nicknames in Tooltips.", "Allow Tooltips for Minions");
            new ConfigElement(ref PluginLink.Configuration.replaceEmotesOnMinions, "Allow Custom Nicknames in Emotes for Minions", "Replace a Minions in-game Name with your Custom Nickname.", "Allow Custom Nicknames in Emotes for Minions");
            new ConfigElement(ref PluginLink.Configuration.showNamesInMinionBook, "Show Nicknames in the Minion Journal", "Shows your Custom Nicknames in the Minion Journal.", "Show Nicknames in the Minion Journal");

            if (PenumbraValid)
            {
                Header("[Penumbra]");
                new ConfigElement(ref PluginLink.Configuration.redrawMinionOnSpawn, "Redraw Minion", new string[] { "Redraw a Minion upon a Name Change (Fixes nameplate bugs).", "Requires [Penumbra]" }, "Redraw Minion");
            }

            EndElementBox();
        }
    }

    public override void OnDrawBattlePet()
    {
        if (BeginElementBox("Battle Pet Settings"))
        {
            new ConfigElement(ref PluginLink.Configuration.useContextMenuOnBattlePets, "Allow Context Menus for Battle Pets", "Rightclicking on a Battle Pet will add the [Give Nickname] Option.", "Allow Context Menus for Battle Pets");
            new ConfigElement(ref PluginLink.Configuration.allowTooltipsBattlePets, "Allow Tooltips for Battle Pets", "Display Battle Pet Nicknames in Tooltips.", "Allow Tooltips for Battle Pets");
            new ConfigElement(ref PluginLink.Configuration.replaceEmotesBattlePets, "Allow Custom Nicknames in Emotes for Battle Pets", "Replace a Battle Pet in-game Name with your Custom Nickname.", "Allow Custom Nicknames in Emotes for Battle Pets");
            new ConfigElement(ref PluginLink.Configuration.useCustomPetNamesInBattleChat, "Allow Custom Nicknames in the Battle Log for Battle Pets", "Replace a Battle Pet in-game Name with your Custom Nickname.", "Allow Custom Nicknames in the Battle Log for Battle Pets");
            new ConfigElement(ref PluginLink.Configuration.allowCastBarPet, "Show Battle Pet Nickname on Cast Bars", "Shows your Custom Nicknames on Cast bars.", "Show Battle Pet Nickname on Cast Bars");
            new ConfigElement(ref PluginLink.Configuration.useCustomFlyoutPet, "Show Battle Pet Nickname on Flyout Text", "Shows your Custom Nicknames on Flyout Text.", "Show Battle Pet Nickname on Flyout Text");
            
            if (PenumbraValid)
            {
                Header("[Penumbra]");
                new ConfigElement(ref PluginLink.Configuration.redrawBattlePetOnSpawn, "Redraw Battle Pet", new string[] { "Redraw a Battle Pet upon a Name Change (Fixes nameplate bugs).", "Requires [Penumbra]" }, "Redraw Battle Pet");
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
            new ConfigElement(ref PluginLink.Configuration.alwaysOpenAdvancedMode, "Always open Advanced Mode", "Holding [L-Shift] when Exporting Opens Advanced Mode. Is this Enabled, Advanced Mode will Always Open.", "Always open Advanced Mode");
            EndElementBox();
        }
    }

    public override void OnLateDraw()
    {
        if (ImGui.IsKeyDown(ImGuiKey.LeftShift) || DebugMode)
        {
            if (BeginElementBox("Debug Mode"))
            {
                new ConfigElement(ref PluginLink.Configuration.debugMode, "Debug Mode", "Toggles Debug Mode", "Debug Mode");
                if(PluginLink.Configuration.debugMode)
                {
                    new ConfigElement(ref PluginLink.Configuration.autoOpenDebug, "Automatically open Debug Window", "Automatically open Debug Window on Plugin Startup.", "Automatically open Debug Window");
                }
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
        bool outcome = BeginListBox($"##{title}[config]{internalCounter++}", new Vector2(ContentAvailableX, currentHeight), StylingColours.titleBg);
        if (!outcome) return false;
        if (!forceOpen)
        {
            if (ImGui.Button(toggles[title].Item1 ? "v" : ">", new Vector2(BarSize, BarSize)))
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
}

internal readonly ref struct ConfigElement
{
    internal readonly ref bool value;
    internal readonly string Title = string.Empty;
    internal readonly string[] Description = Array.Empty<string>();
    internal readonly string Tooltip = string.Empty;
    internal readonly Action<bool> OnToggle = null!;
    internal readonly Func<bool> Allowed = null!;

    public ConfigElement(ref bool value, string Title, string Description = "", string Tooltip = "", Action<bool> OnToggle = null!, Func<bool> Allowed = null!) : this (ref value, Title, new string[] { Description }, Tooltip, OnToggle, Allowed) { }
    public ConfigElement(ref bool value, string Title, string[] Description, string Tooltip = "", Action<bool> OnToggle = null!, Func<bool> Allowed = null!)
    {
        this.value = ref value;
        this.Title = Title;
        if (Description == null) this.Description = Array.Empty<string>();
        else
        {
            List<string> strs = new List<string>();
            foreach(string str in Description)
                strs.Add(starter + str);
            this.Description = strs.ToArray();
        }
        this.Tooltip = Tooltip;
        this.OnToggle = OnToggle;
        this.Allowed = Allowed;
        Draw();
    }

    void Draw()
    {
        if (!ConfigWindow.canDraw) return;
        float complete = 0;
        if (IsUnsafe.IsNullRef(ref value)) return;
        complete += DrawCheckbox(); 
        ImGui.PushStyleColor(ImGuiCol.Text, StylingColours.whiteText);
        if (Tooltip != null && Tooltip != string.Empty && ImGui.IsItemHovered())
            ImGui.SetTooltip(Tooltip);
        ImGui.PopStyleColor();
        complete += DrawDescriptionBox();
        ConfigWindow.currentHeight += complete;
    }


    float DrawCheckbox()
    {
        ImGui.PushStyleColor(ImGuiCol.Text, StylingColours.whiteText);
        ImGui.PushStyleColor(ImGuiCol.CheckMark, StylingColours.defaultText);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.xButtonHovered);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, StylingColours.xButton);
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.xButtonPressed);
        if (ImGui.Checkbox(Title, ref value)) PluginLink.Configuration.Save();
        ImGui.PopStyleColor(5);
        return ImGui.GetFrameHeight() + ItemSpacingY;
    }

    float DrawDescriptionBox()
    {
        float size = 0;
        ImGui.PushStyleColor(ImGuiCol.Text, StylingColours.whiteText);
        foreach (string str in Description)
        {
            if (Description == null || str == starter) continue;
            ImGui.TextWrapped(str);
            size+= ImGui.CalcTextSize(str, true, ImGui.GetItemRectSize().X).Y + ItemSpacingY;
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(str.Replace(starter, string.Empty));
        }
        
        if (size != 0 && PluginLink.Configuration.spaceOutSettings)
        {
            size += ImGui.GetTextLineHeight() + ItemSpacingY;
            ImGui.Text("");
        }
        ImGui.PopStyleColor();
        return size;
    }

    const string starter = "  ◉   ";
}
