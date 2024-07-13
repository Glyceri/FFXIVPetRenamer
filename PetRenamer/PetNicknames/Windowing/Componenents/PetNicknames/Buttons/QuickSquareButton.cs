using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Base.Style;
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
        BackgroundColor = new("Window.BackgroundLight"),
        StrokeColor = new("Window.TitlebarBorder"),
        StrokeWidth = 1,
        StrokeInset = 0,
        BorderRadius = 3,
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
            BackgroundColor = new("Window.Background"),
            StrokeWidth = 2,
        }),
        new(".ClearButton:fakeDisabled", new Style()
        {
            BackgroundColor = new("Window.Background"),
            Color = new("Window.Text"),
            OutlineColor = new(0, 0, 0),
            StrokeWidth = 1,
            StrokeColor = WindowStyles.WindowBorderInactive,
        })
    ]);

}
