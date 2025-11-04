using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;

internal class StylizedListButtonGroup : KTKComponent
{
    private readonly List<StylizedListButton> _buttons = [];

    private int currentIndex = 0;

    public StylizedListButtonGroup(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        IsVisible = true;
    }

    public StylizedListButton[] Buttons
        => [.. _buttons];

    public override bool OnCustomInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {
        if (inputState != AtkEventData.AtkInputData.InputState.Down)
        {
            return false;
        }

        if (inputId != NavigationInputId.LB && inputId != NavigationInputId.RB) 
        {
            return false; 
        }

        if (_buttons.Count == 0) 
        {
            return false;
        }

        int newIndex = currentIndex;

        if (inputId == NavigationInputId.LB)
        {
            newIndex--;
        }
        else if (inputId == NavigationInputId.RB)
        {
            newIndex++;
        }

        int min = 0;
        int max = _buttons.Count - 1;

        if (newIndex < min)
        {
            newIndex = max;
        }

        if (newIndex > max)
        {
            newIndex = min;
        }

        ClickButton(newIndex);

        return true;
    }

    private unsafe void RecalculateLayout()
    {
        float size = Width / _buttons.Count;

        int buttonCount = _buttons.Count;

        for (int i = 0; i < buttonCount; i++)
        {
            StylizedListButton button = _buttons[i];

            button.Size     = new Vector2(size, Height);
            button.Position = new Vector2(i * size * 0.89f, 0);

            button.CollisionNode.LinkedComponent->ComponentFlags = 1;
            (&button.InternalComponentNode->Component->CursorNavigationInfo)->CursorType = 3;
            (&button.InternalComponentNode->Component->CursorNavigationInfo)->Index = (byte)i;
            (&button.InternalComponentNode->Component->CursorNavigationInfo)->RightIndex = (byte)(i + 1);
            (&button.InternalComponentNode->Component->CursorNavigationInfo)->LeftIndex = (byte)(i - 1);
        }
    }

    public void ClickButton(int index)
        => ClickButton(_buttons[index]);

    public void ClickButton(string label)
    {
        foreach (StylizedListButton button in _buttons)
        {
            if (button.LabelText.TextValue != label)
            {
                continue;
            }

            ClickButton(button);

            break;
        }
    }

    public void ClickButton(StylizedListButton button)
    {
        ClickHandler(button);

        button.OnClick?.Invoke();
    }

    public void SetButtonAsActive(StylizedListButton button)
    {
        foreach (StylizedListButton _button in _buttons)
        {
            _button.IsSelected = false;
        }

        button.IsSelected = true;
    }

    private void ClickHandler(StylizedListButton button)
    {
        currentIndex = _buttons.IndexOf(button);
    }

    public void AddButton(SeString label, Action callback)
    {
        StylizedListButton newButton = new DarkStylizedButton(PetServices)
        {
            LabelText = label,
        };

        newButton.OnClick = () =>
        {
            ClickHandler(newButton);

            callback();
        };

        _buttons.Add(newButton);

        if (_buttons.Count == 1)
        {
            ClickHandler(newButton);
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
