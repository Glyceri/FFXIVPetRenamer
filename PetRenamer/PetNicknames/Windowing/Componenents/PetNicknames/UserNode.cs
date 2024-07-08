using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System.Linq;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class UserNode : Node
{
    readonly Node ProfilePictureRect;
    readonly Node UserNameRect;
    readonly Node HomeWorldRect;
    readonly Node PetcountNode;
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
           ProfilePictureRect = new Node()
           {
               Stylesheet = stylesheet,
               ClassList = ["NamePlateElement"],
               Style = new Style()
               {
                   Margin = new EdgeSize(5),
                   BorderWidth = new EdgeSize(5),
                   Size = new Size(90, 90),
                   BackgroundColor = new Color(0, 0, 0, 0),
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
        PetcountNode.NodeValue = $"Nickname count: {user.ActiveDatabase.IDs.Count()}";
        currentEntry = user;
        _userTexture = ImageDatabase.GetWrapFor(user);
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        base.OnDraw(drawList);
        if (_userTexture == null) return;
        nint handle = _userTexture.ImGuiHandle;
        Rect contentRect = ProfilePictureRect.Bounds.ContentRect;
        drawList.AddImageQuad(handle, contentRect.TopLeft, contentRect.TopRight, contentRect.BottomRight, contentRect.BottomLeft);
        base.OnDraw(drawList);
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
                Size = new Size(135, 25),
                TextOffset = new System.Numerics.Vector2(5, 7),
                FontSize = 10,
                TextOverflow = false,
                Color = new Color("Window.TextLight"),
                OutlineColor = new("Window.TextOutline"),
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
