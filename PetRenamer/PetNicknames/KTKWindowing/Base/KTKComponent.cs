using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Enums;

namespace PetRenamer.PetNicknames.KTKWindowing.Base;

internal abstract class KTKComponent : SimpleComponentNode
{
    protected readonly KTKAddon             ParentAddon;
    protected readonly DalamudServices      DalamudServices;
    protected readonly IPetServices         PetServices;
    protected readonly PettableDirtyHandler DirtyHandler;
    protected readonly KTKWindowHandler     WindowHandler;
    protected readonly NativeController     NativeController;

    protected virtual void OnDirty()   { }
    protected virtual void OnDispose() { }

    public KTKComponent(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler)
    {
        ParentAddon      = parentAddon;
        WindowHandler    = windowHandler;
        DalamudServices  = dalamudServices;
        PetServices      = petServices;
        DirtyHandler     = dirtyHandler;

        IsVisible                   = true;
        CollisionNode.IsVisible     = false;

        NativeController = petServices.NativeController;

        DirtyHandler.RegisterOnClearEntry(HandleDirtyEntry);
        DirtyHandler.RegisterOnDirtyEntry(HandleDirtyEntry);
        DirtyHandler.RegisterOnDirtyName(HandleDirtyDatabase);
        DirtyHandler.RegisterOnConfigurationDirty(HandleDirtyConfig);
        DirtyHandler.RegisterOnPetModeDirty(HandleDirtyPetmode);
        DirtyHandler.RegisterOnWindowDirty(HandleDirtyWindow);
    }

    protected PetWindowMode PetMode
       => KTKAddon.PetMode;

    protected sealed override void Dispose(bool disposing, bool isNativeDestructor)
    {
        DirtyHandler.UnregisterOnClearEntry(HandleDirtyEntry);
        DirtyHandler.UnregisterOnDirtyEntry(HandleDirtyEntry);
        DirtyHandler.UnregisterOnDirtyName(HandleDirtyDatabase);
        DirtyHandler.UnregisterOnConfigurationDirty(HandleDirtyConfig);
        DirtyHandler.UnregisterOnPetModeDirty(HandleDirtyPetmode);
        DirtyHandler.UnregisterOnWindowDirty(HandleDirtyWindow);

        OnDispose();

        base.Dispose(disposing, isNativeDestructor);
    }

    public virtual bool OnCustomInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
        => false;

    protected void AttachNode<T>(ref T node) where T : NodeBase
        => NativeController.AttachNode(node, this);

    private void HandleDirtyDatabase(INamesDatabase namesDatabase)
        => Dirty();

    private void HandleDirtyConfig(Configuration configuration)
        => Dirty();

    private void HandleDirtyEntry(IPettableDatabaseEntry entry)
        => Dirty();

    private void HandleDirtyPetmode(PetWindowMode petMode)
        => Dirty();

    private void HandleDirtyWindow()
        => Dirty();

    private void Dirty()
        => OnDirty();
}
