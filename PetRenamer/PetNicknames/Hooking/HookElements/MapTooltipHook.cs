﻿using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Linq;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class MapTooltipHook : QuickHookableElement, IMapTooltipHook
{
    ushort lastId = ushort.MaxValue;

    // This one gets set in the 2nd constructor
    readonly MapTooltipTextHook tooltipHookMap = null!;
    readonly ITooltipHookHelper TooltipHook;

    readonly string[] allowedTooltipAddonsMap = [
        "AreaMap",
        "_NaviMap"
    ];

    public MapTooltipHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener, ITooltipHookHelper tooltipHook) : base(services, petServices, userList, dirtyListener)
    {
        tooltipHookMap = Hook<MapTooltipTextHook>("Tooltip", [2], Allowed, false, false);
        tooltipHookMap.Register(3);

        TooltipHook = tooltipHook;
    }

    public override void Init()
    {
        TooltipHook.RegisterCallback(ShowTooltipDetour);
    }

    bool Allowed(int id) => PetServices.Configuration.showOnTooltip;

    void ShowTooltipDetour(IntPtr tooltip, byte tooltipType, ushort addonID, IntPtr a4, IntPtr a5, IntPtr a6, ushort a7, ushort a8)
    {
        if (addonID == lastId) return;
        lastId = addonID;

        AtkUnitBase* hoveredOverAddon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonById(addonID);
        if (hoveredOverAddon == null) return;
        if (!hoveredOverAddon->IsFullyLoaded()) return;

        string addonName = hoveredOverAddon->NameString;
        bool validAddonMap = allowedTooltipAddonsMap.Contains(addonName);

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
