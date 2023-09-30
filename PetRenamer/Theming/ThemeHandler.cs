using PetRenamer.Theming.Themes;

namespace PetRenamer.Theming;

internal static class ThemeHandler
{
    internal static readonly Theme baseTheme = new BaseTheme();
    internal static readonly Theme greenTheme = new GreenTheme();
    internal static readonly Theme redTheme = new RedTheme();

    internal static readonly Theme BlackWhite = new BlackWhiteTheme();

    internal static readonly Theme baseThemeCustom = new BaseTheme();
    internal static readonly Theme greenThemeCustom = new GreenTheme();
    internal static readonly Theme redThemeCustom = new RedTheme();

    internal static Theme ActiveTheme { get; private set; } = baseTheme;

    internal static void SetTheme(Theme theme) => ActiveTheme = theme;
}
