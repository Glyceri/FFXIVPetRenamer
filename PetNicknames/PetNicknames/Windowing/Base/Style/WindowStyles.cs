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
        new("Outline",                       new Color(255, 255, 255).ToUInt()),
        new("Outline:Fade",                  new Color(255, 255, 255, 100).ToUInt()),
        new("Titlebar.Minion",              new Color(255, 194, 63, 150).ToUInt()),
        new("Titlebar.Minion:Dark",         new Color(110, 78, 10).ToUInt()),
        new("Titlebar.BattlePet",           new Color(37, 112, 37, 150).ToUInt()),
        new("Titlebar.BattlePet:Dark",      new Color(26, 84, 26).ToUInt()),
        new("Titlebar.Base",                new Color(255, 255, 255).ToUInt()),
        new("Window.Background",             new Color(0, 0, 0, 255).ToUInt()),
        new("Window.BackgroundLight",        new Color(0, 0, 0, 150).ToUInt()),
        new("BackgroundImageColour",         new Color(67, 67, 0, 230).ToUInt()),
        new("ListElementBackground",         new Color(0, 0, 0).ToUInt()),
        new("SearchBarBackground",           new Color(150, 150, 150, 150).ToUInt()),
        new("ModeToggleInactive",           new Color(100, 0, 0, 0.01f).ToUInt()),
        new("Window.TextOutline",            new Color(0, 0, 0, 255).ToUInt()),
        new("Window.TextOutlineButton",      new Color(86, 86, 86, 255).ToUInt()),
        new("PetNicknamesButton",           new Color(91, 120, 83, 150).ToUInt()),
        new("PetNicknamesButton:Hover",     new Color(47, 69, 41, 150).ToUInt()),
        new("Window.Text",                   new Color(0, 0, 0, 255).ToUInt()),
        new("Window.TextLight",              new Color(255, 255, 255, 255).ToUInt()),
        new("WindowBorder:Active",           new Color(117, 117, 0).ToUInt()),
        new("WindowBorder:Inactive",         new Color(0, 0, 0, 0).ToUInt()),
        new("Button.Background",             new Color(168, 168, 168, 200).ToUInt()),
        new("Button.Background:Hover",       new Color(158, 158, 158, 250).ToUInt()),
        new("Button.Background:Inactive",    new Color(193, 193, 193, 100).ToUInt()),
        new("FlareImageColour",             new Color(255, 255, 0).ToUInt()),
    });
}
