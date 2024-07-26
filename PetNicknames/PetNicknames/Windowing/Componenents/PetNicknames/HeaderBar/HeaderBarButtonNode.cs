using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class HeaderBarButtonNode : Node
{
    public readonly CloseButton CloseButton;
    public readonly PetRenameButton PetRenameSquare;
    public readonly PetShareButton PetShareSquare;
    public readonly PetConfigButton PetConfigSquare;

    readonly WindowHandler WindowHandler;
    readonly IPetWindow PetWindow;

    readonly bool HasExtraButtons;

    public HeaderBarButtonNode(in DalamudServices DalamudServices, in PetWindow petWindow, in WindowHandler windowHandler, bool hasExtraButtons)
    {
        PetWindow = petWindow;
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
            PetRenameSquare = new PetRenameButton(in windowHandler, petWindow),
            PetShareSquare = new PetShareButton(in windowHandler, petWindow),
            PetConfigSquare = new PetConfigButton(in windowHandler, petWindow),
        ];

        if (hasExtraButtons) return;

        RemoveChild(PetRenameSquare, true);
        RemoveChild(PetConfigSquare, true);
        RemoveChild(PetShareSquare, true);
    }

    bool kofiMode = false;

    KofiButton? kofiButton = null;

    public void SetKofiButton(bool value)
    {
        if (value == kofiMode) return;

        kofiMode = value;

        if (kofiMode && HasExtraButtons) ChildNodes.Add(kofiButton = new KofiButton(in WindowHandler, PetWindow));
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
