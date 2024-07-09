using Dalamud.Interface;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class PetListNode : Node
{
    readonly IconNode IconNode;
    readonly Node ClearButtonNode;

    readonly QuickButton EditButton;
    readonly QuickButton ClearButton;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode IDNode;
    readonly RenameTitleNode NicknameNode;

    public PetListNode(in IPetSheetData data, string? customName)
    {
        Style = new Style()
        {
            Flow = Flow.Horizontal,
            BackgroundColor = new Color(26, 20, 0, 130),
            Size = new Size(412, 70),
            RoundedCorners = RoundedCorners.All,
            BorderRadius = 8,
            StrokeRadius = 8,
            BorderColor = new(new("Window.TitlebarBorder")),
            BorderWidth = new EdgeSize(1),
            IsAntialiased = false,
        };

        ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Margin = new EdgeSize(6, 0, 0, 8),
                },
                ChildNodes = [
                    SpeciesNode = new RenameTitleNode($"{Translator.GetLine("PetRenameNode.Species")}:", data.BaseSingular),
                    IDNode = new RenameTitleNode($"ID:", data.Model.ToString()),
                    NicknameNode = new RenameTitleNode($"{Translator.GetLine("PetRenameNode.Nickname")}:", customName ?? "..."),
                    new Node()
                    {
                        Style = new Style()
                        {
                            Flow = Flow.Horizontal,
                            Margin = new (0, 0, 0, 220),
                        },
                        ChildNodes = [
                            EditButton = new QuickButton($"{Translator.GetLine("PetRenameNode.Edit")}")
                            {
                                Style = new Style()
                                {
                                    FontSize = 7,
                                    Size = new Size(40, 12),
                                }
                            },
                            ClearButton = new QuickButton($"{Translator.GetLine("PetRenameNode.Clear")}")
                            {
                                Style = new Style()
                                {
                                    FontSize = 7,
                                    Size = new Size(40, 12),
                                }
                            },
                        ]
                    }
                ]
            },
            IconNode = new IconNode()
            {
                Style = new Style()
                {
                    Size = new Size(50, 50),
                    Anchor = Anchor.MiddleRight,
                    Margin = new EdgeSize(0, 33, 0, 0),
                }
            },
            ClearButtonNode = new Node()
            {
                NodeValue = FontAwesomeIcon.Times.ToIconString(),
                Stylesheet = stylesheet,
                ClassList = ["ClearButton"],
            }
        ];

        ClearButtonNode.OnClick += _ => { };

        IconNode.IconID = data.Icon;
    }

    Stylesheet stylesheet = new Stylesheet([
        new (".ClearButton", new Style()
        {
            Anchor = Anchor.TopRight,
            Size = new(15, 15),
            BackgroundColor = new("Window.BackgroundLight"),
            StrokeColor = new("Window.TitlebarBorder"),
            StrokeWidth = 1,
            StrokeInset = 0,
            BorderRadius = 3,
            TextAlign = Anchor.MiddleCenter,
            Font = 2,
            FontSize = 10,
            Color = new("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            TextOverflow = true,
            Margin = new() { Top = 6, Right = 6 },
            IsAntialiased = false,
        }),
        new(".ClearButton:hover", new Style()
        {
            BackgroundColor = new("Window.Background"),
            StrokeWidth = 2,
        })
    ]);
}
