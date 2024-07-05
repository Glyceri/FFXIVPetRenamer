using Dalamud.Interface;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents;

internal class QuickBarNode : Node
{
    public QuickBarNode(string id) 
    { 
        Id = id;
        Stylesheet = QuickBarStylesheet;
        ClassList = ["quickbar--button"];
        NodeValue = FontAwesomeIcon.Times.ToIconString();
    }

    static Stylesheet QuickBarStylesheet = new Stylesheet(
        [
            new(
                ".quickbar--button",
                new()
                {
                    Anchor = Anchor.TopRight,
                    Size = new(22, 22),
                    BackgroundColor = new("Window.TitlebarCloseButton"),
                    StrokeColor = new("Window.TitlebarCloseButtonBorder"),
                    StrokeWidth = 1,
                    StrokeInset = 0,
                    BorderRadius = 3,
                    TextAlign = Anchor.MiddleCenter,
                    Font = 2,
                    FontSize = 12,
                    Color = new("Window.TitlebarCloseButtonX"),
                    OutlineColor = new("Window.TitlebarCloseButtonXOutline"),
                    TextOverflow = true,
                    Margin = new() { Top = 2, Right = 4 },
                    IsAntialiased = false,
                }
            ),
            new(
                ".quickbar--button:hover",
                new()
                {
                    BackgroundColor = new("Window.TitlebarCloseButtonHover"),
                    Color = new("Window.TitlebarCloseButtonXHover"),
                    StrokeWidth = 2,
                }
            ),
        ]);
}
