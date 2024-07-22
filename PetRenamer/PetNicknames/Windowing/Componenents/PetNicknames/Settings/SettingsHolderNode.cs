using Dalamud.Interface;
using ImGuiNET;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;

internal class SettingsHolderNode : Node
{
    readonly Configuration Configuration;

    readonly QuickSquareButton Toggle;
    readonly Node HeaderTextNode;

    public readonly Node ContentNode;

    bool state = false;

    public SettingsHolderNode(in Configuration configuration, string label)
    {
        Configuration = configuration;

        Style = new Style()
        {
            Size = new Size(384, 0),
            Flow = Flow.Vertical,
            BorderColor = new(new("Outline")),
            BorderWidth = new EdgeSize(1),
        };

        ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Horizontal,
                },
                ChildNodes = 
                [
                    Toggle = new QuickSquareButton()
                    {
                        NodeValue = FontAwesomeIcon.ArrowRight.ToIconString(),
                        Style = new Style()
                        {
                            Margin = new EdgeSize(2),
                        }
                    },
                    HeaderTextNode = new Node()
                    {
                        Style = new Style()
                        {
                            Size = new Size(360, 15),
                            Margin = new EdgeSize(2),
                            FontSize = 10,
                            TextOverflow = false,
                            Color = new Color("Window.TextLight"),
                            OutlineColor = new("Window.TextOutline"),
                            OutlineSize = 1,
                            TextAlign = Anchor.MiddleLeft,
                            TextOffset = new System.Numerics.Vector2(0, 1),
                        },
                        NodeValue = label,
                    },
                ]
            },
            
            ContentNode = new Node()
            {
                Style = new Style()
                {
                    Size = new Size(380, 0),
                    IsVisible = false,
                    Margin = new EdgeSize(3, 3, 3, 21),
                    Flow = Flow.Vertical,
                    Gap = 3,
                },
            }
        ];

        Toggle.OnClick += () =>
        {
            state = !state;
            Toggle.NodeValue = state ? FontAwesomeIcon.ArrowDown.ToIconString() : FontAwesomeIcon.ArrowRight.ToIconString();
            ContentNode.Style.IsVisible = state;
        };
    }
}
