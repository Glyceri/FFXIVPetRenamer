using Dalamud.Interface;
using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;

internal class QuickSquareButton : Node
{
    public new Action? OnClick;

    public QuickSquareButton()
    {
        Stylesheet = stylesheet;
        ClassList = ["ClearButton"];
        NodeValue = FontAwesomeIcon.Eye.ToIconString();
        OnMouseUp += _ => ButtonClicked();
    }

    protected virtual void ButtonClicked()
    {
        OnClick?.Invoke();
    }

    readonly Stylesheet stylesheet = new Stylesheet([
    new(".ClearButton", new Style()
    {
        Size = new(15, 15),
        BackgroundColor = new("Button.Background"),
        StrokeColor = new("Outline"),
        StrokeWidth = 1,
        StrokeInset = 0,
        TextAlign = Anchor.MiddleCenter,
        Font = 2,
        FontSize = 10,
        Color = new("Window.TextLight"),
        OutlineColor = new("Window.TextOutline"),
        TextOverflow = true,
        IsAntialiased = false,
    }),
        new(".ClearButton:hover", new Style()
        {
            BackgroundColor = new("Button.Background:Hover"),
            StrokeWidth = 2,
        }),
        new(".ClearButton:fakeDisabled", new Style()
        {
            BackgroundColor = new("Button.Background:Inactive"),
            Color = new("Window.Text"),
            OutlineColor = new("Window.TextOutlineButton"),
            StrokeWidth = 1,
            StrokeColor = new Color("Outline:Fade"),
        }),
        new(".ClearButton:disabled", new Style()
        {
            BackgroundColor = new("Button.Background:Inactive"),
            Color = new("Window.Text"),
            OutlineColor = new("Window.TextOutlineButton"),
            StrokeWidth = 1,
            StrokeColor = new Color("Outline:Fade"),
        })
    ]);
}
