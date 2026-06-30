using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TooltipHook : HookableElement
{
    private static readonly string[] AllowedPreDrawAddons =
    [
        "AreaMap",
        "_NaviMap",
        "_ActionCross",
        "_ActionDoubleCrossL",
        "_ActionDoubleCrossR",
    ];
    
    private static readonly string[] AllowPreDrawAddonsStarts =
    [
        "_ActionBar"
    ];
    
    private readonly Hook<AtkTooltipManager.Delegates.ShowTooltip> ShowTooltipHook;
    
    private ushort lastParentId     = ushort.MaxValue;
    private bool   isAllowedPreDraw = false;
    
    private readonly HookableElement MapHook;
    private readonly IPronounHook    PronounHook;
    
    private SeString?  previousPronoun;
    private SeString?  currentPronoun;
    
    private SeString?  lastPreviousPronoun;
    private SeString?  lastCurrentPronoun;
    
    public TooltipHook(DalamudServices services, IPetServices petServices, HookableElement mapHook, IPronounHook pronounHook) 
        : base(services, petServices)
    {
        MapHook     = mapHook;
        PronounHook = pronounHook;
        
        ShowTooltipHook = DalamudServices.Hooking.HookFromAddress<AtkTooltipManager.Delegates.ShowTooltip>(AtkTooltipManager.Addresses.ShowTooltip.Value, AtkTooltipManagerShowTooltipDetour);
    }
    
    public override void Init()
    {
        ShowTooltipHook.Enable();
        
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "Tooltip",      OnTooltipRequestedUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreDraw,             "Tooltip",      OnTooltipPreDraw);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "ActionDetail", OnActionTooltipRequestedUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreRequestedUpdate,  "ActionDetail", OnActionTooltipRequestedUpdate);
    }
    
    private void SetText(AtkTextNode* textNode, AtkNineGridNode* backgroundNode)
    {
        if (PetServices.HoverService.CurrentlyHoveredPet == null)
        {
            return;
        }
        
        PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowOnTooltipColour, textNode, PetServices.HoverService.CurrentlyHoveredPet, PetServices.HoverService.CurrentNameType);
        
        if (backgroundNode == null)
        {
            return;
        }
        
        textNode->ResizeNodeForCurrentText();
        
        backgroundNode->AtkResNode.SetWidth((ushort)(textNode->Width + 18));
    }
    
    private void OnTooltipPreDraw(AddonEvent addonEvent, AddonArgs addonArgs)
    {
        if (!isAllowedPreDraw)
        {
            return;
        }
        
        OnTooltipRequestedUpdate(addonEvent, addonArgs);
    }
    
    private void OnTooltipRequestedUpdate(AddonEvent addonEvent, AddonArgs addonArgs)
    {
        AddonTooltip* addonTooltip = (AddonTooltip*)addonArgs.Addon.Address;
        
        if (addonTooltip == null)
        {
            return;
        }
        
        AtkTextNode* textNode = addonTooltip->GetTextNodeById(2);
        
        if (textNode == null)
        {
            return;
        }
        
        AtkNineGridNode* backgroundNode = (AtkNineGridNode*)addonTooltip->GetNodeById(3);
        
        SetText(textNode, backgroundNode);
    }
    
    private void HandlePronounChange(AddonEvent addonEvent)
    {
        if (previousPronoun == PronounHook.PreviousLastGottenPronoun && currentPronoun == PronounHook.LastGottenPronoun)
        {
            return;
        }
        
        if (addonEvent == AddonEvent.PreRequestedUpdate)
        {
            lastPreviousPronoun = PronounHook.LastGottenPronoun;
        }
        
        if (addonEvent == AddonEvent.PostRequestedUpdate)
        {
            previousPronoun     = PronounHook.PreviousLastGottenPronoun;
            currentPronoun      = PronounHook.LastGottenPronoun;
            lastCurrentPronoun  = PronounHook.LastGottenPronoun;
            
            if (lastPreviousPronoun != lastCurrentPronoun)
            {
                return;
            }
            
            if (lastCurrentPronoun == null || lastPreviousPronoun == null)
            {
                return;
            }
            
            IPetSheetData? petData =  PetServices.PetSheets.GetPetFromName(lastCurrentPronoun.TextValue);
            
            if (petData == null)
            {
                return;
            }
            
            PetServices.HoverService.SetCurrentNameType(NameType.Pronoun);
            PetServices.HoverService.SetHoveredPet(petData);
        }
    }
    
    private void OnActionTooltipRequestedUpdate(AddonEvent addonEvent, AddonArgs addonArgs)
    {
        HandlePronounChange(addonEvent);
        
        AtkUnitBase* addonActionTooltip =  (AtkUnitBase*)addonArgs.Addon.Address;
        
        if (addonActionTooltip == null)
        {
            return;
        }

        AtkTextNode* textNode = addonActionTooltip->GetTextNodeById(5);
        
        if (textNode == null)
        {
            return;
        }
        
        SetText(textNode, null);
    }
    
    private void HandlePreDraw(string addonName)
    {
        isAllowedPreDraw = AllowedPreDrawAddons.Contains(addonName);
        
        if (isAllowedPreDraw)
        {
            return;
        }
        
        for (int i = 0; i < AllowPreDrawAddonsStarts.Length; i++)
        {
            if (!addonName.Contains(AllowPreDrawAddonsStarts[i]))
            {
                continue;
            }
            
            isAllowedPreDraw = true;
            
            break;
        }
    }
    
    private void RelayShowTooltip(ushort parentId)
    {
        if (lastParentId == parentId)
        {
            return;
        }
        
        lastParentId = parentId;

        PetServices.HoverService.SetHoveredPet(null);
        
        MapHook.Refresh();
        
        AtkUnitBase* hoveredOverAddon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonById(parentId);
        
        if (hoveredOverAddon == null)
        {
            return;
        }
        
        if (!hoveredOverAddon->IsFullyLoaded())
        {
            return;
        }
        
        string addonName = hoveredOverAddon->NameString;
            
        HandlePreDraw(addonName);
        
        PetServices.PetLog.DevLogVerbose($"Showing tooltips: {addonName} {isAllowedPreDraw}");
    }
    
    private void AtkTooltipManagerShowTooltipDetour(AtkTooltipManager* thisPtr, AtkTooltipType type, ushort parentId, AtkResNode* targetNode, AtkTooltipManager.AtkTooltipArgs* tooltipArgs, delegate* unmanaged[Stdcall]<float*, float*, AtkResNode*, void> unkDelegate, bool unk7, bool unk8)
    {
        RelayShowTooltip(parentId);

        ShowTooltipHook!.Original(thisPtr, type, parentId, targetNode, tooltipArgs, unkDelegate, unk7, unk8);
    }
    
    protected override void OnDispose()
    { 
        ShowTooltipHook.Dispose();
        
        DalamudServices.AddonLifecycle.UnregisterListener(OnTooltipPreDraw);
        DalamudServices.AddonLifecycle.UnregisterListener(OnTooltipRequestedUpdate);
        DalamudServices.AddonLifecycle.UnregisterListener(OnActionTooltipRequestedUpdate);
    }
}