using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using System;
using System.Collections.Generic;
using ITargetManager = PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces.ITargetManager;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class TargetManagerWrapper : ITargetManager
{
    public bool Enabled { get; set; } = true;

    private readonly DalamudServices   DalamudServices;
    private readonly IPettableUserList UserList;

    private readonly List<Action>      _callbacks = [];

    private nint? LastTarget;
    private nint? LastSoftTarget;
    private nint? LastFocusTarget;

    public TargetManagerWrapper(DalamudServices dalamudServices, IPettableUserList userList)
    {
        DalamudServices = dalamudServices;
        UserList        = userList;
    }

    private IPettableUserTargetManager? TargetManager
        => UserList.LocalPlayer?.TargetManager;

    public IPettableEntity? SoftTarget
        => TargetManager?.GetSoftTarget();

    public IPettableEntity? Target                   
        => TargetManager?.GetTarget();

    public IPettableEntity? LeadingTarget
        => TargetManager?.GetLeadingTarget();

    public IPettableEntity? TargetOfTarget
        => TargetManager?.GetTargetOfTarget();

    public IPettableEntity? LeadingTargetOfTarget
        => TargetManager?.GetTargetOfLeadingTarget();

    public IPettableEntity? LeadingTargetOfLeadingTarget
        => TargetManager?.GetLeadingTargetOfLeadingTarget();

    public IPettableEntity? TargetOfLeadingTarget
        => TargetManager?.GetTargetOfLeadingTarget();

    public IPettableEntity? FocusTarget              
        => GetEntity(DalamudServices.TargetManager.FocusTarget);

    public IPettableEntity? MouseOverTarget          
        => GetEntity(DalamudServices.TargetManager.MouseOverTarget);

    public IPettableEntity? PreviousTarget           
        => GetEntity(DalamudServices.TargetManager.PreviousTarget);

    public IPettableEntity? GPoseTarget              
        => GetEntity(DalamudServices.TargetManager.GPoseTarget);

    public IPettableEntity? MouseOverNameplateTarget 
        => GetEntity(DalamudServices.TargetManager.MouseOverNameplateTarget);

    private IPettableEntity? GetEntity(IGameObject? target)
    {
        if (target == null)
        {
            return null;
        }

        return GetEntity(target.Address);
    }

    private IPettableEntity? GetEntity(nint address)
    {
        IPettableEntity? entity = UserList.GetUser(address, false);

        if (entity != null)
        {
            return entity;
        }

        return UserList.GetPet(address);
    }

    private bool TargetChanged(ref nint? lastTarget, IGameObject? newTarget)
    {
        nint? newTargetId = newTarget?.Address;

        if (lastTarget == newTargetId)
        {
            return false;
        }

        lastTarget = newTargetId;

        return true;
    }

    public void OnUpdate(IFramework framework)
    {
        bool changed = false;

        changed |= TargetChanged(ref LastTarget,      DalamudServices.TargetManager.Target);
        changed |= TargetChanged(ref LastSoftTarget,  DalamudServices.TargetManager.SoftTarget);
        changed |= TargetChanged(ref LastFocusTarget, DalamudServices.TargetManager.FocusTarget);

        if (!changed)
        {
            return;
        }

        OnTargetStatusChanged();
    }

    private void OnTargetStatusChanged()
    {
        DalamudServices.PluginLog.Verbose("Target status changed. Callbacks will be invoked");

        foreach (Action callback in _callbacks)
        {
            callback?.Invoke();
        }
    }

    public void RegisterTargetChangedListener(Action callback)
    {
        _ = _callbacks.Remove(callback);

        _callbacks.Add(callback);
    }

    public void DeregisterTargetChangedListener(Action callback)
    {
        _ = _callbacks.Remove(callback);
    }
}
