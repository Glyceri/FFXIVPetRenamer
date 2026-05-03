using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TargetHook : HookableElement
{
    public TargetHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener)
        : base(services, userList, petServices, dirtyListener) { }
    
    public override void Init()
    {
        PetServices.TargetManager.RegisterTargetChangedListener(OnTargetChanged);
        
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_TargetInfo",           OnTargetInfo);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_TargetInfoMainTarget", OnTargetInfoMainTarget);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_FocusTargetInfo",      OnFocusTargetInfo);
    }
    
    protected override void OnDispose()
    {
        PetServices.TargetManager.DeregisterTargetChangedListener(OnTargetChanged);
        
        DalamudServices.AddonLifecycle.UnregisterListener(OnTargetInfo);
        DalamudServices.AddonLifecycle.UnregisterListener(OnTargetInfoMainTarget);
        DalamudServices.AddonLifecycle.UnregisterListener(OnFocusTargetInfo);
    }
    
    private void HandleTarget(AddonArgs args, uint textNodeIndex, IPettableEntity? target)
    {
        AtkUnitBase* atkUnitBase = GetAtkUnitBase(args);
        
        if (atkUnitBase == null)
        {
            return;
        }
        
        HandleTargetTextNode(GetAtkTextNode(atkUnitBase, textNodeIndex), target);
    }
    
    private AtkUnitBase* GetAtkUnitBase(AddonArgs args)
        => (AtkUnitBase*)args.Addon.Address;
    
    private AtkTextNode* GetAtkTextNode(AtkUnitBase* unitBase, uint textNodeIndex)
        => unitBase->GetTextNodeById(textNodeIndex);
    
    private void HandleTargetTextNode(AtkTextNode* textNode, IPettableEntity? target)
    {
        if (textNode == null)
        {
            return;
        }
        
        if (target is not IPettablePet pettablePet)
        {
            return;
        }
        
        if (!PetServices.Configuration.showOnTargetBars)
        {
            return;
        }
        
        if (pettablePet.PetData == null)
        {
            return;
        }
        
        string? baseName = PetServices.NameService.GetName(NameType.Raw, pettablePet.PetData);
        
        if (baseName.IsNullOrWhitespace())
        {
            return;
        }
        
        string? customName = pettablePet.CustomName;
        
        if (customName.IsNullOrWhitespace())
        {
            return;
        }
        
        pettablePet.GetDrawColours(out Vector3? edgeColour, out Vector3? textColour);
        
        PetServices.StringHelper.ReplaceATKString(textNode, baseName, customName, edgeColour, textColour);
    }
    
    private void OnTargetInfo(AddonEvent addonEvent, AddonArgs args)
    {
        HandleTarget(args, 17, Target());
        HandleTarget(args, 7,  TargetOfTarget());
    }
    
    private void OnTargetInfoMainTarget(AddonEvent addonEvent, AddonArgs args)
    {
        HandleTarget(args, 10, Target());
        HandleTarget(args, 7,  TargetOfTarget());
    }
    
    private void OnFocusTargetInfo(AddonEvent addonEvent, AddonArgs args)
    {
        HandleTarget(args, 10, FocusTarget());
    }
    
    [Conditional("DEBUG")]
    private void LogTarget(IPettableEntity? entity, [CallerMemberName] string callSource = "")
    {
        if (entity == null)
        {
            PetServices.PetLog.LogVerbose($"{callSource} just got the target: NULL.");

            return;
        }

        if (entity is IPettablePet pet)
        {
            PetServices.PetLog.LogVerbose($"{callSource} just got the target: {pet.Name}.");
        }
        else if (entity is IPettableUser user)
        {
            PetServices.PetLog.LogVerbose($"{callSource} just got the target: {user.Name}.");
        }
    }

    private IPettableEntity? FocusTarget()
    {
        IPettableEntity? returner = PetServices.TargetManager.FocusTarget;

        LogTarget(returner);

        return returner;
    }

    private IPettableEntity? TargetOfTarget()
    {
        IPettableEntity? returner = PetServices.TargetManager.TargetOfLeadingTarget;

        LogTarget(returner);

        return returner;
    }

    private IPettableEntity? Target()
    {
        IPettableEntity? returner = PetServices.TargetManager.LeadingTarget;

        LogTarget(returner);

        return returner;
    }
    
    private void OnTargetChanged()
    {
        PetServices.PetLog.LogVerbose("Received target status changed.");
    }
}