using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Implementations;
using PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Interfaces;
using PetRenamer.PetNicknames.KTKWindowing.Nodes;
using PetRenamer.PetNicknames.KTKWindowing.TransientGuide;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Base;

internal abstract class KTKAddon : NativeAddon
{
    private const int PetBarOffset = 30;

    protected readonly KTKWindowHandler       WindowHandler;
    protected readonly IPetServices           PetServices;
    protected readonly DalamudServices        DalamudServices;
    protected readonly IPettableUserList      UserList;
    protected readonly PettableDirtyHandler   DirtyHandler;
    protected readonly IPettableDatabase      Database;

    public abstract string                    WindowTooltip 
        { get; }

    protected abstract string                 WindowInternalName  
        { get; }

    protected abstract Vector2                WindowSize          
        { get; }

    protected abstract bool                   HasPetBar
        { get; }

    protected IControllerNavigator            ControllerNavigator
        { get; init; }

    protected virtual string? WindowSubtitle     
        { get; }

    protected SimpleComponentNode? MainContainerNode { get; private set; }
    internal  static PetWindowMode PetMode           { get; private set; }

    protected PetBarNode? PetBarNode;

    private bool _isDirty = false;

    private AtkUnitBasePtr Self;

    public TransientGuideHandler? TransientGuideHandler;

    private readonly List<NavigableComponent> _navigableComponents = [];

    [SetsRequiredMembers]
    public KTKAddon(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, PettableDirtyHandler dirtyHandler)
    {
        WindowHandler       = windowHandler;
        DalamudServices     = dalamudServices;
        PetServices         = petServices;
        UserList            = userList;
        Database            = database;
        DirtyHandler        = dirtyHandler;

        NativeController    = petServices.NativeController;

        InternalName        = WindowInternalName;
        Title               = "Pet Nicknames";

        Subtitle            = $"v{DalamudServices.PetNicknamesPlugin.Version}";

        Size                = WindowSize;

        if (HasPetBar)
        {
            Size += new Vector2(0, PetBarOffset);
        }

        ControllerNavigator = new DullImplementation(PetServices);
    }

    protected sealed override unsafe void OnSetup(AtkUnitBase* addon)
    {
        Self                  = new AtkUnitBasePtr((nint)addon);
        TransientGuideHandler = new TransientGuideHandler(Self, PetServices);

        ControllerNavigator.RegisterAddon(this);

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
            PetBarNode = new PetBarNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
            {
                Position  = ContentStartPosition,
                Size      = new Vector2(ContentSize.X, PetBarOffset),
                IsVisible = true,
            };

            contentRegionStartPos += new Vector2(0, PetBarOffset);
            contentRegionSize     -= new Vector2(0, PetBarOffset);

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

    protected virtual unsafe void OnAddonSetup(AtkUnitBase* addon) { }
    protected virtual unsafe void OnAddonUpdate(AtkUnitBase* addon) { }
    protected virtual unsafe void OnAddonFinalize(AtkUnitBase* addon) { }
    protected virtual unsafe void OnDirty() { }

    protected void AttachNode<T>(ref T nodeBase) where T : NodeBase
        => NativeController.AttachNode(nodeBase, MainContainerNode!);

    public unsafe AtkUnitBase* GetUnitBase()
    {
        if (Self == null)
        {
            return null;
        }

        return (AtkUnitBase*)Self.Address;
    }

    private bool OnInput(nint atkUnitBase, NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {
        if (Self == null)
        {
            return false;
        }

        if (atkUnitBase != Self.Address)
        {
            return false;
        }

        if (TransientGuideHandler?.OnInput(inputId, inputState) ?? false)
        {
            return true;
        }

        if (OnCustomInput(inputId, inputState))
        {
            return true;
        }

        return ControllerNavigator.OnCustomInput(inputId, inputState);
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
        Self                  = null;
        TransientGuideHandler = null;

        ControllerNavigator.UnregisterAddon();

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

    public NavigableComponent[] NavigableComponents
        => [.._navigableComponents];

    public void RegisterNavigableComponent(NavigableComponent navigableComponent)
    {
        _ = _navigableComponents.Remove(navigableComponent);

        _navigableComponents.Add(navigableComponent);
    }

    public void DeregisterNavigableComponent(NavigableComponent navigableComponent)
    {
        _ = _navigableComponents.Remove(navigableComponent);
    }

    protected sealed override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        TransientGuideHandler?.Update();

        if (_isDirty)
        {
            _isDirty = false;

            try
            {
                TransientGuideHandler?.RefreshOperationsGuide();
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
