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
    Node UserListNode;
    Node TakeMeShareNode;

    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPetServices PetServices;
    readonly IImageDatabase ImageDatabase;

    public PetListWindow(in DalamudServices dalamudServices, in IPetServices petServices, in IPettableUserList userList, in IPettableDatabase database, in IImageDatabase imageDatabase) : base(dalamudServices, "Pet List")
    {
        UserList = userList;
        Database = database;
        PetServices = petServices;
        ImageDatabase = imageDatabase;

        Node.ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Size = new Size(538, 100),
                    BackgroundColor = new("Window.Background"),
                    IsAntialiased = false,
                    ShadowSize = new EdgeSize(0, 0, 64, 0),

                },
                ChildNodes = [

                    UserNode = new UserNode(in ImageDatabase),
                new Node()
                {

                    Style = new Style()
                    {
                        Size = new Size(300, 100),
                        BorderColor = new(new("Window.TitlebarBorder")),
                        BorderWidth = new EdgeSize(0, 1, 1, 0),
                        RoundedCorners = RoundedCorners.BottomRight,
                        BorderRadius = 6,
                        StrokeRadius = 6,
                        IsAntialiased = false,

                    },
                    ChildNodes =
                          [
                              /*UserListNode = new BackgroundNode(in dalamudServices)
                              {
                                  Style = new Style()
                                  {
                                      Margin = new EdgeSize(10, 0, 10, 20),
                                      Size = new Size(279, 80),
                                      BackgroundGradient = GradientColor.Horizontal(new("Window.Background"), new("Window.BackgroundLight")),
                                      Flow = Flow.Vertical,
                                      BorderColor = new(new("Window.TitlebarBorder")),
                                      BorderWidth = new EdgeSize(3, 0, 3, 3),
                                      RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                                      BorderRadius = 6,
                                      StrokeRadius = 6,
                                  },
                                  ChildNodes = [
                                      new Node()
                                      {

                                          Style = new Style()
                                          {
                                              Size = new Size(279, 40),
                                              RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                                              BorderRadius = 6,
                                              StrokeRadius = 6,
                                              TextAlign = Anchor.TopCenter,
                                              FontSize = 15,
                                              TextOffset = new Vector2(0, 15),
                                              TextOverflow = false,
                                              Color = new Color("Window.TextLight"),
                                              OutlineColor = new("Window.TextOutline"),
                                              OutlineSize = 1,
                                          },
                                          NodeValue = "Sharing has moved!"
                                      },
                                  TakeMeShareNode = new Node()
                                  {
                                      NodeValue = "Take me",
                                      Stylesheet = stylesheet,
                                      ClassList = ["TakeMeButton"]
                                      }
                                  ],
                              }*/
                         ]
                    },
                ]
            }
        ];

       /* TakeMeShareNode.OnMouseUp += _ =>
        {
            if (UserList.LocalPlayer == null) return;
            ImageDatabase.Redownload(UserList.LocalPlayer!.DataBaseEntry);
        };*/
    }

    protected override string Title { get; } = "Pet List";
    protected override string ID { get; } = "Pet List Window";

    protected override Vector2 MinSize { get; } = new Vector2(550, 240);
    protected override Vector2 MaxSize { get; } = new Vector2(550, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(550, 240);
    protected override bool HasModeToggle { get; } = true;

    public override void OnDraw()
    {
        if (UserList.LocalPlayer == null)
        {
            Close();
            return;
        }
        else
        {
            UserNode.SetUser(UserList.LocalPlayer.DataBaseEntry);
        }

    }

    Stylesheet stylesheet = new Stylesheet([
        new(".TakeMeButton", new Style()
        {
            Size = new Size(100, 25),
            Anchor = Anchor.MiddleCenter,
            Margin = new EdgeSize(20, 0 ,0 ,0),
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
