using Dalamud.Interface;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using Una.Drawing;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.WindowNodes;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

internal partial class PetListWindow
{
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPettableDatabase LegacyDatabase;
    readonly IPetServices PetServices;
    readonly IImageDatabase ImageDatabase;

    readonly UserNode UserNode;

    readonly Node HeaderNode;
    readonly Node ScrollListBaseNode;
    readonly Node ScrollistContentNode;
    readonly Node BottomPortion;

    readonly Node NextListNode;
    readonly Node PreviousListNode;

    readonly QuickButton UserListButton;
    readonly QuickButton SharingButton;
    readonly QuickButton ActiveEveryoneButton;

    readonly SmallHeaderNode SmallHeaderNode;

    public PetListWindow(in DalamudServices dalamudServices, in IPetServices petServices, in IPettableUserList userList, in IPettableDatabase database, IPettableDatabase legacyDatabase, in IImageDatabase imageDatabase) : base(dalamudServices, "Pet List")
    {
        UserList = userList;
        Database = database;
        LegacyDatabase = legacyDatabase;
        PetServices = petServices;
        ImageDatabase = imageDatabase;
        ContentNode.Style.Flow = Flow.Vertical;
        ContentNode.Stylesheet = stylesheet;

        ContentNode.ChildNodes = [
            HeaderNode = new Node()
            {
                Style = new Style()
                {
                    Size = new Size(540, 100),
                },
                ChildNodes = [
                    new Node()
                    {
                        Style = new Style()
                        {
                            Size = new Size(410, 100),
                            BorderColor = new(new("Window.TitlebarBorder")),
                            BorderWidth = new EdgeSize(0, 1, 1, 1),
                            RoundedCorners = RoundedCorners.BottomLeft,
                            BorderRadius = 6,
                            StrokeRadius = 6,
                            IsAntialiased = false,
                        },
                        ChildNodes =
                        [
                            UserNode = new UserNode(in DalamudServices, in ImageDatabase),
                        ]
                    },
                    new Node()
                    {
                        Style = new Style()
                        {
                            Size = new Size(131, 100),
                            BorderColor = new(new("Window.TitlebarBorder")),
                            BorderWidth = new EdgeSize(0, 1, 1, 1),
                            RoundedCorners = RoundedCorners.BottomRight,
                            BorderRadius = 6,
                            StrokeRadius = 6,
                            IsAntialiased = false,
                            Flow = Flow.Vertical,
                        },
                        ChildNodes = [
                    new SmallHeaderNode("Navigation")
                    {
                        Style = new Style()
                        {
                            Margin = new EdgeSize(5),
                            Size = new Size(60, 20),
                            Anchor = Anchor.TopCenter,
                        }
                    },
                            new Node()
                            {
                                Style = new Style()
                                {
                                    Margin = new EdgeSize(5),
                                    Size = new Size(120, 80),
                                    Anchor = Anchor.BottomCenter,
                                    Flow = Flow.Vertical,
                                },
                                ChildNodes =
                                [
                                    UserListButton = new QuickButton(in DalamudServices, "User List")
                                    {
                                        Style = new Style()
                                        {
                                            Size = new Size(60, 15),
                                            Margin = new EdgeSize(18, 1, 1, 1),
                                            Anchor = Anchor.TopCenter,
                                        },
                                    },
                                    SharingButton = new QuickButton(in DalamudServices, "Sharing")
                                    {
                                        Style = new Style()
                                        {
                                            Size = new Size(60, 15),
                                            Margin = new EdgeSize(1),
                                            Anchor = Anchor.TopCenter,
                                        },
                                    },
                                    ActiveEveryoneButton = new QuickButton(in DalamudServices, "Active Everyone")
                                    {
                                        Style = new Style()
                                        {
                                            Size = new Size(60, 15),
                                            Margin = new EdgeSize(18, 1, 1, 1),
                                            Anchor = Anchor.TopCenter,
                                        },
                                    },
                                ]
                            }
                        ],
                    }
                ]
            },
            BottomPortion = new Node()
            {
                ChildNodes = [
                    SmallHeaderNode = new SmallHeaderNode(Translator.GetLine("PetListWindow.ListHeaderPersonalMinion"))
                    {
                        Style = new Style()
                        {
                            Anchor = Anchor.TopCenter,
                            Margin = new EdgeSize(5, 0, 0, 0),
                            Size = new Size(300, 20),
                            FontSize = 16,
                        }
                    },
                    new Node()
                    {
                        Style = new Style()
                        {
                            Flow = Flow.Vertical,
                            Anchor = Anchor.TopRight,
                            Gap = 5,
                            Margin = new EdgeSize(32, 37, 0, 0),
                        },
                        ChildNodes =
                        [
                            NextListNode = new Node()
                            {
                                ClassList = ["ListButton"],
                                NodeValue = FontAwesomeIcon.ArrowRight.ToIconString(),
                            },
                            PreviousListNode = new Node()
                            {
                                ClassList = ["ListButton"],
                                NodeValue = FontAwesomeIcon.ArrowLeft.ToIconString(),
                            },
                        ]
                    },
                    ScrollListBaseNode = new Node()
                    {
                        Overflow = false,
                        Style = new Style()
                        {
                            Anchor = Anchor.MiddleCenter,
                            ScrollbarTrackColor = new Color(0, 0, 0, 0),
                            ScrollbarThumbColor = new Color(224, 183, 18, 50),
                            ScrollbarThumbHoverColor = new Color(224, 183, 18, 200),
                            ScrollbarThumbActiveColor = new Color(237, 197, 33, 255),
                            //ScrollbarRounding = 15,
                            //ScrollbarWidth = 11,
                        },
                        ChildNodes =
                        [
                            ScrollistContentNode = new Node()
                            {
                                Style = new Style()
                                {
                                    Flow = Flow.Vertical,
                                    Padding = new EdgeSize(3),
                                    Gap = 10,
                                }
                            }
                        ]
                    }
                ]
            },
            new Node()
            {
                Style = new Style()
                {
                    BackgroundGradient = GradientColor.Vertical(WindowStyles.WindowBorderActive, new Color(224, 183, 18, 0)),
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    BorderRadius = 6,
                    Margin = new(129, 0, 0, 0),
                    Size = new Size(422, 4),
                    Anchor = Anchor.TopCenter,
                }
            },
            new Node()
            {
                Style = new Style()
                {
                    BackgroundGradient = GradientColor.Vertical(new Color(224, 183, 18, 0), WindowStyles.WindowBorderActive),
                    RoundedCorners = RoundedCorners.BottomRight | RoundedCorners.BottomLeft,
                    BorderRadius = 6,
                    Margin = new(0, 0, 29, 0),
                    Size = new Size(422, 4),
                    Anchor = Anchor.BottomCenter,
                }
            },
        ];

        UserListButton.Clicked += ToggleUserMode;

        NextListNode.OnMouseUp += _ => DalamudServices.Framework.Run(() => HandleIncrement(1));
        PreviousListNode.OnMouseUp += _ => DalamudServices.Framework.Run(() => HandleIncrement(-1));
    }

}
