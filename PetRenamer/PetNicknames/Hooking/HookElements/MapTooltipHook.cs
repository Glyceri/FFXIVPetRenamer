﻿using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Linq;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class MapTooltipHook : QuickHookableElement
{
    ushort lastId = ushort.MaxValue;

    // This one gets set in the 2nd constructor
    readonly MapTooltipTextHook tooltipHookMap = null!;

    readonly string[] allowedTooltipAddonsMap = [
        "AreaMap",
        "_NaviMap"
    ];

    // The dalamud TooltipType enum is NOT accurate it seems
    public delegate int AccurateShowTooltip(AtkUnitBase* tooltip, byte tooltipType, ushort addonID, AtkUnitBase* a4, IntPtr a5, IntPtr a6, ushort a7, ushort a8);

    [Signature("E8 ?? ?? ?? ?? B8 5E 01 00 00 ", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<AccurateShowTooltip> showTooltip = null!;

    public MapTooltipHook(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, petServices, userList)
    {
        tooltipHookMap = Hook<MapTooltipTextHook>("Tooltip", [2], Allowed, false);
        tooltipHookMap.Register(3);
    }

    public override void Init()
    {
        showTooltip?.Enable();
    }

    bool Allowed(int id) => true;

    unsafe int ShowTooltipDetour(AtkUnitBase* tooltip, byte tooltipType, ushort addonID, AtkUnitBase* a4, IntPtr a5, IntPtr a6, ushort a7, ushort a8)
    {
        if (addonID == lastId) return showTooltip!.Original(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);
        lastId = addonID;

        AtkUnitBase* hoveredOverAddon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonById(addonID);
        if (hoveredOverAddon == null) return showTooltip!.Original(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);

        string addonName = hoveredOverAddon->NameString;
        bool validAddonMap = allowedTooltipAddonsMap.Contains(addonName);

        tooltipHookMap.SetBlockedState(!validAddonMap);

        return showTooltip!.Original(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);
    }

    public override void Dispose()
    {
        showTooltip?.Dispose();
    }

    public void OverrideUser(IPettableUser user)
    {
        tooltipHookMap.SetUser(user);
    }
}
