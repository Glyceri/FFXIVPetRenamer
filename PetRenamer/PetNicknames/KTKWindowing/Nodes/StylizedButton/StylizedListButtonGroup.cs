using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;

internal class StylizedListButtonGroup : SimpleComponentNode
{
    private readonly IPetServices PetServices;

    private readonly List<StylizedListButton> _buttons = [];

    public StylizedListButtonGroup(IPetServices petServices)
    {
        PetServices = petServices;
        IsVisible   = true;
    }

    public StylizedListButton[] Buttons
        => [.. _buttons];

    private void RecalculateLayout()
    {
        float size = Width / _buttons.Count;

        int buttonCount = _buttons.Count;

        for (int i = 0; i < buttonCount; i++)
        {
            StylizedListButton button = _buttons[i];

            button.Size     = new Vector2(size, Height);
            button.Position = new Vector2(i * size * 0.89f, 0);
        }
    }

    private void ClickHandler(StylizedListButton button)
    {
        foreach (StylizedListButton _button in _buttons)
        {
            _button.IsSelected = false;
        }

        button.IsSelected = true;
    }

    public void AddButton(SeString label, Action callback)
    {
        StylizedListButton newButton = new DarkStylizedButton(PetServices)
        {
            LabelText = label,
        };

        newButton.AddEvent(AtkEventType.ButtonClick, () =>
        {
            ClickHandler(newButton);

            callback();
        });

        _buttons.Add(newButton);

        if (_buttons.Count == 1)
        {
            newButton.IsSelected = true;
        }

        PetServices.NativeController.AttachNode(newButton, this);

        RecalculateLayout();
    }

    public void RemoveButton(SeString label)
    {
        StylizedListButton? button = _buttons.FirstOrDefault(button =>
        {
            return button.LabelText == label;
        });

        if (button == null)
        {
            return;
        }

        button.Dispose();
        
        _ = _buttons.Remove(button);

        RecalculateLayout();
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        RecalculateLayout();
    }
}
