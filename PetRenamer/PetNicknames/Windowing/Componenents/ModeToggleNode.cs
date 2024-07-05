using PetRenamer.PetNicknames.Services;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents;

internal class ModeToggleNode : Node
{
    Node CompanionNode => QuerySelector("MinionMode")!;
    Node BattlePetNode => QuerySelector("BattlePetMode")!;

    public ModeToggleNode(DalamudServices dalamudServices) 
    {
        Id = "BaseModeToggleNode";
        Stylesheet = ModeToggleStylesheet;
        ClassList = ["ModeToggleBase"];

        ChildNodes = [
            new()
            {
                Id = "MinionMode",
                Stylesheet = ModeToggleStylesheet,
                ClassList = ["ModeToggleButtonActive"],
            },
            new()
            {
                Id = "BattlePetMode",
                Stylesheet = ModeToggleStylesheet,
                ClassList = ["ModeToggleButtonActive"]
            },
        ];

        CompanionNode.OnClick += _ => { };
        BattlePetNode.OnClick += _ => { };
    }


    static readonly Stylesheet ModeToggleStylesheet = new Stylesheet(
        [
            new(
                ".ModeToggleBase",
                new()
                {
                    Anchor = Anchor.TopLeft,
                    Flow = Flow.Horizontal,
                    Size = new Size(70, 28),
                    Padding = new(2),
                }
            ),
            new(
                ".ModeToggleButtonActive",
                new()
                {
                    Size = new Size(33, 25),
                    StrokeColor = new("Window.TitlebarCloseButton"),
                    StrokeWidth = 1,
                    StrokeInset = 1,
                    BorderRadius = 6,
                    IsAntialiased = false,
                    RoundedCorners = RoundedCorners.All,
                    ShadowSize = new(15),
                    ShadowInset = 1,
                    Margin = new EdgeSize(0, 2)
                }
                ),
            new(
                ".ModeToggleButtonActive:hover",
                new()
                {
                    StrokeColor = new("Window.TitlebarCloseButtonHover"),
                }
                ),
        ]);
}
