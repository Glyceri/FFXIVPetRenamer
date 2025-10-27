using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Linq;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkTooltipManager;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class MapTooltipHook : QuickHookableElement, IMapTooltipHook
{
    private ushort lastId = ushort.MaxValue;

    // This one gets set in the 2nd constructor
    private readonly MapTooltipTextHook tooltipHookMap = null!;
    private readonly ITooltipHookHelper TooltipHook;

    private readonly string[] allowedTooltipAddonsMap = 
    [
        "AreaMap",
        "_NaviMap"
    ];

    public MapTooltipHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener, ITooltipHookHelper tooltipHook) 
        : base(services, petServices, userList, dirtyListener)
    {
        tooltipHookMap = Hook<MapTooltipTextHook>("Tooltip", [2], Allowed, false, false);
        tooltipHookMap.Register(3);

        TooltipHook = tooltipHook;
    }

    public override void Init()
    {
        TooltipHook.RegisterCallback(ShowTooltipDetour);
    }

    private bool Allowed(PetSkeleton id) 
        => PetServices.Configuration.showOnTooltip;

    private void ShowTooltipDetour(nint tooltip, AtkTooltipType tooltipType, ushort addonID, nint a4, nint a5, nint a6, bool a7, bool a8)
    {
        if (addonID == lastId)
        {
            return;
        }

        lastId = addonID;

        AtkUnitBase* hoveredOverAddon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonById(addonID);

        if (hoveredOverAddon == null)
        {
            return;
        }

        if (!hoveredOverAddon->IsFullyLoaded())
        {
            return;
        }

        string addonName     = hoveredOverAddon->NameString;
        bool   validAddonMap = allowedTooltipAddonsMap.Contains(addonName);

        tooltipHookMap.SetBlockedState(!validAddonMap);
    }

    protected override void OnQuickDispose()
    {
        TooltipHook.DeregisterCallback(ShowTooltipDetour);
    }

    public void OverridePet(IPettablePet? pet)
    {
        tooltipHookMap.SetPet(pet);
    }
}
