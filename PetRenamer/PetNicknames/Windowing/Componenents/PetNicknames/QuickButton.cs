using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class QuickButton : Node
{
    public Action? Clicked;

    public QuickButton(string text)
    {
        Stylesheet = stylesheet;
        ClassList = ["Button"];
        NodeValue = text;

        OnMouseUp += _ =>
        {
            Clicked?.Invoke();
        };
    }

    public void SetText(string text)
    {
        NodeValue = text;
    }

    Stylesheet stylesheet = new Stylesheet([
        new(".Button", new Style()
        {
            Size = new Size(60, 20),
            FontSize = 10,
            TextOverflow = true,
            Color = new Color("Window.TextLight"),
            TextAlign = Anchor.MiddleCenter,
            OutlineColor = new("Window.TextOutlineButton"),
            OutlineSize = 1,
            BorderInset = new EdgeSize(7, 15, 3, 15),
            
        }),
        new(".Button:hover", new Style()
        {
            BorderColor = new BorderColor(new Color(255, 255, 255)),
            BorderWidth = new EdgeSize(0, 0, 2, 0),
        }),
    ]);
}
