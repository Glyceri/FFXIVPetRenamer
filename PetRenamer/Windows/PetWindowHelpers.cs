using Dalamud.Game.Text;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Networking.NetworkingElements;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.PetWindows;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.Windows;

/// <summary>
/// NEVER EXTEND FROM THIS
/// </summary>
public abstract class PetWindowHelpers : PetWindowStyling
{
    public PetWindowHelpers(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    protected readonly bool drawToggle = false;
    internal static int internalCounter = 0;

    readonly Dictionary<PetMode, string> tooltipStrings = new Dictionary<PetMode, string>()
    {
        { PetMode.Normal,       "[Minion Mode]" },
        { PetMode.BattlePet,    "[Battle Pet Mode]" },
        { PetMode.ShareMode,    "[Sharing Mode]" }
    };

    readonly List<(string, Type, string, Func<PetWindowHelpers, bool>)> helpButtons = new List<(string, Type, string, Func<PetWindowHelpers, bool>)>()
    { 
        (SeIconChar.Triangle.ToIconString(),            typeof(DeveloperWindow),        "[Debug Window]", (pw) => PluginLink.Configuration.debugMode),
        (SeIconChar.BoxedLetterL.ToIconString(),        typeof(ChangelogWindow),        "[Changelog]", (pw) => pw is ConfigWindow),
        (SeIconChar.BoxedLetterC.ToIconString(),        typeof(CreditsWindow),          "[Credits]", (pw) => pw is ConfigWindow),
        (SeIconChar.BoxedLetterT.ToIconString(),        typeof(ThemeEditorWindow),      "[Theme Editor]", (pw) => pw is ConfigWindow),
        (SeIconChar.BoxedQuestionMark.ToIconString(),   typeof(PetHelpWindow),          "[Help]", null!),
        (SeIconChar.MouseWheel.ToIconString(),          typeof(ConfigWindow),           "[Settings]", null!),
        (SeIconChar.AutoTranslateOpen.ToIconString() + " " + SeIconChar.AutoTranslateClose.ToIconString(),   typeof(PetRenameWindow),        "[Give Nickname]", null!),
        (SeIconChar.Square.ToIconString(),              typeof(PetListWindow),          "[Pet/Minion List]", null!),
    };

    Vector2 oldPadding;
    Vector2 oldWindowPadding;
    Vector2 oldCellPadding;
    Vector2 oldItemSpacing;
    Vector2 oldItemInnerSpacing;
    Vector2 touchExtraPadding;
    float indentSpacing;
    float scrollbarSize;
    float grabMinSize;

    public sealed override void PreDraw()
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.alternativeText);
        PushStyleColor(ImGuiCol.TitleBg, StylingColours.panelColour);
        PushStyleColor(ImGuiCol.TitleBgActive, StylingColours.titleBgActive);
        PushStyleColor(ImGuiCol.TitleBgCollapsed, StylingColours.tileBgCollapsed);

        ImGuiStylePtr ptr = ImGui.GetStyle();

        oldPadding = ptr.FramePadding;
        oldWindowPadding = ptr.WindowPadding;
        oldCellPadding = ptr.CellPadding;
        oldItemSpacing = ptr.ItemSpacing;
        oldItemInnerSpacing = ptr.ItemInnerSpacing;
        touchExtraPadding = ptr.TouchExtraPadding;
        indentSpacing = ptr.IndentSpacing;
        scrollbarSize = ptr.ScrollbarSize;
        grabMinSize = ptr.GrabMinSize;

        ptr.FramePadding = new Vector2(4, 3);
        ptr.WindowPadding = new Vector2(8, 8);
        ptr.CellPadding = new Vector2(4, 4);
        ptr.ItemSpacing = new Vector2(8, 4);
        ptr.ItemInnerSpacing = new Vector2(4, 4);
        ptr.TouchExtraPadding = new Vector2(0, 2);
        ptr.IndentSpacing = 21;
        ptr.ScrollbarSize = 16;
        ptr.GrabMinSize = 10;
    }

    protected void PostDrawHelper() 
    {
        ImGuiStylePtr ptr = ImGui.GetStyle();

        ptr.FramePadding = oldPadding;
        ptr.WindowPadding = oldWindowPadding;
        ptr.CellPadding = oldCellPadding;
        ptr.ItemSpacing = oldItemSpacing;
        ptr.ItemInnerSpacing = oldItemInnerSpacing;
        ptr.TouchExtraPadding = touchExtraPadding;
        ptr.IndentSpacing = indentSpacing;
        ptr.ScrollbarSize = scrollbarSize;
        ptr.GrabMinSize = grabMinSize;
    }

    protected void DrawModeToggle()
    {
        if (!BeginListBox($"###ModeToggleBox{internalCounter++}", new Vector2(ContentAvailableX, BarSizePadded))) return;
        int pressed = DotBar();
        HelpBar();
        ImGui.EndListBox();
        if (pressed != -1) (this as PetWindow)!.SetPetMode((PetMode)pressed);
    }

    int DotBar()
    {
        int pressed = -1;

        ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPosX(), ImGui.GetCursorPosY() + (ToggleButtonStyle.Y * 0.5f)));
        for (int i = 0; i < (int)PetMode.COUNT; i++)
        {
            if (((int)PetWindow.petMode == i) ? ToggleButton() : ToggleButtonBad()) pressed = i;
            SetTooltipHovered(tooltipStrings[(PetMode)i]);
            SameLineNoMargin();
        }
        ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPosX(), ImGui.GetCursorPosY() - (ToggleButtonStyle.Y * 0.5f)));
        Continue();
        return pressed;
    }

    void HelpBar()
    {
        int validCount = ValidHelpButtonsCount();

        float leftArea = ToggleButtonStyle.X * (int)PetMode.COUNT;
        float buttonArea = validCount * (Styling.SmallButton.X + SpaceSize) - SpaceSize;
        float framePadding = FramePaddingX * 2;
        float windowPadding = WindowPaddingX * 4;
        ImGui.SameLine(0, FillingWidth - leftArea - buttonArea - framePadding + windowPadding);

        for (int i = 0; i < helpButtons.Count; i++)
        {
            if (!helpButtons[i].Item4?.Invoke(this) ?? false) continue;
            if (Button(helpButtons[i].Item1 + $"##{internalCounter++}", Styling.SmallButton, helpButtons[i].Item3))
            {
                if (PluginLink.Configuration.quickButtonsToggle) PluginLink.WindowHandler.ToggleWindow(helpButtons[i].Item2);
                else PluginLink.WindowHandler.OpenWindow(helpButtons[i].Item2);
            }
            SameLinePretendSpace();
        }
    }

    int ValidHelpButtonsCount()
    {
        int counter = 0;
        for (int i = 0; i < helpButtons.Count; i++)
        {
            if (!helpButtons[i].Item4?.Invoke(this) ?? false) continue;
            counter++;
        }
        return counter;
    }

    protected void TextColoured(Vector4 colour, string text) => ImGui.TextColored(colour, text);

    protected bool ToggleButton()
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonPressed);
        return ImGui.Button($"##{internalCounter++}toggleButton", ToggleButtonStyle);
    }

    protected bool ToggleButtonBad()
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.basicLabelColour);
        PushStyleColor(ImGuiCol.Button, StylingColours.basicLabelColour);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.basicLabelColour);
        return ImGui.Button($"##{internalCounter++}toggleButtonBad", ToggleButtonStyle);
    }

    protected bool Button(string text, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.alternativeText);
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonPressed);
        bool returner = ImGui.Button(text);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool Button(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonPressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.alternativeText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool XButtonError(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonAlternativeHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.buttonAlternative);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonAlternativePressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool XButton(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonAlternativeHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.buttonAlternative);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonAlternativePressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.alternativeText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool RedownloadButton(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.panelColour);
        PushStyleColor(ImGuiCol.Button, StylingColours.panelColour);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.defaultBackground);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool TransparentLabel(Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColours(new Vector4(0, 0, 0, 0), LabelColours);
        PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 0));
        bool returner = ImGui.Button(string.Empty, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool TransparentLabel(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColours(new Vector4(0, 0, 0, 0), LabelColours);
        PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 0));
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool Label(string text, string tooltipText = "", Action callback = null!)
    {
        PushStyleColours(StylingColours.basicLabelColour, LabelColours);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        bool returner = ImGui.Button(text);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool Label(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColours(StylingColours.basicLabelColour, LabelColours);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool Label(string text, Vector4 textColour)
    {
        PushStyleColours(StylingColours.basicLabelColour, LabelColours);
        PushStyleColor(ImGuiCol.Text, textColour);
        return ImGui.Button(text);
    }

    protected bool Label(string text, Vector2 styling, Vector4 textColour)
    {
        PushStyleColours(StylingColours.basicLabelColour, LabelColours);
        PushStyleColor(ImGuiCol.Text, textColour);
        return ImGui.Button(text, styling);
    }

    protected bool NewLabel(string text, Vector2 styling)
    {
        PushStyleColours(StylingColours.button, LabelColours);
        PushStyleColor(ImGuiCol.Text, StylingColours.alternativeText);
        return ImGui.Button(text, styling);
    }

    protected bool OverrideLabel(string text, Vector2 styling)
    {
        PushStyleColours(StylingColours.buttonAlternative, LabelColours);
        PushStyleColor(ImGuiCol.Text, StylingColours.alternativeText);
        return ImGui.Button(text, styling);
    }

    protected bool Checkbox(string text, ref bool value) => Checkbox(text, string.Empty, ref value);

    protected bool Checkbox(string text, string tooltip, ref bool value)
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        PushStyleColor(ImGuiCol.CheckMark, StylingColours.defaultText);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.buttonAlternativeHovered);
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.buttonAlternative);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.buttonAlternativePressed);
        bool checkbox = ImGui.Checkbox(text, ref value);
        if (tooltip != string.Empty) SetTooltipHovered(tooltip);
        return checkbox;
    }

    protected void SetTooltipHovered(string text)
    {
        if (text == string.Empty || text == null) return;
        if (!ImGui.IsItemHovered()) return;
        PushStyleColor(ImGuiCol.Text, StylingColours.tooltipText);
        SetTooltip(text);
    }

    protected void SetTooltip(string text)
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.tooltipText);
        ImGui.SetTooltip(text);
    }

    static int popCount = 0;

    protected void PushStyleColor(ImGuiCol imGuiCol, Vector4 colour)
    {
        popCount++;
        ImGui.PushStyleColor(imGuiCol, colour);
    }

    protected void PushStyleColours(Vector4 colour, params ImGuiCol[] colours)
    {
        foreach (ImGuiCol col in colours)
        {
            popCount++;
            ImGui.PushStyleColor(col, colour);
        }
    }

    protected void _PopAllStyleColours()
    {
        ImGui.PopStyleColor(popCount);
        popCount = 0;
    }

    protected bool BeginListBox(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        PushStyleColor(ImGuiCol.ScrollbarGrab, StylingColours.button);
        PushStyleColor(ImGuiCol.ScrollbarGrabActive, StylingColours.buttonPressed);
        PushStyleColor(ImGuiCol.ScrollbarGrabHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.ScrollbarBg, StylingColours.panelColour);
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.panelColour);
        return ImGui.BeginListBox(text, styling);
    }

    protected bool InputTextMultiLine(string label, ref string input, uint maxLength, Vector2 size, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, string tooltipText = "")
    {
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.buttonAlternative);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.buttonAlternativePressed);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.buttonAlternativeHovered);
        PushStyleColor(ImGuiCol.Text, StylingColours.alternativeText);
        bool returnable = ImGui.InputTextMultiline(label, ref input, maxLength, size, flags);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        return returnable;
    }

    protected void DrawRedownloadButton(Action drawMe)
    {
        if (drawMe == null) return;
        ImGui.SetItemAllowOverlap();
        SameLine();
        ImGui.SetCursorPos(ImGui.GetCursorPos() - new Vector2(35, -59));
        drawMe.Invoke();
    }

    protected void DrawRedownloadButton(PettableUser u)
    {
        RedownloadButton(SeIconChar.QuestSync.ToIconString() + $"##<Redownload>{internalCounter++}",
            Styling.SmallButton,
            $"Redownload profile picture for: {u.UserName}@{SheetUtils.instance.GetWorldName(u.Homeworld)}",
            () => ProfilePictureNetworked.instance.RequestDownload((u.UserName, u.Homeworld)));
    }

    protected void DrawAdvancedBarWithQuit(string label, string value, Action callback, string quitText = "", string quitTooltip = "", Action callback2 = null!)
    {
        DrawBasicLabel(label);
        Button($"          {value} ##<{internalCounter++}>", new Vector2(480, 25), $"{label}: {value.Trim()}", callback.Invoke);
        if (callback2 == null) return;
        SameLinePretendSpace();
        XButton(quitText + $"##<Close{internalCounter++}>", Styling.SmallButton, quitTooltip, callback2.Invoke);
    }

    protected void DrawYesNoBar(string label, Action yesCallback, Action noCallback)
    {
        TransparentLabel("", new Vector2(1, 25));
        Label(label + $"##<{internalCounter++}>", new Vector2(508, 25));
        SameLinePretendSpace2();
        Button("Yes", Styling.ListIDField, PluginConstants.Strings.deleteUserTooltip, yesCallback.Invoke);
        SameLinePretendSpace();
        Button("No", Styling.ListIDField, PluginConstants.Strings.keepUserTooltip, noCallback.Invoke);
    }

    protected void DrawBasicBar(string label, string value)
    {
        DrawBasicLabel(label);
        Label(value.ToString() + $"##<{internalCounter++}>", new Vector2(508, 25));
        SetTooltipHovered($"{label}: {value}");
    }

    protected void DrawBasicLabel(string label)
    {
        Label(label, Styling.ListButton);
        SameLinePretendSpace2();
    }

    protected void SetColumn(int column)
    {
        if (column == 0) ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(column);
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(StylingColours.panelColour));
    }

    protected void DrawFillerBar(int columns)
    {
        ImGui.TableNextRow();
        for (int i = 0; i < columns; i++)
        {
            ImGui.TableSetColumnIndex(i);
            ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 1)));
        }
    }

    protected void DrawWarningLabel(string message, string buttonIcon, string buttonTooltip, Action onButton)
    {
        OverrideLabel(message, new Vector2(ContentAvailableX - Styling.SmallButton.X - FramePaddingX * 2, BarSize));
        SameLinePretendSpace();
        if (XButton(buttonIcon + "##<SafetySettings>", Styling.SmallButton))
            onButton?.Invoke();
        SetTooltipHovered(buttonTooltip);
    }

    protected void DrawEmptyBar(string buttonString, string tooltipString, Action callback, bool check = true)
    {
        TransparentLabel(Styling.ListButton);
        SameLinePretendSpace(); SameLinePretendSpace();
        TransparentLabel($"##<Transparent{internalCounter++}>", new Vector2(480, 25));
        SameLinePretendSpace();
        if (!check) return;
        XButton(buttonString + $"##<Close{internalCounter++}>", Styling.SmallButton, tooltipString, callback.Invoke);
    }

    protected void DrawImage(nint handle, Vector2 size)
    {
        if (PluginLink.Configuration.displayImages) ImGui.Image(handle, size);
        else
        {
            PushStyleColours(StylingColours.defaultBackground, ImGuiCol.Button | ImGuiCol.ButtonActive | ImGuiCol.ButtonHovered);
            Button("", size);
        }
    }

    protected void SpaceBottomRightButton()
    {
        NewLine();
        ImGui.SameLine(638);
    }

    protected void SameLine() => ImGui.SameLine();
    protected void SameLineNoMargin() => ImGui.SameLine(0, 0);
    protected void SameLineMinimalMargin() => ImGui.SameLine(0, 0.5f);
    protected void SameLinePretendSpace() => ImGui.SameLine(0, SpaceSize);
    protected void SameLinePretendSpace2() { SameLinePretendSpace(); SameLinePretendSpace(); }
    protected void NewLine() => ImGui.NewLine();
    protected void Continue()
    {
        NewLine();
        SameLineNoMargin();
    }
}
