using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class HeaderBarButtonNode : Node
{
    public readonly CloseButton CloseButton;
    public readonly PetListButton PetListSquare;
    public readonly PetRenameButton PetRenameSquare;
    public readonly PetShareButton PetShareSquare;
    public readonly PetConfigButton PetConfigSquare;

    readonly Configuration Configuration;
    readonly WindowHandler WindowHandler;

    readonly bool HasExtraButtons;

    public HeaderBarButtonNode(in DalamudServices DalamudServices, in PetWindow petWindow, in Configuration configuration, in WindowHandler windowHandler, bool hasExtraButtons)
    {
        Configuration = configuration;
        WindowHandler = windowHandler;

        HasExtraButtons = hasExtraButtons;

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

        if (hasExtraButtons) return;

        RemoveChild(PetRenameSquare, true);
        RemoveChild(PetListSquare, true);
        RemoveChild(PetConfigSquare, true);
        RemoveChild(PetShareSquare, true);
    }

    bool kofiMode = false;

    KofiButton? kofiButton = null;

    public void SetKofiButton(bool value)
    {
        if (value == kofiMode) return;

        kofiMode = value;

        if (kofiMode && HasExtraButtons) ChildNodes.Add(kofiButton = new KofiButton(in Configuration, in WindowHandler));
        else
        {
            if (kofiButton != null)
            {
                ChildNodes.Remove(kofiButton);
                kofiButton.Dispose();
                kofiButton = null;
            }
        }
    }
}
