using Dalamud.Game.Gui;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class ActionTooltipHook : QuickHookableElement, IActionTooltipHook
{
    const int MinionActionKind = 34;

    readonly ActionTooltipTextHook tooltipHook = null!;
    readonly ActionTooltipTextHook actionTooltipHook = null!;

    public uint LastActionID { get; private set; } = 0;

    public ActionTooltipHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) : base(services, petServices, userList, dirtyListener)
    {
        tooltipHook = Hook<ActionTooltipTextHook>("Tooltip", [2], Allowed, true);
        tooltipHook.Register(3);

        actionTooltipHook = Hook<ActionTooltipTextHook>("ActionDetail", [5], Allowed, true);
        actionTooltipHook.Register();
    }

    public override void Init()
    {
        DalamudServices.GameGui.HoveredActionChanged += OnHoveredActionChanged;
    }

    bool Allowed(int id) => true;

    void OnHoveredActionChanged(object? sender, HoveredAction e)
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
        DalamudServices.GameGui.HoveredActionChanged -= OnHoveredActionChanged;
    }
}
