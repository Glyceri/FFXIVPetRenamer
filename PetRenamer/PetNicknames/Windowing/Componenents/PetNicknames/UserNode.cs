using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class UserNode : Node
{
    readonly Node ProfilePictureRect;
    readonly Node UserNameRect;
    readonly Node HomeWorldRect;
    readonly Node PetcountNode;

    public UserNode()
    {
        Stylesheet = stylesheet;
        ClassList = ["HeaderBar"];

        ChildNodes =
        [
           ProfilePictureRect = new Node()
           {
               Style = new Style()
               {
                   IconId = 1,
                   Margin = new EdgeSize(5),
                   Size = new Size(90, 90)
               }
           },
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Size = new Size(100, 100)
                },
                ChildNodes = [
                   UserNameRect = new Node()
                   {
                       Overflow = false,
                       Stylesheet = stylesheet,
                       ClassList = ["NamePlateElement"],
                   },
                    HomeWorldRect = new Node()
                    {
                        Overflow = false,
                        Stylesheet = stylesheet,
                        ClassList = ["NamePlateElement"],
                    },
                    PetcountNode = new Node()
                    {
                        Overflow = false,
                        Stylesheet = stylesheet,
                        ClassList = ["NamePlateElement"],
                    }
               ]
            }
        ];
    }

    public void SetUser(IPettableDatabaseEntry user)
    {
        UserNameRect.NodeValue = user.Name;
        HomeWorldRect.NodeValue = user.HomeworldName;
    }

    readonly Stylesheet stylesheet = new Stylesheet(
        [
            new(".NamePlateElement", new Style()
            {
                Margin = new EdgeSize(6, 5, 0, 0),
                BorderColor = new(new("Window.TitlebarBorder")),
                BorderWidth = new EdgeSize(1),
                RoundedCorners = RoundedCorners.All,
                BorderRadius = 6,
                StrokeRadius = 6,
                Size = new Size(135, 25),
                TextOffset = new System.Numerics.Vector2(5, 7),
                FontSize = 10,
                TextOverflow = false,
                OutlineColor = new("Window.TitlebarTextOutline"),
                OutlineSize = 1,
            }),
            new (".HeaderBar", new Style()
            {
                Size = new Size(240, 100),
                BorderColor = new(new("Window.TitlebarBorder")),
                BorderWidth = new EdgeSize(0, 1, 1, 1),
                RoundedCorners = RoundedCorners.BottomLeft,
                BorderRadius = 6,
                ShadowSize = new EdgeSize(0, 64, 0, 0),
                Flow = Flow.Horizontal,
                StrokeRadius = 6,
                IsAntialiased = false,
            }),
        ]
        );
}
