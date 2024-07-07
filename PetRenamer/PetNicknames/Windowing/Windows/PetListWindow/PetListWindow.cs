using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

internal class PetListWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPetServices PetServices;

    public PetListWindow(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database) : base(dalamudServices, "Pet List")
    {
        UserList = userList;
        Database = database;
        PetServices = petServices;
    }

    protected override string Title { get; } = "Pet List";
    protected override string ID { get; } = "Pet List Window";

    protected override Vector2 MinSize { get; } = new Vector2(400, 240);
    protected override Vector2 MaxSize { get; } = new Vector2(400, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(400, 240);
    protected override bool HasModeToggle { get; } = true;

    public override void OnDraw()
    {

    }

    protected override Node Node { get; } = new Node()
    {
        ChildNodes = [
            new Node()
            {
                
                Style = new Style()
                {
                    Size = new Size(393, 100),
                    BackgroundColor = new("Window.Background"),
                    IsAntialiased = false,
                    ShadowSize = new EdgeSize(0, 0, 64, 0),
                    
                },
                ChildNodes = [
                     new Node()
                     {
                         Style = new Style()
                         {
                             Size = new Size(100, 100),
                             BorderColor = new(new("Window.TitlebarBorder")),
                             BorderWidth = new EdgeSize(0, 1, 1, 1),
                             RoundedCorners = RoundedCorners.BottomLeft,
                             BorderRadius = 6,
                             ShadowSize = new EdgeSize(0, 64, 0, 0),

                             StrokeRadius = 6,
                             IsAntialiased = false,
                         }
                     },
                      new Node()
                      {
                          Style = new Style()
                          {
                              Size = new Size(293, 100),
                              BorderColor = new(new("Window.TitlebarBorder")),
                              BorderWidth = new EdgeSize(0, 1, 1, 0),
                              RoundedCorners = RoundedCorners.BottomRight,
                              BorderRadius = 6,
                              StrokeRadius = 6,
                              IsAntialiased = false,
                          },
                          ChildNodes =
                          [
                                /*new Node()
                                {
                                    Style = new Style()
                                    {
                                        Margin = new EdgeSize(3, 1, 3, 1),
                                        Size = new Size(94, 94),
                                        BackgroundColor = new("Window.Background"),
                                        BorderColor = new(new("Window.TitlebarBorder")),
                                        BorderWidth = new(1),
                                        IsAntialiased = false,
                                        RoundedCorners = RoundedCorners.All,
                                        BorderRadius = 6,
                                    }
                                },
                              new Node()
                              {
                                  Style = new Style()
                                  {
                                      Margin = new EdgeSize(3, 0, 3, 0),
                                      Size = new Size(80, 94),
                                      BackgroundColor = new("Window.Background"),
                                      BorderColor = new(new("Window.TitlebarBorder")),
                                      BorderWidth = new(1),
                                      IsAntialiased = false,
                                      RoundedCorners = RoundedCorners.All,
                                      BorderRadius = 6,
                                  }
                              },*/
                          ]
                      },
                   
                ]
            }
        ]
    };
}
