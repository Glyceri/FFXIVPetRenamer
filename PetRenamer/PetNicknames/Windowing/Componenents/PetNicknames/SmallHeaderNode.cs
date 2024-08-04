using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class SmallHeaderNode : Node
{
    public readonly Node UnderlineNodeLeft;
    public readonly Node UnderlineNodeRight;

    public SmallHeaderNode(string text)
    {
        NodeValue = text;
        Stylesheet = stylesheet;
        ClassList = ["HeaderText"];
        Style.Flow = Flow.Horizontal;
        Style.Gap = 0;

        ChildNodes = [
            UnderlineNodeLeft = new Node()
            {
                Stylesheet = stylesheet,
                ClassList = ["UnderlineNodeLeft"]
            },
            UnderlineNodeRight = new Node()
            {
                Stylesheet = stylesheet,
                ClassList = ["UnderlineNodeRight"]
            },
        ];

        BeforeReflow += _ =>
        {
            UnderlineNodeLeft.Style.Size = new Size((int)(ComputedStyle.Size.Width * 0.5f / Node.ScaleFactor), 2);
            UnderlineNodeRight.Style.Size = new Size((int)(ComputedStyle.Size.Width * 0.5f / Node.ScaleFactor), 2);
            return false;
        };
    }


    readonly Stylesheet stylesheet = new Stylesheet([
        new (".HeaderText", new Style()
        {
            FontSize = 12,
            TextOverflow = true,
            Color = new Color("Window.TextLight"),
            TextAlign = Anchor.MiddleCenter,
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
            BorderInset = new EdgeSize(7, 15, 3, 15),
        }),
        new(".UnderlineNodeLeft", new Style()
        {
            BackgroundGradient = GradientColor.Horizontal(new Color("Outline:Fade"), new Color("Outline")),
            RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
            BorderRadius = 3,
            Anchor = Anchor.BottomCenter,
            Margin = new(0),
            Padding = new(0),
        }),
        new(".UnderlineNodeRight", new Style()
        {
            BackgroundGradient = GradientColor.Horizontal(new Color("Outline"), new Color("Outline:Fade")),
            RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
            BorderRadius = 3,
            Anchor = Anchor.BottomCenter,
            Margin = new(0),
            Padding = new(0),
        }),
    ]);
}
