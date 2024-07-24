using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar.Helper;

internal class WindowOpenerButton<T> : QuickSquareButton where T : PetWindow
{
    readonly Configuration Configuration;
    readonly WindowHandler WindowHandler;

    public WindowOpenerButton(in Configuration configuration, in WindowHandler windowHandler)
    {
        Configuration = configuration;
        WindowHandler = windowHandler;

        Style = new Style()
        {
            Anchor = Anchor.TopRight,
            Size = new Size(20, 20),
            FontSize = 12,
            Margin = new EdgeSize(6, 0),
        };

        OnClick = () =>
        {
            if (Configuration.quickButtonsToggle)
            {
                WindowHandler.Toggle<T>();
            }
            else
            {
                WindowHandler.Open<T>();
            }
        };
    }
}
