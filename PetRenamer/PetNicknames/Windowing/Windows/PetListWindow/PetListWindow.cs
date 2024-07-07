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

    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPetServices PetServices;

    public PetListWindow(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database) : base(dalamudServices, "Pet List")
    {
        UserList = userList;
        Database = database;
        PetServices = petServices;

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

                    UserNode = new UserNode()
                     {

                     },
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

                          ]
                    },
                ]
            }
        ];
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

    protected override Node Node { get; } = new Node()
    {

    };
}
