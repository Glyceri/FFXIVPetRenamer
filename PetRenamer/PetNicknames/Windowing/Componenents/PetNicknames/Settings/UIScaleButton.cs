using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using System;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;

internal class UIScaleButton : QuickSquareButton
{
    public UIScaleButton(string text, Action callback)
    {
        NodeValue = text;
        Style = new Una.Drawing.Style()
        {
            Size = new Una.Drawing.Size(30, 17),
            FontSize = 8,
        };

        OnClick += callback.Invoke;
    }
}
