using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class HeaderBarButtonNode : Node
{
    public readonly CloseButton CloseButton;
    public readonly PetListButton PetListSquare;
    public readonly PetRenameButton PetRenameSquare;
    public readonly PetShareButton PetShareSquare;
    public readonly PetConfigButton PetConfigSquare;

    public HeaderBarButtonNode(in DalamudServices DalamudServices, in PetWindow petWindow, in Configuration configuration, in WindowHandler windowHandler)
    {
        Style = new Style()
        {
            Anchor = Anchor.TopRight,
            Flow = Flow.Horizontal,
            Gap = 5,
        };

        ChildNodes = [
            CloseButton = new CloseButton(in DalamudServices, petWindow),
            PetRenameSquare = new PetRenameButton(in configuration, in windowHandler),
            PetListSquare = new PetListButton(in configuration, in windowHandler),
            PetShareSquare = new PetShareButton(in configuration, in windowHandler),
            PetConfigSquare = new PetConfigButton(in configuration, in windowHandler),
        ];
    }

}
