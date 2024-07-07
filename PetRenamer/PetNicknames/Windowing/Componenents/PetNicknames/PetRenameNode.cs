using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class PetRenameNode : Node
{
    readonly Node RenameNode;
    readonly Node ImageNode;

    readonly Node HeaderNode;

    readonly Node TextNode;
    readonly StringInputNode InputField;

    readonly IPettableUser User;
    readonly IPetSheetData ActivePet;

    const int Margin = 3;

    string? CurrentValue = null;

    public PetRenameNode(in IPettableUser user, in IPetSheetData activePet)
    {
        string? nullableName = user.DataBaseEntry.GetName(activePet.Model);
        string customName = nullableName ?? "";
        CurrentValue = nullableName;

        ActivePet = activePet;
        User = user;
        Stylesheet = PetRenameStyleSheet;
        ClassList = ["RenameElementStyle"];
        ChildNodes = [
            RenameNode = new Node()
            {
                Stylesheet = PetRenameStyleSheet,
                ClassList = ["RenameElementStyle", "RenameElementMargin", "RenamePortion"],
                ChildNodes = [
                    HeaderNode = new Node()
                    {
                        Stylesheet = PetRenameStyleSheet,
                        ClassList = ["RenameElementTopText", "RenameElementStyle", "RenameElementMargin"],
                        NodeValue = "TEMPORARY",
                    },
                    TextNode = new Node()
                    {
                        Stylesheet = PetRenameStyleSheet,
                        ClassList = ["RenameElementTopText", "RenameElementStyle", "RenameElementMargin"],
                        NodeValue = TranslationString(customName, in ActivePet),
                    },
                    InputField = new StringInputNode("RenameNode", customName, PluginConstants.ffxivNameSize)
                    { 
                        Stylesheet = PetRenameStyleSheet,
                        ClassList = ["RenameElementStyle", "RenameElementMargin"],
                    }
                ]
            },
            ImageNode = new Node()
            {
                Stylesheet = PetRenameStyleSheet,
                ClassList = ["RenameElementStyle", "RenameElementMargin"],
                ChildNodes =
                [
                    new IconNode(activePet.Icon) { }
                ],
            }
        ];

        InputField.OnValueChanged += (str) => CurrentValue = str;

        BeforeReflow += _ =>
        {
            Style.Size = (ParentNode!.Bounds.ContentSize - ParentNode!.ComputedStyle.Padding.Size) / ScaleFactor;

            int height = Style.Size.Height;
            ImageNode.Style.Size = new Size(height, height) - ImageNode.ComputedStyle.Margin.Size / ScaleFactor;
            RenameNode.Style.Size = Style.Size - new Size(height, 0);
            RenameNode.Style.Size = Style.Size - RenameNode.ComputedStyle.Margin.Size / ScaleFactor - new Size(height, 0);

            HeaderNode.Style.Size = new Size(RenameNode.Style.Size.Width, 28) - RenameNode.ComputedStyle.Margin.Size / ScaleFactor;
            TextNode.Style.Size = new Size(RenameNode.Style.Size.Width, 47) - RenameNode.ComputedStyle.Margin.Size / ScaleFactor;
            InputField.Style.Size = new Size(RenameNode.Style.Size.Width, 38) - RenameNode.ComputedStyle.Margin.Size / ScaleFactor;

            return true;
        };
    }

    string TranslationString(string customName, in IPetSheetData activePet)
    {
        if (customName == string.Empty) return string.Format(Translator.GetLine("PetRenameNode.IsNotRenamed"), activePet.BaseSingular);
        return string.Format(Translator.GetLine("PetRenameNode.IsRenamed"), activePet.BaseSingular, customName);
    }

    static Stylesheet PetRenameStyleSheet { get; } = new Stylesheet(
        [
            new(".RenameElementStyle", new Style()
            {
                BackgroundColor = new Color("Window.Background"),
                BorderRadius = 6,
                IsAntialiased = false,
                RoundedCorners = RoundedCorners.All,
                ShadowSize = new(64),
                ShadowInset = 8,
            }),
            new(".RenamePortion", new Style()
            {
                Flow = Flow.Vertical,
            }),
            new(".RenameElementMargin", new Style()
            {
                Margin = new EdgeSize(Margin),
            }),
            new(".RenameElementTopText", new Style()
            {
                TextShadowSize = 2,
                TextShadowColor = new Color("Window.TitlebarTextOutline"),
                FontSize = 15,
                WordWrap = true,
                Padding = new EdgeSize(Margin),
            }),
        ]
    );
}
