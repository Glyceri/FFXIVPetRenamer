using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using System;
using System.Collections.Generic;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class TooltipHook : QuickTextHookableElement
{
    [Signature("66 44 89 44 24 ?? 53 55", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<Delegates.AccurateShowTooltip> showTooltip = null!;

    public static string latestOutcome = string.Empty;

    internal override void OnQuickInit()
    {
        RegisterSoftHook("ActionDetail", 5, Allowed, -1, null!, (str) => latestOutcome = str);
        RegisterSoftHook("Tooltip", 2, Allowed, 3, () => TooltipHelper.getNextUser);

        showTooltip?.Enable();
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
    }

    unsafe int ShowTooltipDetour(AtkUnitBase* tooltip, byte a2, uint a3, IntPtr a4, IntPtr a5, IntPtr a6, char a7, char a8)
    {
        TooltipHelper.handleAsMap = false;
        if (TooltipHelper.lastWasMap)
        {
            TooltipHelper.handleAsMap = true;
            TooltipHelper.lastWasMap = false;
        }
        return showTooltip!.Original(tooltip, a2, a3, a4, a5, a6, a7, a8);
    }
}

public unsafe static class TooltipHelper
{
    public static List<PartyListInfo> partyListInfos = new List<PartyListInfo>();

    public static bool lastWasMap = false;
    public static bool handleAsMap = false;
    public static PettableUser nextUser = null!;

    public static PettableUser getNextUser
    {
        get 
        { 
            if (handleAsMap) return nextUser;
            return PluginLink.PettableUserHandler.LocalUser()!; 
        }
    }
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
