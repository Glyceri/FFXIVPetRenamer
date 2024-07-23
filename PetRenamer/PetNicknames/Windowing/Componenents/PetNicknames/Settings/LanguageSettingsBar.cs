using Dalamud.Game.Text;
using Dalamud.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;

internal class LanguageSettingsBar : Node
{
    public readonly Node LabelNode;
    public readonly Node UnderlineNode;

    readonly Configuration Configuration;

    public LanguageSettingsBar(in Configuration configuration)
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
                        NodeValue = Translator.GetLine("Config.LanguageSettingsBar.Header.Title"),
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
                            new UIScaleButton(FontAwesomeIcon.Gamepad.ToIconString(), () => SetLangage(0)) { Tooltip = Translator.GetLine("Language.Game") },
                            new UIScaleButton(SeIconChar.BoxedLetterE.ToIconString(), () => SetLangage(1)) { Tooltip = Translator.GetLine("Language.English") },
                            new UIScaleButton(SeIconChar.BoxedLetterG.ToIconString(), () => SetLangage(2)) { Tooltip = Translator.GetLine("Language.German") },
                            new UIScaleButton(SeIconChar.BoxedLetterF.ToIconString(), () => SetLangage(3)) { Tooltip = Translator.GetLine("Language.French") },
                            new UIScaleButton(SeIconChar.BoxedLetterJ.ToIconString(), () => SetLangage(4)) { Tooltip = Translator.GetLine("Language.Japanese") },
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

    void SetLangage(int language)
    {
        Configuration.languageSettings = language;
        Configuration.Save();
        
    }
}
