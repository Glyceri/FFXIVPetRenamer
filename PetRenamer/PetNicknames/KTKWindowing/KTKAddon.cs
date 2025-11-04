using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using PetRenamer.PetNicknames.Hooking.Enum;
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

    private   TransientRegistration? TransientRegistration;
    protected SimpleComponentNode? MainContainerNode { get; private set; }
    internal  static PetWindowMode PetMode           { get; private set; }

    protected PetBarNode? PetBarNode;

    private bool _isDirty = false;

    private nint? OwnAddress = null;

    private int   activeTransient = -1;

    private List<TransientRegistration> _transients = [];

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

    private unsafe void ClearTransient(byte index)
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
        activeTransient = index;

        int transientCount = _transients.Count;

        if (activeTransient >= transientCount)
        {
            activeTransient = 0;
        }

        if (transientCount == 0)
        {
            activeTransient = -1;
        }

        if (OwnAddress == null)
        {
            return;
        }

        unsafe
        {
            AtkUnitBase* unitBase = (AtkUnitBase*)DalamudServices.GameGui.GetAddonByName("OperationGuide").Address;

            if (unitBase == null)
            {
                return;
            }

            AtkUnitBase* unitBase2 = (AtkUnitBase*)OwnAddress.Value;

            if (unitBase2 == null)
            {
                return;
            }

            unitBase2->SetPosition((short)(unitBase2->X + 1), unitBase2->Y);
            setPos = true;

            unitBase->RootNode->DrawFlags = unitBase->RootNode->DrawFlags | 1;
            unitBase2->RootNode->DrawFlags = unitBase2->RootNode->DrawFlags | 1;
        }
    }

    public void RegisterTransient(TransientRegistration transientRegistration)
    {
        if (_transients.Contains(transientRegistration))
        {
            return;
        }

        _transients.Add(transientRegistration);

        SetTransientIndex(0);
    }

    public void DeregisterTransient(TransientRegistration transientRegistration)
    {
        _ = _transients.Remove(transientRegistration);

        SetTransientIndex(0);
    }

    private unsafe void SetTransient(byte index, uint transientId, OperationGuidePoint point, OperationGuidePoint relativePoint, short offsetX, short offsetY)
    {
        if (index >= 5)
        {
            return;
        }

        ClearTransient(index);

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
        addon->OperationGuides[index].AddonTransientId = PluginConstants.PET_NICKNAMES_TRANSIENT_OFFSET + transientId;
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

            TransientRegistration = new TransientRegistration
            {
                LeftTransientId    = 0,
                LeftPoint          = OperationGuidePoint.TopLeft,
                LeftRelativePoint  = OperationGuidePoint.TopLeft,
                LeftOffsetX        = -15,
                LeftOffsetY        = 23,

                RightTransientId   = 1,
                RightPoint         = OperationGuidePoint.TopLeft,
                RightRelativePoint = OperationGuidePoint.TopLeft,
                RightOffsetX       = 210,
                RightOffsetY       = 23,

                CallbackComponent  = PetBarNode,
            };

            RegisterTransient(TransientRegistration);

            SetTransient(0, 2, OperationGuidePoint.Center, OperationGuidePoint.Bottom, 0, 0);

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

    private Vector2 GetStartPosition()
    {
        Vector2 startPos = ContentStartPosition;

        if (HasPetBar)
        {
            startPos += new Vector2(0, PetBarOffset);
        }

        return startPos;
    }

    protected Vector2 StartPosition
        => GetStartPosition();

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

        SetTransientIndex(activeTransient + 1);

        PetServices.PetLog.LogWarning("Transients: " + _transients.Count + ", " + activeTransient);

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
            if (activeTransient != -1)
            {
                TransientRegistration registration = _transients[activeTransient];

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

    protected void SetDirty()
        => _isDirty = true;

    protected sealed override unsafe void OnFinalize(AtkUnitBase* addon)
    {
        OwnAddress = null;

        if (TransientRegistration != null)
        {
            DeregisterTransient(TransientRegistration);

            TransientRegistration = null;
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

    protected override unsafe void OnDraw(AtkUnitBase* addon)
    {
        base.OnDraw(addon);

        if (setPos)
        {
            setPos = false;

            if (OwnAddress != null)
            {
                AtkUnitBase* unitBase2 = (AtkUnitBase*)OwnAddress.Value;

                if (unitBase2 == null)
                {
                    return;
                }

                unitBase2->SetPosition((short)(unitBase2->X - 1), unitBase2->Y);
            }
        }
    }

    protected sealed override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        if (activeTransient == -1)
        {
            ClearTransient(1);
            ClearTransient(2);
        }
        else
        {
            TransientRegistration registration = _transients[activeTransient];

            SetTransient(1, registration.LeftTransientId,  registration.LeftPoint,  registration.LeftRelativePoint,  registration.LeftOffsetX,  registration.LeftOffsetY);
            SetTransient(2, registration.RightTransientId, registration.RightPoint, registration.RightRelativePoint, registration.RightOffsetX, registration.RightOffsetY);
        }

        if (_isDirty)
        {
            _isDirty = false;

            try
            {
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
