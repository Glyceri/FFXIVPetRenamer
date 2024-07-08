using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class RenameTitleNode : Node
{
    readonly string Label;

    public readonly Node UnderlineNode;

    readonly Node TextNode;

    public Action? Hovered;
    public Action? HoveredExit;

    public RenameTitleNode(string label, string text)
    {
        Label = label;
        
        Style.Size = new Size(370, 25);
        ChildNodes = [
            new Node()
            {
                Stylesheet = stylesheet,
                ClassList = ["LabelNode"],
                NodeValue = Label,
                ChildNodes = [
                     TextNode = new Node()
                     {
                         Stylesheet = stylesheet,
                         ClassList = ["TextNode"],
                         NodeValue = text,
                     },
                    UnderlineNode = new Node()
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["UnderlineNode"],
                    },
                ]
            }
        ];

        TextNode.OnMouseEnter += _ => Hovered?.Invoke();
        TextNode.OnMouseLeave += _ => HoveredExit?.Invoke();
    }


    public void SetText(string text)
    {
        TextNode.NodeValue = text;
    }

    Stylesheet stylesheet = new Stylesheet([
        new(".LabelNode", new Style()
        {
            Margin = new EdgeSize(5, 0, 0, 0),
            Size = new Size(70, 25),
            TextAlign = Anchor.TopLeft,
            TextOffset = new System.Numerics.Vector2(0, 12),
            Flow = Flow.Vertical,
            FontSize = 10,
            TextOverflow = false,
            Color = new Color("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
        }),
        new(".TextNode", new Style()
        {
            Margin = new EdgeSize(5, 0, 0, 0),
            Size = new Size(300, 20),
            BorderColor = new BorderColor(new Color(255, 255, 255)),
            TextAlign = Anchor.MiddleRight,
            FontSize = 14,
            TextOverflow = false,
            Color = new Color("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
        }),
        new(".TextNode:hover", new Style()
        {
            //FontSize = 18,
        }),
        new(".UnderlineNode", new Style()
        {
            Size = new Size(300, 2),
            BackgroundGradient = GradientColor.Horizontal(new Color(255, 255, 255, 55), new Color(255, 255, 255, 255)),
            RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
            BorderRadius = 3,
        }),
    ]);

}
