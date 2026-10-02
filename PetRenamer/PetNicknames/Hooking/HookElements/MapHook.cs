using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using InteropGenerator.Runtime;
using PetRenamer.PetNicknames.Hooking.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class MapHook : HookableElement
{
    private readonly Hook<AgentMap.Delegates.CreateTooltip> ContextTooltipHandleHook;
    private readonly Hook<BattleChara.Delegates.GetName>    GetNameHook;
    
    private IPettablePet? _selectedPet              = null;
    private bool          _passContextTooltipHandle = false;
    private bool          _passGetName              = false;
    private bool          _handledGetName           = false;
    
    public MapHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
    {
        ContextTooltipHandleHook = DalamudServices.Hooking.HookFromAddress<AgentMap.Delegates.CreateTooltip>((nint)AgentMap.MemberFunctionPointers.CreateTooltip, ContextTooltipHandleDetour);
        GetNameHook              = DalamudServices.Hooking.HookFromAddress<BattleChara.Delegates.GetName>((nint)BattleChara.StaticVirtualTablePointer->GetName, GetNameDetour);
    }

    public override void Init()
    { 
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate,  "AreaMap",  MapUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "AreaMap",  MapUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate,  "_NaviMap", MapUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "AreaMap",  MapUpdate);
        
        ContextTooltipHandleHook.Enable();
        GetNameHook.Enable();
    }
    
    protected override void OnDispose()
    {
        GetNameHook.Dispose();
        ContextTooltipHandleHook.Dispose();
        
        DalamudServices.AddonLifecycle.UnregisterListener(MapUpdate);
    }
    
    private CStringPointer GetNameDetour(BattleChara* gameObject)
    {
        if (!_passGetName)
        {
            return GetNameHook.OriginalDisposeSafe(gameObject);
        }
        
        _selectedPet   = PetServices.UserList.GetPet((nint)gameObject);
        _handledGetName = true;
        
        return GetNameHook.OriginalDisposeSafe(gameObject);
    }
    
    private bool ContextTooltipHandleDetour(AgentMap* agentMap, Utf8String* tooltipString, uint tooltipContext)
    {
        if (!_passContextTooltipHandle)
        {
            return ContextTooltipHandleHook.OriginalDisposeSafe(agentMap, tooltipString, tooltipContext);
        }
        
        MapTooltipType mapTooltipType = (MapTooltipType)(tooltipContext >> 24);
        uint           objectIndex    = tooltipContext & 0xFFFFFF;
        
        if (mapTooltipType != MapTooltipType.BattleCharaMarker)
        {  
            return ContextTooltipHandleHook.OriginalDisposeSafe(agentMap, tooltipString, tooltipContext);
        }
        
        // In the vanilla code the object index is used like this:
        // 'agentMap->UIModuleInterface->GetUI3DModule()->MemberInfoPointers[(int)objectIndex].Value->BattleChara->GetName()'
        // Well... it actually calls the vtable function on it, but it corresponds to this.
        
        PetServices.PetLog.DevLogVerbose($"ContextTooltipHandleDetour: [TooltipType:{mapTooltipType}], [ObjectIndex: {objectIndex}]");
        
        _passGetName    = true;
        _handledGetName = false;
        
        bool returner = ContextTooltipHandleHook.OriginalDisposeSafe(agentMap, tooltipString, tooltipContext);

        HandleTooltipRename(tooltipString);
        
        _handledGetName = false;
        _passGetName    = false;
        
        return returner;
    }
    
    private void MapUpdate(AddonEvent type, AddonArgs _)
    {
        _selectedPet = null;
        
        if (type == AddonEvent.PreUpdate)
        {
            _passContextTooltipHandle = true;
        }
        else if (type == AddonEvent.PostUpdate)
        {
            _passContextTooltipHandle = false;
            _passGetName              = false;
        }
    }
    
    private void HandleTooltipRename(Utf8String* tooltipString)
    {
        if (!_handledGetName)
        {
            return;
        }
        
        if (_selectedPet == null)
        {
            return;
        }
        
        if (!_selectedPet.IsActive)
        {
            return;
        }
        
        if (_selectedPet.Owner == null)
        {
            return;
        }
        
        if (_selectedPet.PetData == null)
        {
            return;
        }
        
        string? customName = _selectedPet.Owner.GetCustomName(_selectedPet.SkeletonId);
        
        if (customName.IsNullOrWhitespace())
        {
            return;
        }
        
        using Utf8String           editableString   = new Utf8String();
        
        editableString.Copy(tooltipString); 
        
        SeString                   editableSeString = SeString.Parse(editableString.AsReadOnlySeString());
        Configuration.ColourConfig colourConfig     = PetServices.Configuration.ShowOnTooltipColour;
        
        if (!PetServices.StringHelper.ReplaceSeString(colourConfig, ref editableSeString, _selectedPet.SkeletonId, _selectedPet.PetData.Singular, _selectedPet.Owner))
        {
            return;
        }
        
        tooltipString->SetString(editableSeString.EncodeWithNullTerminator());
    }
}
