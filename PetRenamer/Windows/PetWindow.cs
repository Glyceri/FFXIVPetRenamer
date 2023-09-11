using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.AutoRegistry.Interfaces;
using PetRenamer.Core.Handlers;
using PetRenamer.Theming;
using PetRenamer.Windows.PetWindows;
using System;
using System.Collections.Generic;

namespace PetRenamer.Windows;

public abstract class PetWindow : Window, IDisposableRegistryElement
{
    internal static PetMode _petMode = PetMode.Normal;
    internal static PetMode petMode { get => _petMode; 
        set 
        { 
            _petMode = value;
            if (petMode == PetMode.Normal)  ThemeHandler.SetTheme(ThemeHandler.baseTheme);
            else if (petMode == PetMode.BattlePet) ThemeHandler.SetTheme(ThemeHandler.greenTheme);
            else if (petMode == PetMode.ShareMode) ThemeHandler.SetTheme(ThemeHandler.redTheme);
        } 
    }

    internal static void SetPetMode(PetMode mode) => petMode = mode;

    protected PetWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    public void Dispose() => OnDispose();
    protected virtual void OnDispose() { }

    protected readonly bool drawToggle = false;

    readonly Dictionary<PetMode, string> tooltipStrings = new Dictionary<PetMode, string>()
    {
        { PetMode.Normal,       "[Minion Mode]" },
        { PetMode.BattlePet,    "[Battle Pet Mode]" },
        { PetMode.ShareMode,    "[Sharing Mode]" }
    };

    readonly List<(string, Type, string)> helpButtons = new List<(string, Type, string)>()
    {
        (SeIconChar.BoxedQuestionMark.ToIconString(),   typeof(PetHelpWindow),  "[Help]"),
        (SeIconChar.MouseWheel.ToIconString(),          typeof(ConfigWindow),   "[Settings]"),
        (SeIconChar.Square.ToIconString(),              typeof(PetListWindow),  "[Pet/Minion List]")
    };

    public sealed override void PreDraw()
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        PushStyleColor(ImGuiCol.TitleBg, StylingColours.titleBg);
        PushStyleColor(ImGuiCol.TitleBgActive, StylingColours.titleBgActive);
        PushStyleColor(ImGuiCol.TitleBgCollapsed, StylingColours.tileBgCollapsed);
    }

    public sealed override unsafe void Draw()
    {
        if (drawToggle) DrawModeToggle();
        OnDraw();
        if (petMode == PetMode.Normal) OnDrawNormal();
        else if (petMode == PetMode.BattlePet) OnDrawBattlePet();
        else OnDrawSharing();
        OnLateDraw();
        PopAllStyleColours();
    }

    public sealed override void PostDraw()
    {
        PopAllStyleColours();
    }

    public unsafe virtual void OnDraw() { }
    public unsafe virtual void OnDrawNormal() { }
    public unsafe virtual void OnDrawBattlePet() { }
    public unsafe virtual void OnDrawSharing() { }
    public unsafe virtual void OnLateDraw() { }

    public static class Styling
    {
        public static Vector2 ListButton = new Vector2(150, 25);
        public static Vector2 ListNameButton = new Vector2(480, 25);
        public static Vector2 ListIDField = new Vector2(75, 25);
        public static Vector2 SmallButton = new Vector2(25, 25);
        public static Vector2 ListSmallNameField = new Vector2(200, 25);
        public static Vector2 FillSize = new Vector2(755, 25);
        public static Vector2 FillSizeSmall = new Vector2(746, 25);
        public static Vector2 FillSizeFull = new Vector2(782, 25);
        public static Vector2 FillSizeFullSmall = new Vector2(774, 25);

        public static Vector2 ToggleButton = new Vector2(30, 12);
        public static Vector2 helpButtonSize = new Vector2(25, 25);
    }

    public static class StylingColours
    {
        public static Vector4 defaultBackground => ThemeHandler.ActiveTheme.defaultBackground;
        public static Vector4 titleBgActive => ThemeHandler.ActiveTheme.titleBgActive;
        public static Vector4 titleBg => ThemeHandler.ActiveTheme.titleBg;
        public static Vector4 tileBgCollapsed => ThemeHandler.ActiveTheme.tileBgCollapsed;

        public static Vector4 whiteText => ThemeHandler.ActiveTheme.whiteText; 
        public static Vector4 defaultText => ThemeHandler.ActiveTheme.defaultText;
        public static Vector4 errorText => ThemeHandler.ActiveTheme.errorText;
        public static Vector4 highlightText => ThemeHandler.ActiveTheme.highlightedText;
        public static Vector4 readableBlueText => ThemeHandler.ActiveTheme.readableBlueText;

        public static Vector4 idleColor => ThemeHandler.ActiveTheme.idleColor;

        public static Vector4 buttonHovered => ThemeHandler.ActiveTheme.buttonHovered;
        public static Vector4 buttonPressed => ThemeHandler.ActiveTheme.buttonPressed;
        public static Vector4 button => ThemeHandler.ActiveTheme.button;

        public static Vector4 textFieldHovered => ThemeHandler.ActiveTheme.textFieldHovered;
        public static Vector4 textFieldPressed => ThemeHandler.ActiveTheme.textFieldPressed;
        public static Vector4 textField => ThemeHandler.ActiveTheme.textField;

        public static Vector4 xButtonHovered => ThemeHandler.ActiveTheme.xButtonHovered;
        public static Vector4 xButtonPressed => ThemeHandler.ActiveTheme.xButtonPressed;
        public static Vector4 xButton => ThemeHandler.ActiveTheme.xButton;

        public static Vector4 listBox => ThemeHandler.ActiveTheme.listBox;
        public static Vector4 scrollBarBG => ThemeHandler.ActiveTheme.scrollBarBG;
    }

    int ButtonCount = 3;

    private void DrawModeToggle()
    {
        int pressed = -1;

        for(int i = 0; i < (int)PetMode.COUNT; i++)
        {
            int identifier = 60 + i;
            if (((int)petMode == i) ? ToggleButton(identifier) : ToggleButtonBad(identifier))
                pressed = i;

            SetTooltipHovered(tooltipStrings[(PetMode)i]);

            SameLineNoMargin();
        }

        float widthLeft = (int)PetMode.COUNT * Styling.ToggleButton.X;
        float widthRight = ButtonCount * (Styling.helpButtonSize.X + (5 * 2));

        float setWidth = ImGui.GetWindowSize()!.X - widthLeft - widthRight;

        ImGui.SameLine(0, setWidth);

        for(int i = 0; i < helpButtons.Count; i++)
        {
            if(Button(helpButtons[i].Item1, Styling.helpButtonSize)) PluginLink.WindowHandler.OpenWindow(helpButtons[i].Item2);
            SetTooltipHovered(helpButtons[i].Item3);
            if (i != helpButtons.Count - 1) SameLinePretendSpace();
        }

        if (pressed == -1) return;
        petMode = (PetMode)pressed;
    }

    protected void TextColoured(Vector4 colour, string text) => ImGui.TextColored(colour, text);

    protected bool ToggleButton(int count = 0)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonPressed);
        return ImGui.Button($"##{count}toggleButton", Styling.ToggleButton);
    }

    protected bool ToggleButtonBad(int count = 0)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Button, StylingColours.idleColor);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.idleColor);
        return ImGui.Button($"##{count}toggleButtonBad", Styling.ToggleButton);
    }

    protected bool Button(string text)
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonPressed);
        return ImGui.Button(text);
    }

    protected bool Button(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonPressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        return ImGui.Button(text, styling);
    }

    protected bool XButtonError(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.xButtonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.xButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.xButtonPressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.errorText);
        return ImGui.Button(text, styling);
    }

    protected bool XButton(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.xButtonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.xButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.xButtonPressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        return ImGui.Button(text, styling);
    }

    protected bool TransparentLabel(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0,0,0,0));
        PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
        PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0, 0, 0));
        PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 0));
        return ImGui.Button(text, styling);
    }

    protected bool Label(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered,    StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Button,           StylingColours.idleColor);
        PushStyleColor(ImGuiCol.ButtonActive,     StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Text, StylingColours.readableBlueText);
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

    protected bool Checkbox(string text, ref bool value)
    {
        PushStyleColor(ImGuiCol.Text, StylingColours.whiteText);
        PushStyleColor(ImGuiCol.CheckMark, StylingColours.whiteText);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.xButtonHovered);
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.xButton);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.xButtonPressed);
        return ImGui.Checkbox(text, ref value);
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

    private void PushStyleColor(ImGuiCol imGuiCol, Vector4 colour)
    {
        if (!PluginLink.Configuration.useCustomTheme) return;
        popCount++;
        ImGui.PushStyleColor(imGuiCol, colour);
    }

    private void PopAllStyleColours() 
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

    protected bool InputText(string label, ref string input, uint maxLength, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.textField);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.textFieldPressed);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.textFieldHovered);
        return ImGui.InputText(label, ref input, maxLength, flags);
    }

    protected void SameLine() => ImGui.SameLine();
    protected void SameLineNoMargin() => ImGui.SameLine(0, 0);
    protected void SameLinePretendSpace() => ImGui.SameLine(0, 3f);
    protected void NewLine() => ImGui.NewLine();
}

internal enum PetMode
{
    Normal,
    BattlePet,
    ShareMode,
    COUNT
}