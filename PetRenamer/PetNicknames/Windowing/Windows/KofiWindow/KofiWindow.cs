using Dalamud.Interface;
using Dalamud.Utility;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetShareWindow;

internal class KofiWindow : PetWindow
{
    protected override string ID { get; } = "KofiWindow";
    protected override Vector2 MinSize { get; } = new Vector2(350, 100);
    protected override Vector2 MaxSize { get; } = new Vector2(350, 100);
    protected override Vector2 DefaultSize { get; } = new Vector2(350, 100);
    protected override bool HasModeToggle { get; } = false;
    protected override string Title { get; } = Translator.GetLine("Kofi.Title");
    protected override bool HasExtraButtons { get; } = false;

    readonly QuickSquareButton Button;

    readonly SmallHeaderNode headerNode1;
    readonly SmallHeaderNode headerNode2;

    public KofiWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "KofiWindow")
    {
        ContentNode.Style = new Style()
        {
            Flow = Flow.Vertical,
            Padding = new EdgeSize(5, 0),
        };

        ContentNode.ChildNodes = [
            headerNode1 = new SmallHeaderNode(Translator.GetLine("Kofi.Line1"))
            {
                Style = new Style()
                {
                    Size = new Size(342, 15),
                    TextAlign = Anchor.MiddleCenter,
                }
            },
            headerNode2 = new SmallHeaderNode(Translator.GetLine("Kofi.Line2"))
            {
                Style = new Style()
                {
                    Size = new Size(342, 15),
                    TextAlign = Anchor.MiddleCenter,
                }
            },
            new Node()
            {
                Style = new Style()
                {
                    Padding = new EdgeSize(2, 150),
                },
                ChildNodes = [
                    Button = new QuickSquareButton()
                    {
                        Tooltip = Translator.GetLine("Kofi.TakeMe"),
                        NodeValue = FontAwesomeIcon.ArrowRightLong.ToIconString(),

                        Style = new Style()
                        {
                            Size = new Size(30, 15),
                        }
                    },
                ]
            }
        ];

        headerNode1.ChildNodes.Clear();
        headerNode2.ChildNodes.Clear();

        Button.OnClick += () =>
        {
            Util.OpenLink("https://ko-fi.com/glyceri");
        };
    }

    public override void OnDraw()
    {

    }
}
