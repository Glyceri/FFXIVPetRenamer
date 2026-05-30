using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TargetHook : HookableElement
{
    public TargetHook(DalamudServices services, IPetServices petServices)
        : base(services, petServices) { }
    
    public override void Init()
    {
        PetServices.TargetManager.RegisterTargetChangedListener(OnTargetChanged);
        
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_TargetInfo",           OnTargetInfo);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_TargetInfoMainTarget", OnTargetInfoMainTarget);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_FocusTargetInfo",      OnFocusTargetInfo);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_CastBar",              OnCastBar);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_TargetInfoCastBar",    OnTargetInfoCastBar);
    }
    
    protected override void OnDispose()
    {
        PetServices.TargetManager.DeregisterTargetChangedListener(OnTargetChanged);
        
        DalamudServices.AddonLifecycle.UnregisterListener(OnTargetInfo);
        DalamudServices.AddonLifecycle.UnregisterListener(OnTargetInfoMainTarget);
        DalamudServices.AddonLifecycle.UnregisterListener(OnFocusTargetInfo);
        DalamudServices.AddonLifecycle.UnregisterListener(OnCastBar);
        DalamudServices.AddonLifecycle.UnregisterListener(OnTargetInfoCastBar);
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
    
    private void HandleTargetCastBar(AddonArgs args, uint textNodeIndex, IPettableEntity? target)
    {
        AtkUnitBase* atkUnitBase = GetAtkUnitBase(args);
        
        if (atkUnitBase == null)
        {
            return;
        }
        
        if (target is not IPettableUser user)
        {
            return;
        }
        
        AtkTextNode* textNode = GetAtkTextNode(atkUnitBase, textNodeIndex);
        
        if (textNode == null)
        {
            return;
        }
        
        if (!textNode->IsVisible())
        {
            return;
        }
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromAction(user.CurrentCastId);
        
        if (petData == null)
        {
            return;
        }
        
        petData = PetServices.PetSheets.MakeSoft(user, petData);
        
        PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowOnCastbarsColour, textNode, petData, NameType.Action, user);
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
        
        if (!textNode->IsVisible())
        {
            return;
        }
        
        if (target is not IPettablePet pettablePet)
        {
            return;
        }
        
        PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowOnTargetBarsColour, textNode, pettablePet, NameType.Raw);
    }
    
    private void OnTargetInfo(AddonEvent addonEvent, AddonArgs args)
    {
        HandleTarget       (args, 16, Target());
        HandleTarget       (args, 7,  TargetOfTarget());
        HandleTargetCastBar(args, 12, Target());
    }
    
    private void OnTargetInfoMainTarget(AddonEvent addonEvent, AddonArgs args)
    {
        HandleTarget(args, 10, Target());
        HandleTarget(args, 7,  TargetOfTarget());
    }
    
    private void OnFocusTargetInfo(AddonEvent addonEvent, AddonArgs args)
    {
        HandleTarget       (args, 10, FocusTarget());
        HandleTargetCastBar(args, 5, FocusTarget());
    }
    
    private void OnCastBar(AddonEvent addonEvent, AddonArgs args)
    {
        HandleTargetCastBar(args, 4, PetServices.UserList.LocalPlayer);
    }
    
    private void OnTargetInfoCastBar(AddonEvent addonEvent, AddonArgs args)
    {
        HandleTargetCastBar(args, 4, PetServices.UserList.LocalPlayer);
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
            PetServices.PetLog.LogVerbose($"{callSource} just got the target: {pet.Address}.");
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
        
        // TODO: Make target text change c:
    }
}