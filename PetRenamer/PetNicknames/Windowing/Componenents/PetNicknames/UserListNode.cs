using Dalamud.Interface;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class UserListNode : Node
{
    readonly ProfilePictureNode IconNode;
    readonly Node ClearButtonNode;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode IDNode;
    readonly RenameTitleNode NicknameNode;

    public UserListNode(in IImageDatabase imageDatabase, in IPettableDatabaseEntry entry)
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
                    Margin = new EdgeSize(11, 0, 0, 8),
                },
                ChildNodes = [
                    SpeciesNode = new RenameTitleNode("Name:", entry.Name),
                    IDNode = new RenameTitleNode("Homeworld:", entry.HomeworldName),
                    NicknameNode = new RenameTitleNode("ContentID:", entry.ContentID.ToString()),
                    new Node()
                    {
                        Style = new Style()
                        {
                            Flow = Flow.Horizontal,
                            Margin = new(0, 0, 0, 220),
                        },
                        ChildNodes = [
                           
                        ]
                    }
                ]
            },
            IconNode = new ProfilePictureNode(in imageDatabase)
            {
                Style = new Style()
                {
                    Size = new Size(50, 50),
                    Margin = new EdgeSize(10, 0, 0, 15),
                }
            },
            ClearButtonNode = new Node()
            {
                NodeValue = FontAwesomeIcon.Times.ToIconString(),
                Stylesheet = stylesheet,
                ClassList = ["ClearButton"],
            }
        ];

        IconNode.SetUser(entry);
        IconNode.RedownloadNode.Style.FontSize = 8;
        IconNode.RedownloadNode.Style.Size = new Size(24, 24);

        ClearButtonNode.OnClick += _ => { };
    }

    Stylesheet stylesheet = new Stylesheet([
        new(".ClearButton", new Style()
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
