using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.WindowNodes;

internal class UserNode : Node
{
    readonly ProfilePictureNode ProfilePictureRect;
    readonly Node PlateRect;
    readonly RenameTitleNode UserNameRect;
    readonly RenameTitleNode HomeWorldRect;
    readonly RenameTitleNode PetcountNode;
    readonly IImageDatabase ImageDatabase;

    IPettableDatabaseEntry? currentEntry = null;

    readonly DalamudServices DalamudServices;

    public UserNode(in DalamudServices services, in IImageDatabase imageDatabase)
    {
        DalamudServices = services;
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
                    Margin = new EdgeSize(23, 0, 0, 10),
                    Gap = 3,
                },
                ChildNodes = [
                    UserNameRect = new RenameTitleNode(in DalamudServices, Translator.GetLine("Name") + ":", "..."),
                    HomeWorldRect = new RenameTitleNode(in DalamudServices, Translator.GetLine("Homeworld") + ":", "..."),
                    PetcountNode = new RenameTitleNode(in DalamudServices, Translator.GetLine("Petcount") + ":", "..."),
                ]
            },
            ProfilePictureRect = new ProfilePictureNode(in DalamudServices, in imageDatabase)
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
        PetcountNode.SetText(user?.ActiveDatabase.Length.ToString() ?? "...");
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
            new(".HeaderBar", new Style()
            {
                Size = new Size(410, 100),
                Flow = Flow.Horizontal,
            }),
        ]
    );
}
