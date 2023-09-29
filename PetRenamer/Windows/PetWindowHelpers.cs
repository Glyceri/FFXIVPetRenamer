using Dalamud.Game.Text;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;
using System.Collections.Generic;
using System;
using System.Numerics;
using PetRenamer.Core.Networking.NetworkingElements;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core;
using PetRenamer.Utilization.UtilsModule;

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

    readonly List<(string, Type, string, Func<bool>)> helpButtons = new List<(string, Type, string, Func<bool>)>()
    {
        (SeIconChar.Triangle.ToIconString(),            typeof(DeveloperWindow),        "[Debug Window]", () => PluginLink.Configuration.debugMode),
        (SeIconChar.AutoTranslateOpen.ToIconString() + " " + SeIconChar.AutoTranslateClose.ToIconString(),   typeof(NewPetRenameWindow),        "[Give Nickname]", null!),
        (SeIconChar.BoxedQuestionMark.ToIconString(),   typeof(PetHelpWindow),          "[Help]", null!),
        (SeIconChar.MouseWheel.ToIconString(),          typeof(ConfigWindow),           "[Settings]", null!),
        (SeIconChar.Square.ToIconString(),              typeof(PetListWindow),          "[Pet/Minion List]", null!)
    };

    public sealed override void PreDraw()
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        PushStyleColor(ImGuiCol.TitleBg, StylingColours.titleBg);
        PushStyleColor(ImGuiCol.TitleBgActive, StylingColours.titleBgActive);
        PushStyleColor(ImGuiCol.TitleBgCollapsed, StylingColours.tileBgCollapsed);
    }

    protected void DrawModeToggle()
    {
        BeginListBox($"###ModeToggleBox{internalCounter++}", new Vector2(FillingWidth, BarSizePadded), StylingColours.titleBg);
        int pressed = DotBar();
        HelpBar();
        ImGui.EndListBox();

        if (pressed == -1) return;
        PetWindow.petMode = (PetMode)pressed;
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
        float buttonArea = validCount * (Styling.helpButtonSize.X + SpaceSize);
        float framePadding = FramePaddingX * 2;
        float windowPadding = WindowPaddingX * 2;
        ImGui.SameLine(0, FillingWidth - leftArea - buttonArea - framePadding + windowPadding);

        for (int i = 0; i < helpButtons.Count; i++)
        {
            if (!helpButtons[i].Item4?.Invoke() ?? false) continue;
            if (Button(helpButtons[i].Item1 + $"##{internalCounter++}", Styling.helpButtonSize, helpButtons[i].Item3)) PluginLink.WindowHandler.OpenWindow(helpButtons[i].Item2);
            SameLinePretendSpace();
        }
    }

    int ValidHelpButtonsCount()
    {
        int counter = 0;
        for (int i = 0; i < helpButtons.Count; i++)
        {
            if (!helpButtons[i].Item4?.Invoke() ?? false) continue;
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
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Button, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.idleColor);
        return ImGui.Button($"##{internalCounter++}toggleButtonBad", ToggleButtonStyle);
    }

    protected bool Button(string text, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
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
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool XButtonError(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.xButtonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.xButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.xButtonPressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.errorText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool XButton(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.xButtonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.xButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.xButtonPressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool RedownloadButton(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.titleBg);
        PushStyleColor(ImGuiCol.Button, StylingColours.listBox);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.defaultBackground);
        PushStyleColor(ImGuiCol.Text, StylingColours.readableBlueText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool TransparentLabel(Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0, 0));
        PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
        PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0, 0, 0));
        PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 0));
        bool returner = ImGui.Button(string.Empty, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool TransparentLabel(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0, 0));
        PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
        PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0, 0, 0));
        PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 0));
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool Label(string text, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Button, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Text, StylingColours.readableBlueText);
        bool returner = ImGui.Button(text);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool Label(string text, Vector2 styling, string tooltipText = "", Action callback = null!)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Button, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Text, StylingColours.readableBlueText);
        bool returner = ImGui.Button(text, styling);
        if (tooltipText != string.Empty) SetTooltipHovered(tooltipText);
        if (callback != null && returner) callback();
        return returner;
    }

    protected bool Label(string text, Vector4 textColour)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Button, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Text, textColour);
        return ImGui.Button(text);
    }

    protected bool Label(string text, Vector2 styling, Vector4 textColour)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Button, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Text, textColour);
        return ImGui.Button(text, styling);
    }

    protected bool NewLabel(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.button);
        PushStyleColor(ImGuiCol.Button, StylingColours.button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.button);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        return ImGui.Button(text, styling);
    }

    protected bool OverrideLabel(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.xButton);
        PushStyleColor(ImGuiCol.Button, StylingColours.xButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.xButton);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        return ImGui.Button(text, styling);
    }

    protected bool OverrideLabel(string text)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.xButton);
        PushStyleColor(ImGuiCol.Button, StylingColours.xButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.xButton);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        return ImGui.Button(text);
    }

    protected bool OverrideLabel(string text, Vector4 textColour)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.xButton);
        PushStyleColor(ImGuiCol.Button, StylingColours.xButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.xButton);
        PushStyleColor(ImGuiCol.Text, textColour);
        return ImGui.Button(text);
    }

    protected bool OverrideLabel(string text, Vector2 styling, Vector4 textColour)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.xButton);
        PushStyleColor(ImGuiCol.Button, StylingColours.xButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.xButton);
        PushStyleColor(ImGuiCol.Text, textColour);
        return ImGui.Button(text, styling);
    }

    protected bool Checkbox(string text, ref bool value) => Checkbox(text, string.Empty, ref value);

    protected bool Checkbox(string text, string tooltip, ref bool value)
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.whiteText);
        PushStyleColor(ImGuiCol.CheckMark, StylingColours.whiteText);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.xButtonHovered);
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.xButton);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.xButtonPressed);
        bool checkbox = ImGui.Checkbox(text, ref value);
        if (tooltip != string.Empty) SetTooltipHovered(tooltip);
        return checkbox;
    }

    protected void SetTooltipHovered(string text)
    {
        if (!ImGui.IsItemHovered()) return;
        SetTooltip(text);
    }

    protected void SetTooltip(string text)
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.whiteText);
        ImGui.SetTooltip(text);
    }

    static int popCount = 0;

    protected void PushStyleColor(ImGuiCol imGuiCol, Vector4 colour)
    {
        if (!PluginLink.Configuration.useCustomTheme) return;
        popCount++;
        ImGui.PushStyleColor(imGuiCol, colour);
    }

    protected void _PopAllStyleColours()
    {
        ImGui.PopStyleColor(popCount);
        popCount = 0;
    }

    protected bool BeginListBox(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ScrollbarGrab, StylingColours.button);
        PushStyleColor(ImGuiCol.ScrollbarGrabActive, StylingColours.buttonPressed);
        PushStyleColor(ImGuiCol.ScrollbarGrabHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.ScrollbarBg, StylingColours.scrollBarBG);
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.listBox);
        return ImGui.BeginListBox(text, styling);
    }

    protected bool BeginListBox(string text, Vector2 styling, Vector4 bgColour)
    {
        PushStyleColor(ImGuiCol.ScrollbarGrab, StylingColours.button);
        PushStyleColor(ImGuiCol.ScrollbarGrabActive, StylingColours.buttonPressed);
        PushStyleColor(ImGuiCol.ScrollbarGrabHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.ScrollbarBg, StylingColours.scrollBarBG);
        PushStyleColor(ImGuiCol.FrameBg, bgColour);
        return ImGui.BeginListBox(text, styling);
    }


    protected bool InputText(string label, ref string input, uint maxLength, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.textField);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.textFieldPressed);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.textFieldHovered);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        return ImGui.InputText(label, ref input, maxLength, flags);
    }

    protected bool InputTextMultiLine(string label, ref string input, uint maxLength, Vector2 size, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None, string tooltipText = "")
    {
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.textField);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.textFieldPressed);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.textFieldHovered);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
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
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(StylingColours.listBox));
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
        OverrideLabel(message, Styling.FillSize);
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
