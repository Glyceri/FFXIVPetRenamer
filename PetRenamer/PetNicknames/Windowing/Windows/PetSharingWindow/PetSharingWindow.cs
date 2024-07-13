using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.WindowNodes;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetSharingWindow;

internal class PetSharingWindow : PetWindow
{
    protected override string ID { get; } = "PetSharing";
    protected override Vector2 MinSize { get; } = new Vector2(425, 300);
    protected override Vector2 MaxSize { get; } = new Vector2(425, 1500);
    protected override Vector2 DefaultSize { get; } = new Vector2(425, 500);
    protected override bool HasModeToggle { get; } = false;
    protected override string Title { get; } = "Sharing";

    readonly UserNode UserNode;
    readonly SmallHeaderNode TitleBarNode;

    public PetSharingWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, in IImageDatabase imageDatabase) : base(windowHandler, dalamudServices, configuration, "PetSharing")
    {
        IsOpen = false;

        ContentNode.ChildNodes = [
            UserNode = new UserNode(in dalamudServices, in imageDatabase),
            TitleBarNode = new SmallHeaderNode("Sharing")
            {
                Style = new Style()
                {
                    //Anchor = Anchor.TopCenter,
                    Margin = new EdgeSize(5, 0, 0, 0),
                    Size = new Size(415, 20),
                    FontSize = 16,
                }
            },
        ];
    }

    public override void OnDraw()
    {

    }
}
