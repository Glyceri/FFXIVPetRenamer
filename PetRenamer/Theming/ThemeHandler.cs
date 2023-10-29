using PetRenamer.Theming.Themes;

namespace PetRenamer.Theming;

internal static class ThemeHandler
{
    internal static readonly Theme baseTheme = new BaseTheme();
    internal static readonly Theme greenTheme = new GreenTheme();
    internal static readonly Theme redTheme = new RedTheme();

    internal static readonly Theme BlackWhite = new BlackWhiteTheme();

    internal static readonly Theme baseThemeCustomTemporary = new BaseTheme();
    internal static readonly Theme greenThemeCustomTemporary = new GreenTheme();
    internal static readonly Theme redThemeCustomTemporary = new RedTheme();

    internal static Theme ActiveTheme { get; private set; } = baseTheme;

    internal static void SetTheme(Theme theme) => ActiveTheme = theme;

    internal static void FillTheme(Theme theme, Theme newTheme)
    {
        theme.defaultText = newTheme.defaultText;
        theme.alternativeText = newTheme.alternativeText;
        theme.titleBgActive = newTheme.titleBgActive;
        theme.buttonAlternativeHovered = newTheme.buttonAlternativeHovered;
        theme.buttonHovered = newTheme.buttonHovered;
        theme.basicLabelColour = newTheme.basicLabelColour;
        theme.button = newTheme.button;
        theme.buttonHovered = theme.buttonHovered;
        theme.buttonPressed = newTheme.buttonPressed;
        theme.buttonAlternativePressed = newTheme.buttonAlternativePressed;
        theme.buttonAlternative = newTheme.buttonAlternative;
        theme.titleBgActive = theme.titleBgActive;
        theme.tileBgCollapsed = theme.tileBgCollapsed;
        theme.panelColour = newTheme.panelColour;
        theme.ipcLabelColour = newTheme.ipcLabelColour;
        theme.panelSubColour = newTheme.panelSubColour;
    }
}
