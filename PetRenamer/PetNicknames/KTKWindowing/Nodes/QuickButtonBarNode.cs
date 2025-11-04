using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.KTKWindowing.Addons;
using PetRenamer.PetNicknames.KTKWindowing.Helpers;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class QuickButtonBarNode : KTKComponent
{
    private readonly KTKAddon KTKAddon;

    private readonly QuickButton<PetRenameAddon>    PetRenameQuickButton;
    private readonly QuickButton<PetListAddon>      PetListQuickButton;
    private readonly QuickButton<PetListAddon>      SharingQuickButton;
    private readonly QuickButton<PetSettingsAddon>  ConfigQuickButton;
    private readonly QuickButton<KofiAddon>         KofiQuickButton;
    private readonly QuickButton<PetDevAddon>       PetDevQuickButton;

    private readonly TransientRegistration          TransientRegistration;

    private float widthOffset = 0;

    public QuickButtonBarNode(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler, KTKAddon ktkAddon) 
        : base(windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        KTKAddon      = ktkAddon;

        IsVisible     = true;

        PetRenameQuickButton = new QuickButton<PetRenameAddon>(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText        = SeIconChar.ImeAlphanumeric,
            ShouldBeVisible  = () =>
            {
                return (ktkAddon is not PetRenameAddon) || PetServices.Configuration.quickButtonsToggle;
            }
        };

        AttachNode(ref PetRenameQuickButton);

        PetListQuickButton  = new QuickButton<PetListAddon>(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.Collectible,
            ShouldBeVisible = () =>
            {
                return ((ktkAddon is not PetListAddon) || PetServices.Configuration.quickButtonsToggle) && (PetServices.Configuration.listButtonLayout == 0 || PetServices.Configuration.listButtonLayout == 2);
            }
        };

        AttachNode(ref PetListQuickButton);

        SharingQuickButton  = new QuickButton<PetListAddon>(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.Glamoured,
            ShouldBeVisible = () =>
            {
                return ((ktkAddon is not PetListAddon) || PetServices.Configuration.quickButtonsToggle) && (PetServices.Configuration.listButtonLayout == 0 || PetServices.Configuration.listButtonLayout == 1);
            }
        };

        AttachNode(ref SharingQuickButton);

        ConfigQuickButton   = new QuickButton<PetSettingsAddon>(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.BoxedQuestionMark,
            ShouldBeVisible = () =>
            {
                return (ktkAddon is not PetSettingsAddon) || PetServices.Configuration.quickButtonsToggle;
            }
        };

        AttachNode(ref ConfigQuickButton);

        KofiQuickButton     = new QuickButton<KofiAddon>(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.BoxedLetterK,
            ShouldBeVisible = () =>
            {
                return PetServices.Configuration.showKofiButton;
            }
        };

        AttachNode(ref KofiQuickButton);

        PetDevQuickButton   = new QuickButton<PetDevAddon>(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.Hexagon,
            ShouldBeVisible = () =>
            {
                return PetServices.Configuration.debugModeActive;
            }
        };

        AttachNode(ref PetDevQuickButton);

        TransientRegistration = new TransientRegistration
        {
            LeftTransientId    = 0,
            LeftPoint          = OperationGuidePoint.TopRight,
            LeftRelativePoint  = OperationGuidePoint.TopRight,
            LeftOffsetX        = -210,
            LeftOffsetY        = 23,

            RightTransientId   = 1,
            RightPoint         = OperationGuidePoint.TopRight,
            RightRelativePoint = OperationGuidePoint.TopRight,
            RightOffsetX       = 15,
            RightOffsetY       = 23,

            CallbackComponent  = this,
        };

        ktkAddon.RegisterTransient(TransientRegistration);
    }

    private void SetPosition<T>(QuickButton<T> button) where T : KTKAddon
    {
        button.X     = widthOffset;

        if (!button.IsActive)
        {
            return;
        }

        widthOffset -= button.Width * 0.85f;
    }

    private void HandlePositions()
    {
        widthOffset = Width - PetRenameQuickButton.Width;

        SetPosition(PetRenameQuickButton);
        SetPosition(PetListQuickButton);
        SetPosition(SharingQuickButton);
        SetPosition(ConfigQuickButton);
        SetPosition(KofiQuickButton);
        SetPosition(PetDevQuickButton);

        TransientRegistration.LeftOffsetX = (short)(((short)-widthOffset) + 15);
    }

    protected override void OnDirty()
        => HandlePositions();

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        float width = Height * 1.3f;

        PetRenameQuickButton.Size = new Vector2(width, Height);
        PetListQuickButton.Size   = new Vector2(width, Height);
        SharingQuickButton.Size   = new Vector2(width, Height);
        ConfigQuickButton.Size    = new Vector2(width, Height);
        KofiQuickButton.Size      = new Vector2(width, Height);
        PetDevQuickButton.Size    = new Vector2(width, Height);

        HandlePositions();
    }

    protected override void OnDispose()
    {
        KTKAddon.DeregisterTransient(TransientRegistration);

        base.OnDispose();
    }
}
