using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Logging;
using System;
using System.Collections.Generic;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class TooltipHook : QuickTextHookableElement
{
    [Signature("66 44 89 44 24 ?? 53 55", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<Delegates.AccurateShowTooltip> showTooltip = null!;

    // Hook from: https://github.com/Kouzukii/ffxiv-whichpatchwasthat/blob/main/WhichPatchWasThat/Hooks.cs
    // Actually its now from: https://github.com/Caraxi/SimpleTweaksPlugin/blob/b390e0686c1f8b30e163306dcc3420390b67f340/Tweaks/Tooltips/AdditionalItemInfo.cs#L21
    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 41 54 41 55 41 56 41 57 48 83 EC 20 4C 8B AA ?? ?? ?? ??", DetourName = nameof(ItemDetailOnUpdateDetour))]
    Hook<Delegates.AddonOnRequestedUpdate>? ItemDetailOnUpdateHook { get; init; }

    public static string latestOutcome = string.Empty;

    internal override void OnQuickInit()
    {
        RegisterHook("ActionDetail", 5, Allowed, -1, null!, (str) => latestOutcome = str);
        RegisterHook("Tooltip", 2, Allowed, 3, () => TooltipHelper.nextUser);

        showTooltip?.Enable();
        ItemDetailOnUpdateHook?.Enable();
    }

    private unsafe IntPtr ItemDetailOnUpdateDetour(IntPtr a1, IntPtr a2, IntPtr a3)
    {
        TooltipHelper.handleAsItem = true;
        return ItemDetailOnUpdateHook!.Original(a1, a2, a3);
    }

    bool Allowed(int id)
    {
        if (!PluginLink.Configuration.displayCustomNames) return false;
        if (id >= 0 && !PluginLink.Configuration.allowTooltipsOnMinions) return false;
        if (id <= -2 && !PluginLink.Configuration.allowTooltipsBattlePets) return false;
        return true;
    }

    internal override void OnQuickDispose()
    {
        showTooltip?.Dispose();
        ItemDetailOnUpdateHook?.Dispose();
    }

    IntPtr lastTooltip;

    unsafe int ShowTooltipDetour(AtkUnitBase* tooltip, byte a2, uint a3, IntPtr a4, IntPtr a5, IntPtr a6, char a7, char a8)
    {
        if (!TooltipHelper.lastTooltipWasMap) TooltipHelper.nextUser = null!;
        TooltipHelper.lastTooltipWasMap = false;
        if (lastTooltip != a4)
        {
            lastTooltip = a4;
            TooltipHelper.handleAsItem = false;
        }
        return showTooltip!.Original(tooltip, a2, a3, a4, a5, a6, a7, a8);
    }
}

public unsafe static class TooltipHelper
{
    static PettableUser _nextUser = null!;
    public static PettableUser nextUser
    {
        get => _nextUser ?? PluginLink.PettableUserHandler.GetUser(PluginHandlers.ClientState.LocalPlayer!.Address);
        set => _nextUser = value;
    }

    public static bool lastTooltipWasMap = false;
    public static void SetNextUp(PettableUser user) => _nextUser = user;

    public static List<PartyListInfo> partyListInfos = new List<PartyListInfo>();
    public static bool handleAsItem = false;
}

public class PartyListInfo
{
    public string UserName = string.Empty;
    public bool hasPet;
    public bool hasChocobo;

    public PartyListInfo(string userName, bool hasPet, bool hasChocobo)
    {
        UserName = userName;
        this.hasPet = hasPet;
        this.hasChocobo = hasChocobo;
    }

    public new string ToString() => $"UserName: {UserName}, Has Pet: {hasPet}, Has Chocobo: {hasChocobo}";
}
