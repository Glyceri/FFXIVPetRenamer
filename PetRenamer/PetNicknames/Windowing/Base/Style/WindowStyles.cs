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
                    StrokeColor = new("Window.Border"),
                    StrokeWidth = 1,
                    BorderRadius = 6,
                    IsAntialiased = false,
                    RoundedCorners = RoundedCorners.All,
                    ShadowSize = new(64),
                    ShadowInset = 8,
                    Padding = new(3),
                }
            ),
            new(
                ".window--titlebar",
                new()
                {
                    Flow = Flow.Horizontal,
                    Size = new(0, 32),
                    Color = new("Window.TitlebarText"),
                    BackgroundColor = new("Window.TitlebarBackground"),
                    BackgroundGradient = GradientColor.Vertical(
                        new("Window.TitlebarGradient1"),
                        new("Window.TitlebarGradient2")
                    ),
                    BorderColor = new(new("Window.TitlebarBorder")),
                    BorderWidth = new(1),
                    BorderRadius = 6,
                    IsAntialiased = false,
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    //Margin = new(1) { Right = -1, Bottom = -1 },
                }
            ),
            new(
                ".window--titlebar-text",
                new()
                {
                    FontSize = 16,
                    Color = new("Window.TitlebarText"),
                    OutlineColor = new("Window.TitlebarTextOutline"),
                    OutlineSize = 1,
                    TextAlign = Anchor.MiddleCenter,
                    TextOverflow = false,
                    WordWrap = false,
                    Size = new(0, 32),
                    Padding = new(0, 6)
                }
            ),
            new(
                ".window--close-button",
                new()
                {
                    Anchor = Anchor.MiddleRight,
                    Size = new(25, 25),
                    BackgroundColor = new("Window.BackgroundLight"),
                    StrokeColor = new("Window.TitlebarBorder"),
                    StrokeWidth = 1,
                    StrokeInset = 0,
                    BorderRadius = 3,
                    TextAlign = Anchor.MiddleCenter,
                    Font = 2,
                    FontSize = 14,
                    Color = new("Window.TextLight"),
                    OutlineColor = new("Window.TextOutline"),
                    TextOverflow = true,
                    Margin = new() { Top = 3, Right = 4 },
                    IsAntialiased = false,
                }
            ),
            new(
                ".window--close-button:hover",
                new()
                {
                    BackgroundColor = new("Window.Background"),
                    //Color = new("Window.TitlebarCloseButtonXHover"),
                    StrokeWidth = 2,
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
                    BorderRadius = 6,
                    RoundedCorners = RoundedCorners.BottomLeft | RoundedCorners.BottomRight,
                    ScrollbarTrackColor = new("Window.ScrollbarTrack"),
                    ScrollbarThumbColor = new("Window.ScrollbarThumb"),
                    ScrollbarThumbHoverColor = new("Window.ScrollbarThumbHover"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                }
            )
        ]
    );

    public static void RegisterDefaultColors()
    {
        Color.AssignByName("Titlebar.Minion", new Color(255, 194, 63).ToUInt());
        Color.AssignByName("Titlebar.Minion:Dark", new Color(110, 78, 10).ToUInt());
        Color.AssignByName("Titlebar.BattlePet", new Color(37, 112, 37).ToUInt());
        Color.AssignByName("Titlebar.BattlePet:Dark", new Color(26, 84, 26).ToUInt());
        Color.AssignByName("Titlebar.Base", new Color(255, 255, 255).ToUInt());
        Color.AssignByName("Window.Border:Active", new Color(224, 183, 18).ToUInt());

        Color.AssignByName("Window.Background", new Color(26, 20, 0, 200).ToUInt());
        Color.AssignByName("Window.BackgroundLight", new Color(161, 155, 138, 110).ToUInt());

        Color.AssignByName("ModeToggleInactive", new Color(15, 15, 15).ToUInt());

        Color.AssignByName("Window.TextOutline", new Color(189, 141, 6, 190).ToUInt());

        Color.AssignByName("PetNicknamesButton", new Color(91, 120, 83, 150).ToUInt());
        Color.AssignByName("PetNicknamesButton:Hover", new Color(47, 69, 41, 150).ToUInt());


        Color.AssignByName("Window.Border:Inactive", 0xFF404040);
        Color.AssignByName("ModeToggleButton.Border:Active", 0xFF484848);

        Color.AssignByName("ModeToggleButton.Border:Hover", new Color(50, 50, 50).ToUInt());

        Color.AssignByName("Widget.PopupBackground", 0xFF101010);
        Color.AssignByName("Widget.PopupBackground.Gradient1", 0xFF2F2E2F);
        Color.AssignByName("Widget.PopupBackground.Gradient2", 0xFF1A1A1A);

        Color.AssignByName("Window.TitlebarBorder", new Color(176, 169, 120).ToUInt()); //0xFF404040

        Color.AssignByName("Window.TitlebarBackground", 0xFF101010);
        Color.AssignByName("Window.Text", 0xFFD0D0D0);
        Color.AssignByName("Window.TextLight", 0xFFFFFFFF);
        Color.AssignByName("Window.TextMuted", 0xB0C0C0C0);
        
        Color.AssignByName("Window.TextDisabled", 0xA0A0A0A0);


        Color.AssignByName("Window.TitlebarGradient1", 0xFF2F2E2F);
        Color.AssignByName("Window.TitlebarGradient2", 0xFF1A1A1A);
        Color.AssignByName("Window.TitlebarText", 0xFFD0D0D0);
        Color.AssignByName("Window.TitlebarTextOutline", 0xC0000000);
        Color.AssignByName("Window.TitlebarCloseButton", 0xFF101010);
        Color.AssignByName("Window.TitlebarCloseButtonBorder", 0xFF404040);
        Color.AssignByName("Window.TitlebarCloseButtonHover", 0xFF304090);
        Color.AssignByName("Window.TitlebarCloseButtonX", 0xFFD0D0D0);
        Color.AssignByName("Window.TitlebarCloseButtonXHover", 0xFFFFFFFF);
        Color.AssignByName("Window.TitlebarCloseButtonXOutline", 0xFF000000);
        Color.AssignByName("Window.ScrollbarTrack", 0xFF212021);
        Color.AssignByName("Window.ScrollbarThumb", 0xFF484848);
        Color.AssignByName("Window.ScrollbarThumbHover", 0xFF808080);
        Color.AssignByName("Window.ScrollbarThumbActive", 0xFF909090);

        Color.AssignByName("Window.AccentColor", 0xFF4c8eb9);
        Color.AssignByName("Input.Background", 0xFF151515);
        Color.AssignByName("Input.Border", 0xFF404040);
        Color.AssignByName("Input.Text", 0xFFD0D0D0);
        Color.AssignByName("Input.TextMuted", 0xA0D0D0D0);
        Color.AssignByName("Input.TextOutline", 0xC0000000);
        Color.AssignByName("Input.BackgroundHover", 0xFF212021);
        Color.AssignByName("Input.BorderHover", 0xFF707070);
        Color.AssignByName("Input.TextHover", 0xFFFFFFFF);
        Color.AssignByName("Input.TextOutlineHover", 0xFF000000);
        Color.AssignByName("Input.BackgroundDisabled", 0xE0212021);
        Color.AssignByName("Input.BorderDisabled", 0xC0404040);
        Color.AssignByName("Input.TextDisabled", 0xA0A0A0A0);
        Color.AssignByName("Input.TextOutlineDisabled", 0xC0000000);
        Color.AssignByName("Toolbar.InactiveBackground1", 0xC02A2A2A);
        Color.AssignByName("Toolbar.InactiveBackground2", 0xC01F1F1F);
        Color.AssignByName("Toolbar.Background1", 0xFF2F2E2F);
        Color.AssignByName("Toolbar.Background2", 0xFF1A1A1A);
        Color.AssignByName("Toolbar.InactiveBorder", 0xA0484848);
        Color.AssignByName("Toolbar.Border", 0xFF484848);
        Color.AssignByName("Widget.Background", 0xFF101010);
        Color.AssignByName("Widget.BackgroundDisabled", 0xFF2C2C2C);
        Color.AssignByName("Widget.BackgroundHover", 0xFF2F2F2F);
        Color.AssignByName("Widget.Border", 0xFF484848);
        Color.AssignByName("Widget.BorderDisabled", 0xFF484848);
        Color.AssignByName("Widget.BorderHover", 0xFF8A8A8A);
        Color.AssignByName("Widget.Text", 0xFFD0D0D0);
        Color.AssignByName("Widget.TextDisabled", 0xA0D0D0D0);
        Color.AssignByName("Widget.TextHover", 0xFFFFFFFF);
        Color.AssignByName("Widget.TextMuted", 0xFF909090);
        Color.AssignByName("Widget.TextOutline", 0x80000000);

        Color.AssignByName("Widget.PopupBorder", 0xFF484848);
        Color.AssignByName("Widget.PopupMenuText", 0xFFD0D0D0);
        Color.AssignByName("Widget.PopupMenuTextMuted", 0xFFB0B0B0);
        Color.AssignByName("Widget.PopupMenuTextDisabled", 0xFF808080);
        Color.AssignByName("Widget.PopupMenuTextHover", 0xFFFFFFFF);
        Color.AssignByName("Widget.PopupMenuBackgroundHover", 0x802F5FFF);
        Color.AssignByName("Widget.PopupMenuTextOutline", 0xA0000000);
        Color.AssignByName("Widget.PopupMenuTextOutlineHover", 0xA0000000);
        Color.AssignByName("Widget.PopupMenuTextOutlineDisabled", 0x30000000);
    }
}
