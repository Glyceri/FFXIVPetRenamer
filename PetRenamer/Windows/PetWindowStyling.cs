using Dalamud.Interface.Windowing;
using ImGuiNET;
using PetRenamer.Core.AutoRegistry.Interfaces;
using PetRenamer.Theming;
using System.Numerics;

namespace PetRenamer.Windows;

/// <summary>
/// NEVER EXTEND FROM THIS
/// </summary>
public abstract class PetWindowStyling : Window, IDisposableRegistryElement
{
    public PetWindowStyling(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    public void Dispose() => OnDispose();
    protected virtual void OnDispose() { }

    public override void Draw() { }

    public static ImGuiStylePtr stylePtr => ImGui.GetStyle();

    public static float WindowPaddingX => stylePtr.WindowPadding.X;
    public static float WindowPaddingY => stylePtr.WindowPadding.Y;

    public static float FramePaddingX => stylePtr.FramePadding.X;
    public static float FramePaddingY => stylePtr.FramePadding.Y;

    public static float SpaceSize => 3.0f;

    public static float BarSize => 25;
    public static float BarSizePadded => BarSize + (FramePaddingY * 2);

    public static float ContentAvailableX => ImGui.GetContentRegionAvail().X;
    public static float FullWidth => ImGui.GetWindowWidth();

    public static float ItemSpacingX => stylePtr.ItemSpacing.X;
    public static float ItemSpacingY => stylePtr.ItemSpacing.Y;

    public static float FillingWidth => FullWidth - (WindowPaddingX * 2) - stylePtr.ScrollbarSize;
    public static float FillingWidthStepped(int steps = 1) => FullWidth - (FramePaddingX * 2 * steps);

    protected ImGuiCol[] LabelColours = new ImGuiCol[] { ImGuiCol.Button, ImGuiCol.ButtonActive, ImGuiCol.ButtonHovered };

    public Vector2 ToggleButtonStyle => new Vector2(BarSizePadded, BarSize * 0.5f);

    protected StylingClass Styling = new StylingClass();

    public class StylingClass
    {
        public readonly Vector2 ListButton = new Vector2(150, 25);
        public readonly Vector2 ListNameButton = new Vector2(480, 25);
        public readonly Vector2 ListIDField = new Vector2(75, 25);
        public readonly Vector2 SmallButton = new Vector2(25, 25);
        public readonly Vector2 ListSmallNameField = new Vector2(200, 25);
        public readonly Vector2 FillSize = new Vector2(755, 25);
        public readonly Vector2 FillSizeSmall = new Vector2(746, 25);
        public readonly Vector2 FillSizeFull = new Vector2(782, 25);
        public readonly Vector2 FillSizeFullSmall = new Vector2(774, 25);
        public readonly Vector2 PetWindowInsideBar = new Vector2(290, 25);
        public readonly Vector2 helpButtonSize = new Vector2(25, 25);
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
}
