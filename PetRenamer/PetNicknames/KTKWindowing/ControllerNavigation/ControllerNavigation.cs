using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Interfaces;
using PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Struct;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation;

internal unsafe abstract class ControllerNavigation : IControllerNavigator
{
    private KTKAddon? _addonToControl;

    protected readonly IPetServices PetServices;

    public abstract bool OnCustomInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState);

    public ControllerNavigation(IPetServices petServices)
    {
        PetServices = petServices;
    }

    protected KTKAddon? AddonToControl
        => _addonToControl;

    protected AtkUnitBase* AtkUnitBase
    {
        get
        {
            if (AddonToControl == null)
            {
                return null;
            }
            
            return AddonToControl.GetUnitBase();
        }
    }

    protected NavigableComponent? GetFocusedComponent()
    {
        if (_addonToControl == null)
        {
            return null;
        }

        AtkStage* atkStage = AtkStage.Instance();

        if (atkStage == null)
        {
            return null;
        }

        AtkInputManager* atkInputManager = atkStage->AtkInputManager;

        if (atkInputManager == null) 
        {
            return null;
        }

        AtkResNode* focusedNode = atkInputManager->FocusedNode;

        if (focusedNode == null)
        {
            return null;
        }

        nint focusedNodeAddress = (nint)focusedNode;

        return GetComponent(focusedNodeAddress);
    }

    protected CollisionNode GetCollisionNode(NavigableComponent navigableComponent)
    {
        if (navigableComponent.StandinNode != null)
        {
            return navigableComponent.StandinNode;
        }

        return navigableComponent.CollisionNode;
    }

    protected NavigableComponent? GetComponent(nint address)
    {
        if (_addonToControl == null)
        {
            return null;
        }

        foreach (NavigableComponent navigableComponent in _addonToControl.NavigableComponents)
        {
            CollisionNode collisionNode = GetCollisionNode(navigableComponent);

            nint collisionNodeAddress = (nint)collisionNode.Node;

            if (collisionNodeAddress != address)
            {
                continue;
            }

            return navigableComponent;
        }

        return null;
    }

    protected CursorNavigationInfo GetNavigationInfo(NavigableComponent navigableComponent)
    {
        AtkCursorNavigationInfo atkCursorNavigationInfo = navigableComponent.ComponentBase->CursorNavigationInfo;

        return new CursorNavigationInfo
        (
            atkCursorNavigationInfo.Index,
            atkCursorNavigationInfo.UpIndex,
            atkCursorNavigationInfo.DownIndex,
            atkCursorNavigationInfo.LeftIndex,
            atkCursorNavigationInfo.RightIndex,
            navigableComponent.UpStopFlag,
            navigableComponent.DownStopFlag,
            navigableComponent.LeftStopFlag,
            navigableComponent.RightStopFlag
        ); 
    }

    protected CursorNavigationInfo? GetFocusedNavigationInfo()
    {
        NavigableComponent? focusedComponent = GetFocusedComponent();

        if (focusedComponent == null)
        {
            return null;
        }

        return GetNavigationInfo(focusedComponent);
    }

    protected bool IsNavigationBlocked(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState, CursorNavigationInfo cursorInfo)
    {
        if (inputState != AtkEventData.AtkInputData.InputState.Held)
        {
            return false;
        }
        switch (inputId)
        {
            case NavigationInputId.Left  when cursorInfo.LeftStop:  return true;
            case NavigationInputId.Right when cursorInfo.RightStop: return true;
            case NavigationInputId.Up    when cursorInfo.UpStop:    return true;
            case NavigationInputId.Down  when cursorInfo.DownStop:  return true;
        }

        return false;
    }

    protected byte GetNextIndex(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState, CursorNavigationInfo cursorInfo)
    {
        switch (inputId)
        {
            case NavigationInputId.Left:  return cursorInfo.LeftIndex;
            case NavigationInputId.Right: return cursorInfo.RightIndex;
            case NavigationInputId.Up:    return cursorInfo.UpIndex;
            case NavigationInputId.Down:  return cursorInfo.DownIndex;
        }

        return 0;
    }

    protected NavigableComponent? GetComponentAtIndex(byte index)
    {
        if (_addonToControl == null)
        {
            return null;
        }

        foreach (NavigableComponent navigableComponent in _addonToControl.NavigableComponents)
        {
            CursorNavigationInfo information = GetNavigationInfo(navigableComponent);

            if (information.Index != index)
            {
                continue;
            }

            return navigableComponent;
        }

        return null;
    }

    protected bool NextIndexIsValid(byte nextIndex)
    {
        return nextIndex > 0;
    }

    protected bool IsNavigableComponentValid(NavigableComponent navigableComponent)
    {
        return NextIndexIsValid(navigableComponent.NavigationIndex);
    }

    protected bool MoveFocus(NavigableComponent navigableComponent)
    {
        if (_addonToControl == null)
        {
            return false;
        }

        AtkStage* atkStage = AtkStage.Instance();

        if (atkStage == null)
        {
            return false;
        }

        AtkInputManager* atkInputManager = atkStage->AtkInputManager;

        if (atkInputManager == null)
        {
            return false;
        }

        CollisionNode collisionNode = GetCollisionNode(navigableComponent);

        return atkInputManager->SetFocus(&collisionNode.Node->AtkResNode, _addonToControl.GetUnitBase(), 1);
    }

    public void SetFocus(NavigableComponent ktkComponent)
    {
        if (AtkUnitBase == null)
        {
            return;
        }

        CollisionNode collisionNode = GetCollisionNode(ktkComponent);

        AtkUnitBase->FocusNode      = collisionNode;
        AtkUnitBase->CursorTarget   = collisionNode;
    }

    public void RegisterAddon(KTKAddon addonToControl)
    {
        _addonToControl = addonToControl;
    }

    public void UnregisterAddon()
    {
        _addonToControl = null;
    }
}
