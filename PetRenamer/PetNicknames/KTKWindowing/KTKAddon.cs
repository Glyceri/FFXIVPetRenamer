using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.Hooking.Structs;
using PetRenamer.PetNicknames.KTKWindowing.Helpers;
using PetRenamer.PetNicknames.KTKWindowing.Nodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing;

internal abstract class KTKAddon : NativeAddon
{
    private const int PetBarOffset = 30;

    protected readonly KTKWindowHandler       WindowHandler;
    protected readonly IPetServices           PetServices;
    protected readonly DalamudServices        DalamudServices;
    protected readonly IPettableUserList      UserList;
    protected readonly PettableDirtyHandler   DirtyHandler;
    protected readonly IPettableDatabase      Database;

    protected abstract string  WindowInternalName { get; }
    public    abstract string  WindowTooltip      { get; }
    protected abstract Vector2 WindowSize         { get; }
    protected abstract bool    HasPetBar          { get; }

    protected virtual  string? WindowSubtitle     { get; }

    private   GuideRegistration?   TransientGuideRegistration;
    protected SimpleComponentNode? MainContainerNode { get; private set; }
    internal  static PetWindowMode PetMode           { get; private set; }

    protected PetBarNode? PetBarNode;

    private bool _isDirty = false;

    private nint? OwnAddress = null;

    private int   activeGuide = -1;

    private List<GuideRegistration> _guides = [];

    public KTKAddon(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, PettableDirtyHandler dirtyHandler)
    {
        WindowHandler    = windowHandler;
        DalamudServices  = dalamudServices;
        PetServices      = petServices;
        UserList         = userList;
        Database         = database;
        DirtyHandler     = dirtyHandler;

        NativeController = petServices.NativeController;

        InternalName     = WindowInternalName;
        Title            = "Pet Nicknames";

        Subtitle         = $"v{DalamudServices.PetNicknamesPlugin.Version}";

        Size             = WindowSize;

        if (HasPetBar)
        {
            Size += new Vector2(0, PetBarOffset);
        }
    }

    private unsafe void ClearGuide(byte index)
    {
        if (index >= 5)
        {
            return;
        }

        if (OwnAddress == null)
        {
            return;
        }

        AtkUnitBase* addon = (AtkUnitBase*)OwnAddress.Value;

        if (addon == null)
        {
            return;
        }

        addon->OperationGuides[index].Index            = 0;
        addon->OperationGuides[index].AddonTransientId = 0;
        addon->OperationGuides[index].OffsetX          = 0;
        addon->OperationGuides[index].OffsetY          = 0;
    }

    bool setPos = false;

    private void SetTransientIndex(int index)
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

        if (OwnAddress == null)
        {
            return;
        }

        if (activeGuide != -1 && activeGuide < guideCount)
        {
            SetLowerGuide(_guides[activeGuide]?.LowerGuideId ?? 2);
            _guides[activeGuide]?.OnSelected?.Invoke();
        }

        SetDirty();
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

    public void DeregisterTransient(GuideRegistration guideRegistration)
    {
        _ = _guides.Remove(guideRegistration);

        SetTransientIndex(0);
    }

    private unsafe void SetGuide(byte index, uint guideId, OperationGuidePoint point, OperationGuidePoint relativePoint, short offsetX, short offsetY)
    {
        if (index >= 5)
        {
            return;
        }

        ClearGuide(index);

        if (OwnAddress == null)
        {
            return;
        }

        AtkUnitBase* addon = (AtkUnitBase*)OwnAddress.Value;

        if (addon == null)
        {
            return;
        }

        addon->OperationGuides[index].Index            = index;
        addon->OperationGuides[index].Point            = point;
        addon->OperationGuides[index].RelativePoint    = relativePoint;
        addon->OperationGuides[index].AddonTransientId = PluginConstants.PET_NICKNAMES_TRANSIENT_OFFSET + guideId;
        addon->OperationGuides[index].OffsetX          = offsetX;
        addon->OperationGuides[index].OffsetY          = offsetY;
    }

    protected sealed override unsafe void OnSetup(AtkUnitBase* addon)
    {
        OwnAddress = (nint)addon;

        DirtyHandler.RegisterOnDirtyNavigation(OnInput);
        DirtyHandler.RegisterOnClearEntry(HandleDirtyEntry);
        DirtyHandler.RegisterOnDirtyEntry(HandleDirtyEntry);
        DirtyHandler.RegisterOnDirtyName(HandleDirtyDatabase);
        DirtyHandler.RegisterOnConfigurationDirty(HandleDirtyConfig);
        DirtyHandler.RegisterOnPetModeDirty(HandleDirtyPetmode);
        DirtyHandler.RegisterOnWindowDirty(HandleDirtyWindow);

        Vector2 contentRegionStartPos = ContentStartPosition;
        Vector2 contentRegionSize     = ContentSize;

        WindowNode.TitleNode.SetEventFlags    = true;
        WindowNode.SubtitleNode.SetEventFlags = true;

        WindowNode.TitleNode.Tooltip    = WindowTooltip;
        WindowNode.SubtitleNode.Tooltip = $"The current Pet Nicknames Version is: " + Subtitle.TextValue;

        if (HasPetBar)
        {
            PetBarNode = new PetBarNode(WindowHandler, DalamudServices, PetServices, DirtyHandler, this)
            {
                Position  = ContentStartPosition,
                Size      = new Vector2(ContentSize.X, PetBarOffset),
                IsVisible = true,
            };

            contentRegionStartPos += new Vector2(0, PetBarOffset);
            contentRegionSize     -= new Vector2(0, PetBarOffset);

            TransientGuideRegistration = new GuideRegistration
            {
                LeftGuideId        = 0,
                LeftPoint          = OperationGuidePoint.TopLeft,
                LeftRelativePoint  = OperationGuidePoint.TopLeft,
                LeftOffsetX        = -15,
                LeftOffsetY        = 23,

                RightGuideId       = 1,
                RightPoint         = OperationGuidePoint.TopLeft,
                RightRelativePoint = OperationGuidePoint.TopLeft,
                RightOffsetX       = 210,
                RightOffsetY       = 23,

                CallbackComponent  = PetBarNode,
            };

            RegisterGuide(TransientGuideRegistration);

            AttachNode(PetBarNode);
        }

        MainContainerNode = new SimpleComponentNode
        {
            Position  = contentRegionStartPos,
            Size      = contentRegionSize,
            IsVisible = true,
        };

        AttachNode(MainContainerNode);

        OnAddonSetup(addon);
    }

    private void SetLowerGuide(byte index)
    {
        SetGuide(0, index, OperationGuidePoint.Center, OperationGuidePoint.Bottom, 0, 0);

        RefreshOperationsGuide();
    }

    protected virtual unsafe void OnAddonSetup(AtkUnitBase* addon) { }
    protected virtual unsafe void OnAddonUpdate(AtkUnitBase* addon) { }
    protected virtual unsafe void OnAddonFinalize(AtkUnitBase* addon) { }
    protected virtual unsafe void OnDirty() { }

    protected void AttachNode<T>(ref T nodeBase) where T : NodeBase
        => NativeController.AttachNode(nodeBase, MainContainerNode!);

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

    private bool OnInput(nint atkUnitBase, NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {
        if (OwnAddress == null)
        {
            return false;
        }

        if (atkUnitBase != OwnAddress.Value)
        {
            return false;
        }

        PetServices.PetLog.LogWarning($"Found the corresponding Addon [{InternalName}] and is handling the input: {inputId}, {inputState}");

        if (HandleLocalInput(inputId, inputState))
        {
            return true;
        }

        bool passed = false;

        if (HasPetBar)
        {
            if (activeGuide != -1)
            {
                GuideRegistration registration = _guides[activeGuide];

                passed = registration.CallbackComponent.OnCustomInput(inputId, inputState);
            }
        }

        if (passed)
        {
            return true;
        }

        return OnCustomInput(inputId, inputState);
    }

    protected virtual bool OnCustomInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
        => false;

    private void HandleDirtyDatabase(INamesDatabase namesDatabase)
        => SetDirty();

    private void HandleDirtyConfig(Configuration configuration)
        => SetDirty();

    private void HandleDirtyEntry(IPettableDatabaseEntry entry)
        => SetDirty();

    private void HandleDirtyPetmode(PetWindowMode petMode)
    {
        PetMode = petMode;

        SetDirty();
    }

    private void HandleDirtyWindow()
        => SetDirty();

    private unsafe void RefreshOperationsGuide()
    {
        PetNicknamesOperationsGuide* guide = (PetNicknamesOperationsGuide*)&AtkStage.Instance()->OperationGuide;

        if ((nint)guide->UnkUnitBase1 == OwnAddress)
        {
            guide->Unk19 = 1;
        }
    }

    protected void SetDirty()
        => _isDirty = true;
    
    protected sealed override unsafe void OnFinalize(AtkUnitBase* addon)
    {
        OwnAddress = null;

        if (TransientGuideRegistration != null)
        {
            DeregisterTransient(TransientGuideRegistration);

            TransientGuideRegistration = null;
        }

        DirtyHandler.UnregisterOnDirtyNavigation(OnInput);
        DirtyHandler.UnregisterOnClearEntry(HandleDirtyEntry);
        DirtyHandler.UnregisterOnDirtyEntry(HandleDirtyEntry);
        DirtyHandler.UnregisterOnDirtyName(HandleDirtyDatabase);
        DirtyHandler.UnregisterOnConfigurationDirty(HandleDirtyConfig);
        DirtyHandler.UnregisterOnPetModeDirty(HandleDirtyPetmode);
        DirtyHandler.UnregisterOnWindowDirty(HandleDirtyWindow);

        MainContainerNode = null;

        OnAddonFinalize(addon);

        DirtyHandler.DirtyWindow();
    }

    protected sealed override unsafe void OnUpdate(AtkUnitBase* addon)
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

        if (_isDirty)
        {
            _isDirty = false;

            try
            {
                RefreshOperationsGuide();
                OnDirty();
            }
            catch(Exception e)
            {
                PetServices.PetLog.LogFatal(e);
            }
        }

        OnAddonUpdate(addon);
    }
}
