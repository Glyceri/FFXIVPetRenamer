using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Linq;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TooltipHook : QuickHookableElement
{
    IPettableUser? overridenUser = null;
    IPettableUser? lastUser = null;

    ushort lastId = ushort.MaxValue;

    bool firstTime = true;

    // This one gets set in the 2nd constructor
    TooltipTextHook tooltipHook = null!;
    TooltipTextHook tooltipHookMap = null!;

    // Going by addon name and not ID (I know I hate it too)
    // Because addons can share the same ID. I dont want potential issues
    readonly string[] allowedTooltipAddons =
            [
                "_ActionBar",
                "_ActionBar01",
                "_ActionBar02",
                "_ActionBar03",
                "_ActionBar04",
                "_ActionBar05",
                "_ActionBar06",
                "_ActionBar07",
                "_ActionBar08",
                "_ActionBar09",
                "MinionNoteBook",
                "ActionMenu",
                "YKWNote",
                "ActionMenuReplaceList",
                "ActionMenuActionSetting",
                "LovmPaletteEdit",
            ];

    readonly string[] allowedTooltipAddonsMap = [
        "AreaMap",
        "_NaviMap"
    ];

    // The dalamud TooltipType enum is NOT accurate it seems
    public delegate int AccurateShowTooltip(AtkUnitBase* tooltip, byte tooltipType, ushort addonID, AtkUnitBase* a4, IntPtr a5, IntPtr a6, ushort a7, ushort a8);

    [Signature("E8 ?? ?? ?? ?? B8 5E 01 00 00 ", DetourName = nameof(ShowTooltipDetour))]
    readonly Hook<AccurateShowTooltip> showTooltip = null!;

    public TooltipHook(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, petServices, userList)
    {
        
    }

    public override void Init()
    {
        tooltipHook = Hook<TooltipTextHook>("Tooltip", [2], Allowed, true);
        tooltipHook.Register(3);

        tooltipHookMap = Hook<TooltipTextHook>("Tooltip", [2], Allowed, false);
        tooltipHookMap.Register(3);
        Hook<SimpleTextHook>("ActionDetail", [5], Allowed, true);

        showTooltip?.Enable();

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;
        OverrideUser(user);
    }

    bool Allowed(int id) => true;

    unsafe int ShowTooltipDetour(AtkUnitBase* tooltip, byte tooltipType, ushort addonID, AtkUnitBase* a4, IntPtr a5, IntPtr a6, ushort a7, ushort a8)
    {
        HandleFirstTime();

        if (addonID == lastId && overridenUser == lastUser) return showTooltip!.Original(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);
        lastId = addonID;

        tooltipHook.SetUser(overridenUser);
        tooltipHookMap.SetUser(overridenUser);

        AtkUnitBase* hoveredOverAddon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonById(addonID);
        if (hoveredOverAddon == null) return showTooltip!.Original(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);

        string addonName = hoveredOverAddon->NameString;
        bool validAddon = allowedTooltipAddons.Contains(addonName);

        tooltipHook.SetBlockedState(!validAddon);

        bool validAddonMap = allowedTooltipAddonsMap.Contains(addonName);

        tooltipHookMap.SetBlockedState(!validAddonMap);

        overridenUser = UserList.LocalPlayer;
        lastUser = overridenUser;

        return showTooltip!.Original(tooltip, tooltipType, addonID, a4, a5, a6, a7, a8);
    }

    void HandleFirstTime()
    {
        if (!firstTime) return;

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;

        OverrideUser(user);

        firstTime = false;
    }

    public override void Dispose()
    {
        showTooltip?.Dispose();
    }

    public void OverrideUser(IPettableUser user)
    {
        overridenUser = user;
        tooltipHook.SetUser(user);
        tooltipHookMap.SetUser(user);
    }
}
