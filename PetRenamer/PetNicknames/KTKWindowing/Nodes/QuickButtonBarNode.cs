using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.KTKWindowing.Addons;
using PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class QuickButtonBarNode : KTKComponent
{
    // PET DEV      SeIconChar.Hexagon
    // KOFI         SeIconChar.Gil
    // SETTINGS     SeIconChar.BoxedQuestionMark
    // SHARE        SeIconChar.Glamoured
    // LIST         SeIconChar.Collectable
    // RENAME       SeIconChar.ImeAlphanumeric

    private readonly List<StylizedListButton> _buttons = [];

    private readonly AlignedHorizontalListNode HolderNode;

    public QuickButtonBarNode(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        IsVisible     = true;

        HolderNode    = new AlignedHorizontalListNode
        {
            Alignment        = HorizontalListAnchor.Right,
            ItemSpacing      = -4f,
            FirstItemSpacing = 4f,
            IsVisible        = true
        };

        AttachNode(ref HolderNode);

        RefreshList();
    }

    private void ClearList()
    {
        HolderNode.Clear();

        _buttons.Clear();
    }

    private void RefreshList()
    {
        if (!IsVisible)
        {
            return;
        }

        ClearList();

        AddButton<PetRenameAddon>(SeIconChar.Hexagon);
        AddButton<PetRenameAddon>(SeIconChar.BoxedLetterK);
        AddButton<PetSettingsAddon>(SeIconChar.BoxedQuestionMark);
        AddButton<PetRenameAddon>(SeIconChar.Glamoured);
        AddButton<PetRenameAddon>(SeIconChar.Collectible);
        AddButton<PetRenameAddon>(SeIconChar.ImeAlphanumeric);

        RecalculateList();
    }

    private void AddButton<T>(SeIconChar icon) where T : KTKAddon
    {
        StylizedListButton button = AddButton(icon.ToIconString(), () =>
        {
            if (PetServices.Configuration.quickButtonsToggle)
            {
                WindowHandler.Toggle<T>();
            }
            else
            {
                WindowHandler.Open<T>();
            }

            DirtyHandler.DirtyWindow();
        });

        button.IsSelected = WindowHandler.IsOpen<T>();
    }

    private void RecalculateList()
    {
        float size = Height * 1.3f;

        int buttonCount = _buttons.Count;

        for (int i = 0; i < buttonCount; i++)
        {
            StylizedListButton button = _buttons[i];

            button.Size = new Vector2(size, Height);
        }

        HolderNode.RecalculateLayout();
    }

    private void ClickHandler(StylizedListButton button)
    {

    }

    public StylizedListButton AddButton(SeString label, Action callback)
    {
        StylizedListButton newButton = new LightStylizedButton(PetServices)
        {
            LabelText = label,
        };

        newButton.AddEvent(AtkEventType.ButtonClick, () =>
        {
            ClickHandler(newButton);

            callback();
        });

        _buttons.Add(newButton);

        HolderNode.AddNode(newButton);

        return newButton;
    }

    protected override void OnDirty()
        => RefreshList();

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        HolderNode.Size = Size;

        RecalculateList();
    }
}
