using PetRenamer.PetNicknames.Services;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class RenameTitleNode : Node
{
    protected readonly string Label;

    public readonly Node UnderlineNode;
    public readonly Node TextNode;
    public readonly Node LabelNode;

    public bool Interactable { get; set; } = false;

    protected readonly DalamudServices DalamudServices;

    public RenameTitleNode(in DalamudServices dalamudServices, string label, string text)
    {
        DalamudServices = dalamudServices;
        Label = label;

        Style.Size = new Size(300, 17);
        ChildNodes = [
            LabelNode = new Node()
            {
                Stylesheet = stylesheet,
                ClassList = ["LabelNode"],
                NodeValue = Label,
                ChildNodes = [
                    UnderlineNode = new Node()
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["UnderlineNode"],
                    },
                ]
            },
            TextNode = new Node()
            {
                Stylesheet = stylesheet,
                ClassList = ["TextNode"],
                NodeValue = text,
            },
        ];
    }

    public void SetText(string text)
    {
        TextNode.NodeValue = text;
    }

    readonly Stylesheet stylesheet = new Stylesheet([
        new(".LabelNode", new Style()
        {
            //Margin = new EdgeSize(5, 0, 0, 0),
            Size = new Size(100, 15),
            TextAlign = Anchor.TopLeft,
            TextOffset = new System.Numerics.Vector2(0, 3),
            FontSize = 8,
            TextOverflow = false,
            Color = new Color("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
        }),
        new(".TextNode", new Style()
        {
            Size = new Size(200, 15),
            BorderColor = new BorderColor(new Color("Outline")),
            TextAlign = Anchor.MiddleRight,
            FontSize = 12,
            TextOverflow = false,
            Color = new Color("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
        }),
        new(".UnderlineNode", new Style()
        {
            Size = new Size(300, 2),
            Anchor = Anchor.BottomLeft,
            BackgroundGradient = GradientColor.Horizontal(new Color("Outline:Fade"), new Color("Outline")),
            RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
            BorderRadius = 3,
        }),
    ]);

}
