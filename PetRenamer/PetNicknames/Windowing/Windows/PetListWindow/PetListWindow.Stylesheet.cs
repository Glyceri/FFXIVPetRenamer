using PetRenamer.PetNicknames.Windowing.Base.Style;
using Una.Drawing;
using static Una.Drawing.Stylesheet;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

internal partial class PetListWindow
{

    readonly Stylesheet stylesheet = new Stylesheet
    (
        [
            new StyleDefinition(".ListButton", 
                new Style()
                {
                    Size = new Size(20, 20),
                    Anchor = Anchor.TopRight,
                    BackgroundColor = new("Window.BackgroundLight"),
                    StrokeColor = WindowStyles.WindowBorderActive,
                    StrokeWidth = 1,
                    TextAlign = Anchor.MiddleCenter,
                    Font = 2,
                    FontSize = 10,
                    Color = new("Window.TextLight"),
                    OutlineColor = new("Window.TextOutline"),
                    Margin = new() { Top = 3, Right = 4 },
                }
            ),
            new StyleDefinition(".ListButton:hover",
                new Style()
                {
                    BackgroundColor = new("Window.Background"),
                    StrokeWidth = 2,
                }
            ),
            new StyleDefinition(".ListButton:disabled",
                new Style()
                {
                    BackgroundColor = new("Window.Background"),
                    Color = new("Window.Text"),
                    OutlineColor = new(0, 0, 0),
                    StrokeWidth = 1,
                    StrokeColor = WindowStyles.WindowBorderInactive,
                }
            ),
        ]
    );
}
