using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.KTKWindowing.Helpers;
using PetRenamer.PetNicknames.KTKWindowing.Interfaces;
using PetRenamer.PetNicknames.KTKWindowing.Nodes.StyledNodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.FunctionalNodes;

internal class ColourBar : KTKComponent, ICustomInput, IRequireUpdate, IHasGuide, IHasSelectableCallbacks
{
    private Vector2 lastPosition = Vector2.NaN;
    private Vector2 lastSize     = Vector2.NaN;

    private readonly GradiantBarNode GradiantBar;

    public GuideRegistration GuideRegistration
        { get; }

    public bool RequestRefresh 
        { get; set; }

    private bool mouseDown = false;

    public ColourBar(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        GradiantBar = new GradiantBarNode(PetServices)
        { 
            Colour = Vector3.Zero 
        };

        AttachNode(ref GradiantBar);

        GuideRegistration = new GuideRegistration
        {
            LowerGuideId       = 4,

            LeftGuideId        = 0,
            LeftPoint          = OperationGuidePoint.TopLeft,
            LeftRelativePoint  = OperationGuidePoint.TopLeft,
            LeftOffsetX        = -210,
            LeftOffsetY        = 23,

            RightGuideId       = 1,
            RightPoint         = OperationGuidePoint.TopLeft,
            RightRelativePoint = OperationGuidePoint.TopLeft,
            RightOffsetX       = 15,
            RightOffsetY       = 23,

            CallbackComponent  = this,
        };

        CollisionNode.IsVisible = true;
        CollisionNode.NodeFlags &= ~NodeFlags.Focusable;
        CollisionNode.AddEvent(AtkEventType.MouseOver, () => OnSelected?.Invoke());
        CollisionNode.AddEvent(AtkEventType.MouseDown, MouseDown);
        CollisionNode.AddEvent(AtkEventType.MouseUp, MouseUp);
    }

    private void MouseDown()
    {
        if (mouseDown)
        {
            return;
        }

        PetServices.PetLog.LogVerbose("MOUSE DOWN");

        mouseDown = true;
    }

    private void MouseUp()
    {
        if (!mouseDown)
        {
            return;
        }

        mouseDown = false;

        PetServices.PetLog.LogVerbose("MOUSE UP");
    }

    public Action? OnSelected
    {
        get => GuideRegistration.OnSelected;
        set => GuideRegistration.OnSelected = value;
    }

    public Action? OnUnselected
    {
        get => GuideRegistration.OnUnselected;
        set => GuideRegistration.OnUnselected = value;
    }

    public required Vector3 Colour
    {
        get => GradiantBar.Colour;
        set => GradiantBar.Colour = value;
    }

    public void Select()
    {
        GradiantBar.Select();
    }

    public void Unselect()
    {
        GradiantBar.Unselect();
        MouseUp();
    }

    public bool OnCustomGuideInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {
        return false;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        GradiantBar.Size = Size;
    }

    public unsafe void OnAddonUpdate(AtkUnitBase* addon)
    {
        if (lastPosition == Position && lastSize == Size)
        {
            return;
        }

        lastPosition = Position;
        lastSize     = Size;

        short actualYPos = (short)(Position.Y - 12);

        GuideRegistration.LeftOffsetY  = actualYPos;
        GuideRegistration.RightOffsetY = actualYPos;

        GuideRegistration.LeftOffsetX  = (short)(Position.X - 30);
        GuideRegistration.RightOffsetX = (short)(Position.X + Size.X - 14);

        RequestRefresh = true;
    }

    protected override void OnDispose()
    {
        ParentAddon.TransientGuideHandler?.DeregisterGuide(GuideRegistration);

        base.OnDispose();
    }
}
