using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.Hooking.Structs;
using PetRenamer.PetNicknames.KTKWindowing.Helpers;
using PetRenamer.PetNicknames.Services.Interface;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.KTKWindowing.TransientGuide;

internal class TransientGuideHandler
{
    private readonly IPetServices            PetServices;
    private readonly AtkUnitBasePtr          Addon;

    private readonly List<GuideRegistration> _guides = [];

    private int activeGuide = -1;

    public TransientGuideHandler(AtkUnitBasePtr addon, IPetServices petServices)
    {  
        Addon       = addon;
        PetServices = petServices;
    }

    public unsafe void RefreshOperationsGuide()
    {
        PetNicknamesOperationsGuide* guide = (PetNicknamesOperationsGuide*)&AtkStage.Instance()->OperationGuide;

        if (guide == null)
        {
            return;
        }    

        if ((nint)guide->UnkUnitBase1 == Addon.Address)
        {
            guide->Unk19 = 1;
        }
    }

    public void RegisterGuide(GuideRegistration guideRegistration)
    {
        if (_guides.Contains(guideRegistration))
        {
            return;
        }

        _guides.Add(guideRegistration);

        SetTransientIndex(0);
    }

    public void DeregisterGuide(GuideRegistration guideRegistration)
    {
        _ = _guides.Remove(guideRegistration);

        SetTransientIndex(0);
    }

    public void Update()
    {
        if (activeGuide == -1)
        {
            ClearGuide(1);
            ClearGuide(2);
        }
        else
        {
            GuideRegistration registration = _guides[activeGuide];

            SetGuide(1, registration.LeftGuideId,  registration.LeftPoint,  registration.LeftRelativePoint,  registration.LeftOffsetX,  registration.LeftOffsetY);
            SetGuide(2, registration.RightGuideId, registration.RightPoint, registration.RightRelativePoint, registration.RightOffsetX, registration.RightOffsetY);
        }
    }

    private bool HandleLocalInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {
        if (inputId != NavigationInputId.NintendoX)
        {
            return false;
        }

        if (inputState != AtkEventData.AtkInputData.InputState.Down)
        {
            return false;
        }

        SetTransientIndex(activeGuide + 1);

        PetServices.PetLog.LogWarning("Transients: " + _guides.Count + ", " + activeGuide);

        return true;
    }

    private bool HandleActiveInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {
        if (activeGuide <= -1)
        {
            return false;
        }

        GuideRegistration registration = _guides[activeGuide];

        return registration.CallbackComponent.OnCustomInput(inputId, inputState);
    }

    public bool OnInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {
        if (HandleLocalInput(inputId, inputState))
        {
            return true;
        }

        if (HandleActiveInput(inputId, inputState))
        {
            return true;
        }

        return false;
    }

    public unsafe void ClearGuide(byte index)
    {
        if (index >= 5)
        {
            return;
        }

        AtkUnitBase* addon = (AtkUnitBase*)Addon.Address;

        if (addon == null)
        {
            return;
        }

        addon->OperationGuides[index].Index            = 0;
        addon->OperationGuides[index].AddonTransientId = 0;
        addon->OperationGuides[index].OffsetX          = 0;
        addon->OperationGuides[index].OffsetY          = 0;
    }

    public void SetTransientIndex(int index)
    {
        int guideCount = _guides.Count;

        if (activeGuide == index)
        {
            return;
        }

        if (activeGuide >= 0 && activeGuide < guideCount)
        {
            _guides[activeGuide]?.OnUnselected?.Invoke();
        }

        activeGuide = index;

        if (activeGuide >= guideCount)
        {
            activeGuide = 0;
        }

        if (guideCount == 0)
        {
            activeGuide = -1;
        }

        if (activeGuide != -1 && activeGuide < guideCount)
        {
            SetLowerGuide(_guides[activeGuide]?.LowerGuideId ?? 2);
            _guides[activeGuide]?.OnSelected?.Invoke();

            RefreshOperationsGuide();
        }
    }

    public void SetLowerGuide(byte index)
    {
        SetGuide(0, index, OperationGuidePoint.Center, OperationGuidePoint.Bottom, 0, 0);
    }

    public unsafe void SetGuide(byte index, uint guideId, OperationGuidePoint point, OperationGuidePoint relativePoint, short offsetX, short offsetY)
    {
        if (index >= 5)
        {
            return;
        }

        ClearGuide(index);

        AtkUnitBase* addon = (AtkUnitBase*)Addon.Address;

        if (addon == null)
        {
            return;
        }

        addon->OperationGuides[index].Index             = index;
        addon->OperationGuides[index].Point             = point;
        addon->OperationGuides[index].RelativePoint     = relativePoint;
        addon->OperationGuides[index].AddonTransientId  = PluginConstants.PET_NICKNAMES_TRANSIENT_OFFSET + guideId;
        addon->OperationGuides[index].OffsetX           = offsetX;
        addon->OperationGuides[index].OffsetY           = offsetY;

        RefreshOperationsGuide();
    }
}
