using Dalamud.Interface;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class CloseButton : QuickSquareButton
{
    readonly DalamudServices DalamudServices;
    readonly IPetWindow PetWindow;

    public CloseButton(in DalamudServices dalamudServices, in IPetWindow petWindow)
    {
        DalamudServices = dalamudServices;
        PetWindow = petWindow;

        NodeValue = FontAwesomeIcon.Times.ToIconString();
        Style = new Style()
        {
            Anchor = Anchor.TopRight,
            Size = new Size(20, 20),
            FontSize = 12,
            Margin = new EdgeSize(6, 6, 0, 0),
        };

        OnClick += () => DalamudServices.Framework.Run(PetWindow.Close);
    }
}
