using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.KTKWindowing.Addons;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.KTKWindowing.Helpers;
using PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkEventData.AtkInputData;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class QuickButtonBarNode : KTKComponent
{
    private readonly QuickButton<PetRenameAddon>    PetRenameQuickButton;
    private readonly QuickButton<PetListAddon>      PetListQuickButton;
    private readonly QuickButton<PetListAddon>      SharingQuickButton;
    private readonly QuickButton<PetSettingsAddon>  ConfigQuickButton;
    private readonly QuickButton<KofiAddon>         KofiQuickButton;
    private readonly QuickButton<PetDevAddon>       PetDevQuickButton;

    private readonly GuideRegistration              GuideRegistration;

    private float widthOffset    = 0;
    private bool  requestRefresh = false;

    private readonly HighlightableLightStylizedButton[] ButtonReferences;

    private int currentIndex      = 0;
    private int activeButtonCount = 0;

    private bool requireDown = false;

    public QuickButtonBarNode(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        PetRenameQuickButton = new QuickButton<PetRenameAddon>(ParentAddon, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText        = SeIconChar.ImeAlphanumeric,
            ShouldBeVisible  = () =>
            {
                return (ParentAddon is not PetRenameAddon) || PetServices.Configuration.quickButtonsToggle;
            }
        };

        AttachNode(ref PetRenameQuickButton);

        PetListQuickButton  = new QuickButton<PetListAddon>(ParentAddon, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.Collectible,
            ShouldBeVisible = () =>
            {
                return ((ParentAddon is not PetListAddon) || PetServices.Configuration.quickButtonsToggle) && (PetServices.Configuration.listButtonLayout == 0 || PetServices.Configuration.listButtonLayout == 2);
            }
        };

        AttachNode(ref PetListQuickButton);

        SharingQuickButton  = new QuickButton<PetListAddon>(ParentAddon, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.Glamoured,
            ShouldBeVisible = () =>
            {
                return ((ParentAddon is not PetListAddon) || PetServices.Configuration.quickButtonsToggle) && (PetServices.Configuration.listButtonLayout == 0 || PetServices.Configuration.listButtonLayout == 1);
            }
        };

        AttachNode(ref SharingQuickButton);

        ConfigQuickButton   = new QuickButton<PetSettingsAddon>(ParentAddon, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.BoxedQuestionMark,
            ShouldBeVisible = () =>
            {
                return (ParentAddon is not PetSettingsAddon) || PetServices.Configuration.quickButtonsToggle;
            }
        };

        AttachNode(ref ConfigQuickButton);

        KofiQuickButton     = new QuickButton<KofiAddon>(ParentAddon, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.BoxedLetterK,
            ShouldBeVisible = () =>
            {
                return PetServices.Configuration.showKofiButton;
            }
        };

        AttachNode(ref KofiQuickButton);

        PetDevQuickButton   = new QuickButton<PetDevAddon>(ParentAddon, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            LabelText       = SeIconChar.Hexagon,
            ShouldBeVisible = () =>
            {
                return PetServices.Configuration.debugModeActive;
            }
        };

        AttachNode(ref PetDevQuickButton);

        ButtonReferences =
        [
            PetDevQuickButton.Button,
            KofiQuickButton.Button,
            ConfigQuickButton.Button,
            SharingQuickButton.Button,
            PetListQuickButton.Button,
            PetRenameQuickButton.Button
        ];

        GuideRegistration      = new GuideRegistration
        {
            LowerGuideId       = 3,

            LeftGuideId        = 0,
            LeftPoint          = OperationGuidePoint.TopRight,
            LeftRelativePoint  = OperationGuidePoint.TopRight,
            LeftOffsetX        = -210,
            LeftOffsetY        = 23,

            RightGuideId       = 1,
            RightPoint         = OperationGuidePoint.TopRight,
            RightRelativePoint = OperationGuidePoint.TopRight,
            RightOffsetX       = 15,
            RightOffsetY       = 23,

            OnSelected         = OnSelected,
            OnUnselected       = OnUnselected,
            CallbackComponent  = this,
        };

        ParentAddon.TransientGuideHandler?.RegisterGuide(GuideRegistration);
    }

    public override bool OnCustomInput(NavigationInputId inputId, InputState inputState)
    {
        bool inputIsDown = (inputState == InputState.Down);

        if (!inputIsDown && requireDown)
        {
            return false;
        }

        if (inputIsDown && requireDown)
        {
            requireDown = false;
        }

        if (!inputIsDown && inputState != InputState.Held)
        {
            return false;
        }

        if (inputId == NavigationInputId.RB)
        {
            AddIndex(inputState);

            return true;    
        }

        if (inputId == NavigationInputId.LB)
        {
            RemoveIndex(inputState);

            return true;
        }

        if (inputId == NavigationInputId.NintendoA)
        {
            return ClickIndex();
        }

        return false;
    }

    private bool ClickIndex()
    {
        if (currentIndex < 0)
        {
            return false;
        }

        if (currentIndex >= ButtonReferences.Length)
        {
            return false;
        }

        ButtonReferences[currentIndex].Click();

        return true;
    }

    private void OnSelected()
    {
        ResetIndex();
    }

    private void OnUnselected()
    {
        ClearIndex();
    }

    private void ResetActiveButtonCount()
        => activeButtonCount = ActiveButtonCount();

    private int ActiveButtonCount()
    {
        int buttonCount = 0;

        for (int i = 0; i < ButtonReferences.Length; i++)
        {
            HighlightableLightStylizedButton button = ButtonReferences[i];

            if (!button.IsVisible)
            {
                continue;
            }

            buttonCount++;
        }

        return buttonCount;
    }

    private bool HandleInvalidIndex()
    {
        if (activeButtonCount <= 0)
        {
            currentIndex = -1;

            SetHighlightedIndex();

            return true;
        }

        return false;
    }

    private void ResetIndex()
    {
        PetServices.PetLog.LogFatal("RESET INDEX");

        ResetActiveButtonCount();

        if (HandleInvalidIndex())
        {
            return;
        }

        currentIndex = 0;

        SetHighlightedIndex();
    }

    private void ClearIndex()
    {
        PetServices.PetLog.LogFatal("CLEAR INDEX");

        currentIndex = -1;

        SetHighlightedIndex();
    }

    private void AddIndex(InputState inputState)
    {
        ResetActiveButtonCount();

        if (HandleInvalidIndex())
        {
            return;
        }

        do
        {
            currentIndex++;

            if (currentIndex >= ButtonReferences.Length)
            {
                if (inputState == InputState.Held)
                {
                    RemoveIndex(InputState.Down);

                    requireDown = true;
                }
                else
                {
                    currentIndex = 0;
                }
            }
        }
        while (!ButtonReferences[currentIndex].IsVisible && !requireDown);

        SetHighlightedIndex();
    }

    private void RemoveIndex(InputState inputState)
    {
        ResetActiveButtonCount();

        if (HandleInvalidIndex())
        {
            return;
        }

        do
        {
            currentIndex--;

            if (currentIndex < 0)
            {
                if (inputState == InputState.Held)
                {
                    AddIndex(InputState.Down);

                    requireDown = true;
                }
                else
                {
                    currentIndex = activeButtonCount;

                    if (currentIndex >= ButtonReferences.Length)
                    {
                        currentIndex--;
                    }
                }
            }
        }
        while (!ButtonReferences[currentIndex].IsVisible && !requireDown);

        SetHighlightedIndex();
    }

    private void SetHighlightedIndex()
    {
        int buttonLength = ButtonReferences.Length;

        for (int i = 0; i < buttonLength; i++)
        {
            HighlightableLightStylizedButton button = ButtonReferences[i];

            if (i == currentIndex)
            {
                button.Focus();
            }
            else
            {
                button.Unfocus();
            }
        }
    }

    private void SetPosition<T>(QuickButton<T> button) where T : KTKAddon
    {
        button.X     = widthOffset;

        bool isVisible = button.IsVisible;

        bool isActive  = button.IsActive;

        if (isVisible != isActive)
        {
            PetServices.PetLog.LogVerbose("REQUEST REFRESH");

            requestRefresh = true;
        }

        if (!isActive)
        {
            return;
        }

        widthOffset -= button.Width * 0.90f;
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

        GuideRegistration.LeftOffsetX = (short)(-(Width - widthOffset - PetRenameQuickButton.Width));

        if (requestRefresh)
        {
            ResetIndex();

            requestRefresh = false;
        }
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
        ParentAddon.TransientGuideHandler?.DeregisterGuide(GuideRegistration);

        base.OnDispose();
    }
}
