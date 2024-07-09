using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

internal class PetListWindow : PetWindow
{
    UserNode UserNode;

    protected override string Title { get; } = "Pet List";
    protected override string ID { get; } = "Pet List Window";

    protected override Vector2 MinSize { get; } = new Vector2(550, 240);
    protected override Vector2 MaxSize { get; } = new Vector2(550, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(550, 240);
    protected override bool HasModeToggle { get; } = true;

    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPetServices PetServices;
    readonly IImageDatabase ImageDatabase;

    IPettableUser? lastUser = null;

    readonly Node HeaderNode;
    readonly Node ScrollListBaseNode;
    readonly Node ScrolllistContentNode;
    readonly Node BottomPortion;

    public PetListWindow(in DalamudServices dalamudServices, in IPetServices petServices, in IPettableUserList userList, in IPettableDatabase database, in IImageDatabase imageDatabase) : base(dalamudServices, "Pet List")
    {
        UserList = userList;
        Database = database;
        PetServices = petServices;
        ImageDatabase = imageDatabase;
        Node.Style.Flow = Flow.Vertical;

        Node.ChildNodes = [
            HeaderNode = new Node()
            {
                Style = new Style()
                {
                    Size = new Size(550, 100),
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
                ChildNodes = [
                    UserNode = new UserNode(in ImageDatabase),
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
                                ChildNodes = [
                            new QuickButton("User List")
                            {
                                Style = new Style()
                                {
                                    Size = new Size(60, 15),
                                    Margin = new EdgeSize(18, 1, 1, 1),
                                    Anchor = Anchor.TopCenter,
                                },
                            },
                                    new QuickButton("Sharing")
                                    {
                                        Style = new Style()
                                        {
                                            Size = new Size(60, 15),
                                            Margin = new EdgeSize(1),
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
                            ScrollbarRounding = 15,
                            ScrollbarWidth = 11,
                        },
                        ChildNodes = [
                    ScrolllistContentNode = new Node()
                    {
                        Style = new Style()
                        {
                            Flow = Flow.Vertical,
                            Padding = new EdgeSize(3),
                            Gap = 10,
                        },
                        ChildNodes = [
                            // This is the content node!
                            new Node()
                            {
                                Style = new Style()
                                {
                                    Flow = Flow.Vertical,
                                    Padding = new EdgeSize(10),
                                    BackgroundColor = new Color(0, 0, 0),
                                    Size = new Size(412, 70),
                                },
                            },
                            new Node()
                            {
                                Style = new Style()
                                {
                                    Flow = Flow.Vertical,
                                    Padding = new EdgeSize(10),
                                    BackgroundColor = new Color(0, 0, 0),
                                    Size = new Size(412, 70),
                                },
                            },
                            new Node()
                            {
                                Style = new Style()
                                {
                                    Flow = Flow.Vertical,
                                    Padding = new EdgeSize(10),
                                    BackgroundColor = new Color(0, 0, 0),
                                    Size = new Size(412, 70),
                                },
                            },
                            new Node()
                            {
                                Style = new Style()
                                {
                                    Flow = Flow.Vertical,
                                    Padding = new EdgeSize(10),
                                    BackgroundColor = new Color(0, 0, 0),
                                    Size = new Size(412, 70),
                                },
                            },
                            new Node()
                            {
                                Style = new Style()
                                {
                                    Flow = Flow.Vertical,
                                    Padding = new EdgeSize(10),
                                    BackgroundColor = new Color(0, 0, 0),
                                    Size = new Size(412, 70),
                                },
                            },

                        ]
                    }
                ]
                    } 
                ]
            },
            new Node()
            {
                Style = new Style()
                {
                    BackgroundGradient = GradientColor.Vertical(new Color("Window.Border:Active"), new Color(224, 183, 18, 0)),
                    //StrokeColor = new Color("Window.TextOutline"),
                    //StrokeWidth = 1,
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    BorderRadius = 6,
                    //StrokeRadius = 6,
                    //ShadowSize = new EdgeSize(64, 0, 0, 0),
                    Margin = new(125, 0, 0, 0),
                    Size = new Size(422, 8),
                    Anchor = Anchor.TopCenter,
                }
            },
            new Node()
            {
                Style = new Style()
                {
                    BackgroundGradient = GradientColor.Vertical(new Color(224, 183, 18, 0), new Color("Window.Border:Active")),
                    RoundedCorners = RoundedCorners.BottomRight | RoundedCorners.BottomLeft,
                    BorderRadius = 6,
                    Margin = new(0, 0, 25, 0),
                    Size = new Size(422, 8),
                    Anchor = Anchor.BottomCenter,
                }
            },
        ];

        Node.BeforeReflow += _ =>
        {

            return true;
        };
    }

    public override void OnDraw()
    {
        BottomPortion.Style.Size = new Size(ContentSize.Width, ContentSize.Height - 100);
        ScrollListBaseNode.Style.Size = new Size(BottomPortion.Style.Size.Width - 120, BottomPortion.Style.Size.Height - 60);

        if (lastUser != UserList.LocalPlayer)
        {
            lastUser = UserList.LocalPlayer;
            UserNode.SetUser(UserList.LocalPlayer?.DataBaseEntry);
        }
    }

    Stylesheet stylesheet = new Stylesheet([
        new(".TakeMeButton", new Style()
        {
            Size = new Size(100, 25),
            Anchor = Anchor.MiddleCenter,
            Margin = new EdgeSize(20, 0, 0, 0),
            BackgroundColor = new Color("PetNicknamesButton"),
            TextShadowSize = 2,
            TextShadowColor = new Color("Window.TextOutline"),
            FontSize = 10,
            TextAlign = Anchor.MiddleCenter,
            TextOffset = new Vector2(0, 0),
            Color = new Color("Window.TextLight"),
            BorderColor = new(new("Window.TitlebarBorder")),
            BorderWidth = new EdgeSize(2, 2, 2, 2),
            BorderRadius = 8,
            StrokeRadius = 8,
            IsAntialiased = false,
            RoundedCorners = RoundedCorners.All,
        }),
        new(".TakeMeButton:hover", new Style()
        {
            BackgroundColor = new Color("PetNicknamesButton:Hover"),
            FontSize = 12,
        }),
    ]);
}
