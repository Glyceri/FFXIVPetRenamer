using PetRenamer.PetNicknames.Services;
using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class RenameTitleNode : Node
{
    readonly string Label;

    public readonly Node UnderlineNode;
    public readonly Node TextNode;
    public readonly Node LabelNode;

    public Action? Hovered;
    public Action? HoveredExit;

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

        RegisterInputs(text);
    }

    void HoverInvoke(Node _) => DalamudServices.Framework.Run(() => Hovered?.Invoke());
    void HoverExitInvoke(Node _) => DalamudServices.Framework.Run(() => HoveredExit?.Invoke());

    public void SetText(string text)
    {
        TextNode.NodeValue = text;
        RegisterInputs(text);
    }

    void RegisterInputs(string text)
    {
        TextNode.OnMouseEnter -= HoverInvoke;
        TextNode.OnMouseLeave -= HoverExitInvoke;

        if (text != string.Empty && Interactable)
        {
            TextNode.OnMouseEnter += HoverInvoke;
            TextNode.OnMouseLeave += HoverExitInvoke;
        }
    }

    readonly Stylesheet stylesheet = new Stylesheet([
        new(".LabelNode", new Style()
        {
            //Margin = new EdgeSize(5, 0, 0, 0),
            Size = new Size(130, 15),
            TextAlign = Anchor.TopLeft,
            TextOffset = new System.Numerics.Vector2(0, 3),
            FontSize = 10,
            TextOverflow = false,
            Color = new Color("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
        }),
        new(".TextNode", new Style()
        {
            Size = new Size(170, 15),
            BorderColor = new BorderColor(new Color(255, 255, 255)),
            TextAlign = Anchor.MiddleRight,
            FontSize = 14,
            TextOverflow = false,
            Color = new Color("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
        }),
        new(".UnderlineNode", new Style()
        {
            Size = new Size(300, 2),
            Anchor = Anchor.BottomLeft,
            BackgroundGradient = GradientColor.Horizontal(new Color(255, 255, 255, 55), new Color(255, 255, 255, 255)),
            RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
            BorderRadius = 3,
        }),
    ]);

}
