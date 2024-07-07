using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class PetRenameNode : Node
{
    readonly Node RenameNode;
    readonly Node ImageNode;

    readonly Node HeaderNode;

    readonly Node TextNode;
    readonly StringInputNode InputField;

    readonly Node SaveButton;
    readonly Node ClearButton;

    readonly IPetSheetData ActivePet;

    const int Margin = 3;

    string? CurrentValue = null;

    public Action<string?>? OnSave;

    public PetRenameNode(string? customName, in IPetSheetData activePet)
    {
        CurrentValue = customName;

        ActivePet = activePet;
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
                        NodeValue = string.Format(Translator.GetLine(customName == null ? "PetRenameNode.IsNotRenamed" : "PetRenameNode.IsRenamed"), activePet.BaseSingular),
                    },
                    TextNode = new Node()
                    {
                        Stylesheet = PetRenameStyleSheet,
                        ClassList = ["RenameElementTopText", "RenameElementStyle", "RenameElementMargin"],
                        NodeValue = customName ?? "",
                    },
                    InputField = new StringInputNode("RenameNode", customName ?? "", PluginConstants.ffxivNameSize)
                    {
                        Stylesheet = PetRenameStyleSheet,
                        ClassList = ["RenameElementStyle", "RenameElementMargin"],
                    },
                    new Node()
                    {
                        ChildNodes = [
                        SaveButton = new Node()
                        {
                            Stylesheet = PetRenameStyleSheet,
                            ClassList = ["RenameElementStyle", "RenameElementMargin", "RenameElementButton"],
                            NodeValue = Translator.GetLine("PetRenameNode.SaveNickname"),
                        },
                            ClearButton = new Node()
                            {
                                Stylesheet = PetRenameStyleSheet,
                                ClassList = ["RenameElementStyle", "RenameElementMargin", "RenameElementButton"],
                                NodeValue = Translator.GetLine("PetRenameNode.ClearNickname"),
                            }
                        ]
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

        if (customName == null)
        {
            RenameNode.RemoveChild(TextNode);
        }

        InputField.OnValueChanged += (str) => CurrentValue = str;
        SaveButton.OnMouseUp += _ => OnSave?.Invoke(InputField.Value == string.Empty ? null : InputField.Value);
        ClearButton.OnMouseUp += _ => OnSave?.Invoke(null);

        BeforeReflow += _Reflow;
    }

    bool _Reflow(Node? node = null)
    {
        Style.Size = (ParentNode!.Bounds.ContentSize - ParentNode!.ComputedStyle.Padding.Size) / ScaleFactor;

        int height = Style.Size.Height;
        ImageNode.Style.Size = new Size(height, height) - ImageNode.ComputedStyle.Margin.Size / ScaleFactor;
        RenameNode.Style.Size = Style.Size - new Size(height, 0);
        RenameNode.Style.Size = Style.Size - RenameNode.ComputedStyle.Margin.Size / ScaleFactor - new Size(height, 0);

        HeaderNode.Style.Size = new Size(RenameNode.Style.Size.Width, 33) - RenameNode.ComputedStyle.Margin.Size / ScaleFactor;
        TextNode.Style.Size = new Size(RenameNode.Style.Size.Width, 33) - RenameNode.ComputedStyle.Margin.Size / ScaleFactor;
        InputField.Style.Size = new Size(RenameNode.Style.Size.Width, 38) - RenameNode.ComputedStyle.Margin.Size / ScaleFactor;

        SaveButton.Style.Size = new Size(RenameNode.Style.Size.Width / 2, 34) - RenameNode.ComputedStyle.Margin.Size / ScaleFactor;
        ClearButton.Style.Size = new Size(RenameNode.Style.Size.Width / 2, 34) - RenameNode.ComputedStyle.Margin.Size / ScaleFactor;
        return true;
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
                TextOffset = new System.Numerics.Vector2(0, 2.5f),
                Padding = new EdgeSize(Margin),
            }),
            new (".RenameElementButton", new Style()
            {
                BackgroundColor = new Color("Window.BackgroundLight"),
                TextShadowSize = 2,
                TextShadowColor = new Color("Window.TitlebarTextOutline"),
                FontSize = 15,
                TextAlign = Anchor.MiddleCenter,
                TextOffset = new System.Numerics.Vector2(0, 2),
                Padding = new EdgeSize(Margin),
            }),
            new(".RenameElementButton:hover", new Style()
            {
                BackgroundColor = new Color("Window.Background"),
            }),
        ]
    );
}
