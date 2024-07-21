using Dalamud.Game.Text;
using PetRenamer.PetNicknames.TranslatorSystem;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;

internal class UIScaleSettingsBar : Node
{
    public readonly Node LabelNode;
    public readonly Node UnderlineNode;

    readonly Configuration Configuration;

    public UIScaleSettingsBar(in Configuration configuration)
    {
        Configuration = configuration;
        ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Size = new Size(300, 31),
                    Flow = Flow.Vertical,
                },
                ChildNodes = [
                    LabelNode = new Node()
                    {
                        NodeValue = Translator.GetLine("Config.UISettings.UIScale.Header.Title"),
                        Style = new Style()
                        {
                            Size = new Size(300, 10),
                            FontSize = 8,
                            TextOverflow = false,
                            Color = new Color("Window.TextLight"),
                            OutlineColor = new("Window.TextOutline"),
                            OutlineSize = 1,
                            Padding = new EdgeSize(0, 0, 1, 0),
                        }
                    },
                    new Node()
                    {
                        Style = new Style()
                        {
                            Flow = Flow.Horizontal,
                        },
                        ChildNodes = 
                        [
                            new UIScaleButton(SeIconChar.BoxedLetterD.ToIconString(), () => SetSize(0)),
                            new UIScaleButton("80%",  () => SetSize(0.8f)),
                            new UIScaleButton("100%", () => SetSize(1.0f)),
                            new UIScaleButton("117%", () => SetSize(1.17f)),
                            new UIScaleButton("150%", () => SetSize(1.5f)),
                            new UIScaleButton("200%", () => SetSize(2.0f)),
                            new UIScaleButton("250%", () => SetSize(2.5f)),
                            new UIScaleButton("300%", () => SetSize(3.0f)),
                            new UIScaleButton("350%", () => SetSize(3.5f)),
                            new UIScaleButton("400%", () => SetSize(4.0f)),
                        ]
                    },
                    UnderlineNode = new Node()
                    {
                        Style = new Style()
                        {
                            Margin = new EdgeSize(1, 0, 0, 0),
                            Size = new Size(300, 2),
                            BackgroundGradient = GradientColor.Horizontal(new Color("Outline"), new Color("Outline:Fade")),
                        }
                    },
                ]
            },
        ];
    }

    void SetSize(float size)
    {
        Configuration.petNicknamesUIScale = size;
        Configuration.Save();
    }
}
