using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Enums;
using System.ComponentModel;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class PetBarNode : KTKComponent
{
    private readonly SimpleNineGridNode      DividingLineNode;
    public  readonly StylizedListButtonGroup StylizedListButtonGroup;

    private readonly QuickButtonBarNode      QuickButtonBarNode;

    public PetBarNode(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler, KTKAddon ktkAddon)
        : base(windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        StylizedListButtonGroup = new StylizedListButtonGroup(windowHandler, dalamudServices, petServices, dirtyHandler);

        AttachNode(ref StylizedListButtonGroup);

        for (int i = 0; i < (int)PetWindowMode.COUNT; i++)
        {
            PetWindowMode currentMode         = (PetWindowMode)i;

            DescriptionAttribute? description = currentMode.GetAttribute<DescriptionAttribute>();

            if (description == null)
            {
                continue;
            }

            StylizedListButtonGroup.AddButton(description.Description, () =>
            {
                OnButtonClickedForPetMode(currentMode);
            });
        }

        QuickButtonBarNode = new QuickButtonBarNode(WindowHandler, DalamudServices, PetServices, DirtyHandler, ktkAddon);

        AttachNode(ref QuickButtonBarNode);

        SetSelectedButton();

        DividingLineNode       = new SimpleNineGridNode 
        {
            TexturePath        = "ui/uld/WindowA_Line.tex",
            TextureCoordinates = Vector2.Zero,
            TextureSize        = new Vector2(32.0f, 4.0f),
            IsVisible          = true,
            LeftOffset         = 12,
            RightOffset        = 12,
            Height             = 4,
        };

        AttachNode(ref DividingLineNode);

        DividingLineNode.IsVisible = PetServices.Configuration.showDividingLine;
    }

    public override bool OnCustomInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
        => StylizedListButtonGroup.OnCustomInput(inputId, inputState);

    private void OnButtonClickedForPetMode(PetWindowMode petMode)
        => DirtyHandler.DirtyPetMode(petMode);

    protected override void OnDirty()
    {
        SetSelectedButton();

        DividingLineNode.IsVisible = PetServices.Configuration.showDividingLine;
    }

    private void SetSelectedButton()
    {
        string                descriptionText = string.Empty;
        DescriptionAttribute? description     = PetMode.GetAttribute<DescriptionAttribute>();

        if (description != null)
        {
            descriptionText = description.Description;
        }

        if (descriptionText.IsNullOrWhitespace())
        {
            return;
        }

        foreach (StylizedListButton button in StylizedListButtonGroup.Buttons)
        {
            if (button.LabelText.TextValue != descriptionText)
            {
                continue;
            }

            StylizedListButtonGroup.SetButtonAsActive(button);

            break;
        }
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        float baseHeight             = Height - DividingLineNode.Height;

        StylizedListButtonGroup.Size = new Vector2(220, baseHeight);

        QuickButtonBarNode.X         = StylizedListButtonGroup.Size.X;
        QuickButtonBarNode.Size      = new Vector2(Width - StylizedListButtonGroup.Size.X, baseHeight);

        DividingLineNode.Width       = Width;
        DividingLineNode.X           = X - DividingLineNode.LeftOffset;
        DividingLineNode.Y           = baseHeight;
    }
}
