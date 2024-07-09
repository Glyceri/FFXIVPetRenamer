using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System.Linq;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class UserNode : Node
{
    readonly ProfilePictureNode ProfilePictureRect;
    readonly Node PlateRect;
    readonly RenameTitleNode UserNameRect;
    readonly RenameTitleNode HomeWorldRect;
    readonly RenameTitleNode PetcountNode;
    readonly IImageDatabase ImageDatabase;

    IDalamudTextureWrap? _userTexture = null;
    IPettableDatabaseEntry? currentEntry = null;

    public UserNode(in IImageDatabase imageDatabase)
    {
        ImageDatabase = imageDatabase;

        Stylesheet = stylesheet;
        ClassList = ["HeaderBar"];

        ChildNodes =
        [
            PlateRect = new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Size = new Size(300, 100),
                    Margin = new EdgeSize(3, 0, 0, 10),
                },
                ChildNodes = [
                    UserNameRect = new RenameTitleNode("Name:", "..."),
                    HomeWorldRect = new RenameTitleNode("Homeworld:", "..."),
                    PetcountNode = new RenameTitleNode("Petcount:", "..."),
                ]
            },
            ProfilePictureRect = new ProfilePictureNode(in imageDatabase)
            {
                Style = new Style()
                {
                    Margin = new EdgeSize(5),
                    Size = new Size(90, 90),
                }
            },
        ];
    }

    public void SetUser(IPettableDatabaseEntry? user)
    {
        UserNameRect.SetText(user?.Name ?? "...");
        HomeWorldRect.SetText(user?.HomeworldName ?? "...");
        PetcountNode.SetText(user?.ActiveDatabase.IDs.Count().ToString() ?? "...");
        currentEntry = user;
        ProfilePictureRect.SetUser(user);
    }

    readonly Stylesheet stylesheet = new Stylesheet(
        [
            new(".NamePlateElement", new Style()
            {
                Margin = new EdgeSize(6, 5, 0, 0),
                BorderColor = new(new("Window.TitlebarBorder")),
                BackgroundColor = new Color("Window.BackgroundLight"),
                BorderWidth = new EdgeSize(3),
                RoundedCorners = RoundedCorners.All,
                BorderRadius = 6,
                StrokeRadius = 6,
                Size = new Size(235, 25),
                TextOffset = new System.Numerics.Vector2(5, 7),
                FontSize = 10,
                TextOverflow = false,
                Color = new Color("Window.TextLight"),
                OutlineColor = new("Window.TextOutline"),
                OutlineSize = 1,
            }),
            new (".HeaderBar", new Style()
            {
                Size = new Size(410, 100),
                Flow = Flow.Horizontal,
            }),
        ]
    );
}
