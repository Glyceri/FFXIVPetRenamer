using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;

internal class ProfilePictureNode : Node
{
    public readonly Node RedownloadNode;

    readonly IImageDatabase ImageDatabase;

    IDalamudTextureWrap? _userTexture = null;

    IPettableDatabaseEntry? activeUser;

    readonly DalamudServices DalamudServices;

    public ProfilePictureNode(in DalamudServices services, in IImageDatabase imageDatabase)
    {
        DalamudServices = services;
        ImageDatabase = imageDatabase;

        ChildNodes = [
            RedownloadNode = new Node()
            {
                NodeValue = SeIconChar.QuestSync.ToIconString(),
                Stylesheet = styleSheet,
                ClassList = ["RedownloadButton"],
                Tooltip = "(Re)download Profile Picture",
            }
        ];

        RedownloadNode.OnMouseUp += _ => DalamudServices.Framework.Run(() => ImageDatabase.Redownload(activeUser!));
    }

    public void SetUser(IPettableDatabaseEntry? user)
    {
        activeUser = user;
        RedownloadNode.Style.IsVisible = user != null;
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        _userTexture = ImageDatabase.GetWrapFor(activeUser);
        bool isBeingDownloaded = ImageDatabase.IsBeingDownloaded(activeUser);
        RedownloadNode.Style.IsVisible = !isBeingDownloaded;

        if (_userTexture == null) return;

        nint handle = _userTexture.ImGuiHandle;
        Rect contentRect = Bounds.ContentRect;
        drawList.AddImageQuad(handle, contentRect.TopLeft, contentRect.TopRight, contentRect.BottomRight, contentRect.BottomLeft);
        base.OnDraw(drawList);
    }

    Stylesheet styleSheet = new Stylesheet([
        new(".RedownloadButton", new Style()
        {
            Anchor = Anchor.BottomRight,
            TextAlign = Anchor.MiddleCenter,
            Color = new Color("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
            Size = new Size(32, 32)
        }),
        new(".RedownloadButton:hover", new Style()
        {
            Color = new Color("Window.Text"),
        })
    ]);
}
