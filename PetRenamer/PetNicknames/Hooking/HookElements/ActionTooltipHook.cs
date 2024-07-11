using System;
using Dalamud.Game.Gui;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class ActionTooltipHook : QuickHookableElement
{
    const int MinionActionKind = 34;

    readonly ActionTooltipTextHook tooltipHook = null!;
    readonly ActionTooltipTextHook actionTooltipHook = null!;

    public ActionTooltipHook(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, petServices, userList)
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
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromAction(e.ActionID);
        IPetSheetData? softPetSheetData = MakeSoft(petData);

        tooltipHook.SetPetSheetData(softPetSheetData);
        actionTooltipHook.SetPetSheetData(softPetSheetData);
    }

    IPetSheetData? MakeSoft(IPetSheetData? petData)
    {
        if (petData == null) return null;

        if (petData.Model >= -1) return petData;

        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return petData;

        int index = Array.IndexOf(PluginConstants.BaseSkeletons, petData.Model);
        if (index == -1) return petData;

        return PetServices.PetSheets.GetFromSoftIndex(in localUser, in petData, index);
    }

    public override void Dispose()
    {
        DalamudServices.GameGui.HoveredActionChanged -= OnHoveredActionChanged;
    }

}
