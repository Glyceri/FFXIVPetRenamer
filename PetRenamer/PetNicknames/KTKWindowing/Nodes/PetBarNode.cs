using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.KTKWindowing.Helpers;
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
    private readonly GuideRegistration       TransientGuideRegistration;

    public PetBarNode(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler)
        : base(parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        StylizedListButtonGroup    = new StylizedListButtonGroup(ParentAddon, windowHandler, dalamudServices, petServices, dirtyHandler);

        TransientGuideRegistration = new GuideRegistration
        {
            LeftGuideId        = 0,
            LeftPoint          = OperationGuidePoint.TopLeft,
            LeftRelativePoint  = OperationGuidePoint.TopLeft,
            LeftOffsetX        = -30,
            LeftOffsetY        = -15,

            RightGuideId       = 1,
            RightPoint         = OperationGuidePoint.TopLeft,
            RightRelativePoint = OperationGuidePoint.TopLeft,
            RightOffsetX       = 200,
            RightOffsetY       = -15,

            CallbackComponent  = StylizedListButtonGroup,
        };

        ParentAddon.TransientGuideHandler?.RegisterGuide(TransientGuideRegistration);

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

        QuickButtonBarNode = new QuickButtonBarNode(ParentAddon, WindowHandler, DalamudServices, PetServices, DirtyHandler);

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

    protected override void OnDispose()
    {
        ParentAddon.TransientGuideHandler?.DeregisterGuide(TransientGuideRegistration);
    }
}
