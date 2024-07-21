using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;

internal class ToggleConfig : Node
{
    readonly Configuration Configuration;

    readonly Action<bool> Callback;

    public readonly Node UnderlineNode;
    public readonly Node LabelNode;

    readonly QuickSquareButton buttonClick;

    bool CurrentValue;

    public ToggleConfig(in Configuration configuration, string label, bool startValue, Action<bool> callback)
    {
        Configuration = configuration;
        Callback = callback;

        CurrentValue = startValue;

        Style = new Style()
        {
            Flow = Flow.Horizontal,
        };

        ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Size = new Size(15, 18),
                    Gap = 1,
                },
                ChildNodes = [
                    buttonClick = new QuickSquareButton()
                    {
                        NodeValue = startValue ? FontAwesomeIcon.Check.ToIconString() : string.Empty,
                        Style = new Style()
                        {
                            Flow = Flow.Vertical,
                        },
                    },
                    UnderlineNode = new Node()
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["UnderlineNode"],
                    },
                ]
            },
            LabelNode = new Node()
            {
                Stylesheet = stylesheet,
                ClassList = ["LabelNode"],
                NodeValue = label,

            },
        ];

        buttonClick.OnClick += () =>
        {
            CurrentValue = !CurrentValue;
            buttonClick.NodeValue = CurrentValue ? FontAwesomeIcon.Check.ToIconString() : string.Empty;
            callback?.Invoke(CurrentValue);
            Configuration.Save();
        };
    }

    readonly Stylesheet stylesheet = new Stylesheet([
        new(".LabelNode", new Style()
        {
            Size = new Size(270, 15),
            TextAlign = Anchor.TopLeft,
            TextOffset = new System.Numerics.Vector2(0, 3),
            FontSize = 8,
            TextOverflow = false,
            Color = new Color("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
        }
        ),
        new(".UnderlineNode", new Style()
        {
            Size = new Size(300, 2),
            BackgroundGradient = GradientColor.Horizontal(new Color("Outline"), new Color("Outline:Fade")),
            RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
            BorderRadius = 3,
        }),
    ]
    );
}
