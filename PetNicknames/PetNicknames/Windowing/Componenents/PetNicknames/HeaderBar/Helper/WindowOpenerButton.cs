using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;

internal class WindowOpenerButton<T> : QuickSquareButton where T : PetWindow
{
    readonly WindowHandler WindowHandler;

    public WindowOpenerButton(in WindowHandler windowHandler, in IPetWindow petWindow)
    {
        WindowHandler = windowHandler;

        Style = new Style()
        {
            Anchor = Anchor.TopRight,
            Size = new Size(20, 20),
            FontSize = 12,
            Margin = new EdgeSize(6, 0),
        };

        if (typeof(T) == petWindow.GetType()) Style.IsVisible = false;

        OnClick = WindowHandler.Open<T>;
    }
}
