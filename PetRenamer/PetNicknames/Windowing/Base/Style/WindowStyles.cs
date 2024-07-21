using PetRenamer.PetNicknames.ColourProfiling;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Base.Style;

internal static class WindowStyles
{
    public static readonly Stylesheet WindowStylesheet = new(
        [
            new(
                ".window",
                new()
                {
                    Anchor = Anchor.TopLeft,
                    Flow = Flow.Vertical,
                    BackgroundColor = new("Window.Background"),
                    StrokeColor = new Color("Window.Border:Active"),
                    StrokeWidth = 1,
                    Padding = new(3),
                }
            ),
            new(
                ".window--titlebar",
                new()
                {
                    BackgroundColor = new Color(0, 0, 0, 0),
                    BorderColor = new(new("Window.TitlebarBorder")),
                    BorderWidth = new(1),
                }
            ),
            new(
                ".window--titlebar-text",
                new()
                {
                    FontSize = 16,
                    Color = new Color("Window.TextLight"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize = 1,
                    Anchor = Anchor.TopCenter,
                    TextAlign = Anchor.MiddleCenter,
                }
            ),
            new(
                ".window--content",
                new()
                {
                    BorderColor = new(new("Window.TitlebarBorder")),
                    BorderWidth = new(0, 1, 1, 1),
                    IsAntialiased = false,
                    Anchor = Anchor.TopLeft,
                    Flow = Flow.Vertical,
                }
            )
        ]
    );

    public static readonly ColourProfile DefaultColourProfile = new ColourProfile("Default", "System", new()
    {
        new("UnderlineColour",              new Color(255, 255, 255, 255).ToUInt()),
        new("UnderlineColour:Fade",         new Color(255, 255, 255, 100).ToUInt()),
        new("Titlebar.Minion",              new Color(255, 194, 63, 150).ToUInt()),
        new("Titlebar.Minion:Dark",         new Color(110, 78, 10).ToUInt()),
        new("Titlebar.BattlePet",           new Color(37, 112, 37, 150).ToUInt()),
        new("Titlebar.BattlePet:Dark",      new Color(26, 84, 26).ToUInt()),
        new("Titlebar.Base",                new Color(255, 255, 255).ToUInt()),
        new("Window.Background",            new Color(26, 20, 0, 255).ToUInt()),
        new("Window.BackgroundLight",       new Color(161, 155, 138, 110).ToUInt()),
        new("SearchBarBackground",          new Color(150, 150, 150, 150).ToUInt()),
        new("ModeToggleInactive",           new Color(15, 15, 15).ToUInt()),
        new("Window.TextOutline",           new Color(189, 141, 6, 190).ToUInt()),
        new("Window.TextOutlineButton",     new Color(194, 82, 17, 190).ToUInt()),
        new("PetNicknamesButton",           new Color(91, 120, 83, 150).ToUInt()),
        new("PetNicknamesButton:Hover",     new Color(47, 69, 41, 150).ToUInt()),
        new("Window.Text",                  0xFFD0D0D0),
        new("Window.TextLight",             0xFFFFFFFF),
        new("Window.TitlebarBorder",        new Color(176, 169, 120).ToUInt()),
        new("WindowBorder:Active",          new Color(224, 183, 18).ToUInt()),
        new("WindowBorder:Inactive",        0xFF404040),
    });
}
