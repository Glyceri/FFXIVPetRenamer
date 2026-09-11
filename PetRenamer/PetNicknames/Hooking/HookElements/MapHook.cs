using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using PetRenamer.PetNicknames.Hooking.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class MapHook : HookableElement
{
    private const uint MinimapIconHoverEvent = 1;
    private const uint AreaMapIconHoverEvent = 9;
    
    private readonly Hook<AtkValue.Delegates.GetInt>        GetInt;
    private readonly Hook<AgentMap.Delegates.CreateTooltip> ContextTooltipHandleHook;
    private readonly Hook<BattleChara.Delegates.GetName>    GetNameHook;
    
    private uint          _expectedEvent = 0;
    private IPettablePet? _selectedPet   = null;
    
    public MapHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
    {
        GetInt                   = DalamudServices.Hooking.HookFromAddress<AtkValue.Delegates.GetInt>((nint)AtkValue.MemberFunctionPointers.GetInt, GetIntDetour);
        ContextTooltipHandleHook = DalamudServices.Hooking.HookFromAddress<AgentMap.Delegates.CreateTooltip>((nint)AgentMap.MemberFunctionPointers.CreateTooltip, ContextTooltipHandleDetour);
        GetNameHook              = DalamudServices.Hooking.HookFromAddress<BattleChara.Delegates.GetName>((nint)BattleChara.StaticVirtualTablePointer->GetName, GetNameDetour);
    }

    public override void Init()
    { 
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate,  "AreaMap",  AreaMapUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "AreaMap",  AreaMapUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate,  "_NaviMap", NaviMapUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "AreaMap",  NaviMapUpdate);
    }
    
    protected override void OnDispose()
    {
        GetNameHook.Dispose();
        ContextTooltipHandleHook.Dispose();
        GetInt.Dispose();
        
        DalamudServices.AddonLifecycle.UnregisterListener(AreaMapUpdate);
        DalamudServices.AddonLifecycle.UnregisterListener(NaviMapUpdate);
    }
    
    private CStringPointer GetNameDetour(BattleChara* gameObject)
    {
        _selectedPet = PetServices.UserList.GetPet((nint)gameObject);
        
        return GetNameHook.OriginalDisposeSafe(gameObject);
    }
    
    private bool ContextTooltipHandleDetour(AgentMap* agentMap, Utf8String* tooltipString, uint tooltipContext)
    {
        MapTooltipType mapTooltipType = (MapTooltipType)(tooltipContext >> 24);
        
        uint objectIndex = tooltipContext & 0xFFFFFF;
        
        // In the vanilla code the object index is used like this:
        // 'agentMap->UIModuleInterface->GetUI3DModule()->MemberInfoPointers[(int)objectIndex].Value->BattleChara->GetName()'
        // Well... it actually calls the vtable function on it, but it corresponds to this.
        
        PetServices.PetLog.DevLogVerbose($"ContextTooltipHandleDetour: [TooltipType:{mapTooltipType}], [ObjectIndex: {objectIndex}]");
        
        if ((mapTooltipType == MapTooltipType.BattleCharaMarker))
        {
            GetNameHook.Enable();
        }
        
        bool returner = ContextTooltipHandleHook.OriginalDisposeSafe(agentMap, tooltipString, tooltipContext);

        HandleTooltipRename(tooltipString);
        
        GetNameHook.Disable();
        
        return returner;
    }
    
    private int GetIntDetour(AtkValue* atkValue)
    {
        ContextTooltipHandleHook.Disable();
        
        int returner = GetInt.Original(atkValue);
        
        if (returner != _expectedEvent)
        {
            return returner;
        }
        
        ContextTooltipHandleHook.Enable();
        
        return returner;
    }
    
    private void MapUpdate(AddonEvent type)
    {
        _selectedPet   = null;
        
        if (type == AddonEvent.PreUpdate)
        {
            GetInt.Enable();
        }
        else
        {
            GetInt.Disable();
            ContextTooltipHandleHook.Disable();
        }
    }
    
    private void AreaMapUpdate(AddonEvent type, AddonArgs _)
    {
        _expectedEvent = AreaMapIconHoverEvent;
        
        MapUpdate(type);
    }
    
    private void NaviMapUpdate(AddonEvent type, AddonArgs _)
    {
        _expectedEvent = MinimapIconHoverEvent;
        
        MapUpdate(type);
    }
    
    private void HandleTooltipRename(Utf8String* tooltipString)
    {
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
