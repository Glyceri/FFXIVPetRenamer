using PetRenamer.PetNicknames.ColourProfiling;
using PetRenamer.PetNicknames.TranslatorSystem;
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
                    BorderColor = new(new("Outline")),
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
                    BorderColor = new(new("Outline")),
                    BorderWidth = new(0, 1, 1, 1),
                    IsAntialiased = false,
                    Anchor = Anchor.TopLeft,
                    Flow = Flow.Vertical,
                }
            )
        ]
    );

    public static readonly ColourProfile DefaultColourProfile = new ColourProfile(Translator.GetLine("Style.Title.Default"), "Glyceri", new()
    {
        new("Outline",                      new Color(176, 169, 120).ToUInt()),
        new("Outline:Fade",                 new Color(176, 169, 120, 100).ToUInt()),
        new("Titlebar.Minion",              new Color(255, 194, 63, 150).ToUInt()),
        new("Titlebar.Minion:Dark",         new Color(110, 78, 10).ToUInt()),
        new("Titlebar.BattlePet",           new Color(37, 112, 37, 150).ToUInt()),
        new("Titlebar.BattlePet:Dark",      new Color(26, 84, 26).ToUInt()),
        new("Titlebar.Base",                new Color(255, 255, 255).ToUInt()),
        new("Window.Background",            new Color(26, 20, 0, 255).ToUInt()),
        new("Window.BackgroundLight",       new Color(26, 20, 0, 150).ToUInt()),
        new("BackgroundImageColour",        new Color(40, 40, 0, 230).ToUInt()),
        new("ListElementBackground",        new Color(26, 20, 0, 130).ToUInt()),
        new("SearchBarBackground",          new Color(150, 150, 150, 150).ToUInt()),
        new("ModeToggleInactive",           new Color(100, 0, 0, 0.01f).ToUInt()),
        new("Window.TextOutline",           new Color(189, 141, 6, 190).ToUInt()),
        new("Window.TextOutlineButton",     new Color(194, 82, 17, 190).ToUInt()),
        new("PetNicknamesButton",           new Color(91, 120, 83, 150).ToUInt()),
        new("PetNicknamesButton:Hover",     new Color(47, 69, 41, 150).ToUInt()),
        new("Window.Text",                  0xFFD0D0D0),
        new("Window.TextLight",             0xFFFFFFFF),
        new("WindowBorder:Active",          new Color(224, 183, 18).ToUInt()),
        new("WindowBorder:Inactive",        0xFF404040),
        new("Button.Background",            new Color(76, 69, 20, 200).ToUInt()),
        new("Button.Background:Hover",      new Color(76, 69, 20, 250).ToUInt()),
        new("Button.Background:Inactive",   new Color(76, 69, 20, 100).ToUInt()),
        new("FlareImageColour",             new Color(255, 255, 0).ToUInt()),
    });
}
