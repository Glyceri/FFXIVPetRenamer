using Dalamud.Game.Gui;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkTooltipManager;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class ActionTooltipHook : QuickHookableElement, IActionTooltipHook
{
    private readonly ActionTooltipTextHook tooltipHook = null!;
    private readonly ActionTooltipTextHook actionTooltipHook = null!;
    private readonly ITooltipHookHelper TooltipHook;

    public uint LastActionID { get; private set; } = 0;

    public ActionTooltipHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener, ITooltipHookHelper tooltipHookHelper) : base(services, petServices, userList, dirtyListener)
    {
        TooltipHook = tooltipHookHelper;

        tooltipHook = Hook<ActionTooltipTextHook>("Tooltip", [2], Allowed, false, true);
        tooltipHook.Register(3);

        actionTooltipHook = Hook<ActionTooltipTextHook>("ActionDetail", [5], Allowed, false, true);
        actionTooltipHook.Register();
    }

    public override void Init()
    {
        TooltipHook?.RegisterCallback(OnShowTooltipDetour);

        DalamudServices.GameGui.HoveredActionChanged += OnHoveredActionChanged;
    }

    private bool Allowed(PetSkeleton id) 
        => PetServices.Configuration.showOnTooltip;

    private void OnShowTooltipDetour(nint tooltip, AtkTooltipType tooltipType, ushort addonID, nint a4, nint a5, nint a6, bool a7, bool a8)
    {
        tooltipHook.SetPetSheetData(null);
        actionTooltipHook.SetPetSheetData(null);
    }

    private void OnHoveredActionChanged(object? sender, HoveredAction e)
    {
        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return;

        uint action = e.ActionID;
        if (action != 0)
        {
            LastActionID = action;
        }

        IPetSheetData? petData = PetServices.PetSheets.GetPetFromAction(action, in localUser, true);

        tooltipHook.SetPetSheetData(petData);
        actionTooltipHook.SetPetSheetData(petData);
    }

    protected override void OnQuickDispose()
    {
        TooltipHook?.DeregisterCallback(OnShowTooltipDetour);

        DalamudServices.GameGui.HoveredActionChanged -= OnHoveredActionChanged;
    }
}
