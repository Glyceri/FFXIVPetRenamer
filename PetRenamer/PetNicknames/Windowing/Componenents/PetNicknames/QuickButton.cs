using PetRenamer.PetNicknames.Services;
using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class QuickButton : Node
{
    public Action? Clicked;

    readonly DalamudServices DalamudServices;

    public QuickButton(in DalamudServices services, string text)
    {
        DalamudServices = services;
        Stylesheet = stylesheet;
        ClassList = ["Button"];
        NodeValue = text;

        OnMouseUp += _ =>
        {
            DalamudServices.Framework.Run(() => Clicked?.Invoke());
        };
    }

    public void SetText(string text)
    {
        NodeValue = text;
    }

    readonly Stylesheet stylesheet = new Stylesheet([
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
