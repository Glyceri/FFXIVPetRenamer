using Dalamud.Game.Gui;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkTooltipManager;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class ActionTooltipHook : QuickHookableElement, IActionTooltipHook
{
    const int MinionActionKind = 34;

    readonly ActionTooltipTextHook tooltipHook = null!;
    readonly ActionTooltipTextHook actionTooltipHook = null!;

    public uint LastActionID { get; private set; } = 0;
    
    unsafe delegate void ShowTooltipDelegate(AtkTooltipManager* thisPtr, AtkTooltipType type, ushort parentId, AtkResNode* targetNode, AtkTooltipArgs* tooltipArgs, delegate* unmanaged[Stdcall]<float*, float*, void*> unkDelegate, bool unk7, bool unk8);

    [Signature("E8 ?? ?? ?? ?? 33 D2 EB 02", DetourName = nameof(OnShowTooltipDetur))]
    Hook<ShowTooltipDelegate>? showTooltipHook = null;

    public ActionTooltipHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) : base(services, petServices, userList, dirtyListener)
    {
        tooltipHook = Hook<ActionTooltipTextHook>("Tooltip", [2], Allowed, false, true);
        tooltipHook.Register(3);

        actionTooltipHook = Hook<ActionTooltipTextHook>("ActionDetail", [5], Allowed, false, true);
        actionTooltipHook.Register();
    }

    public override void Init()
    {
        DalamudServices.GameGui.HoveredActionChanged += OnHoveredActionChanged;
        showTooltipHook?.Enable();
    }

    bool Allowed(int id) => PetServices.Configuration.showOnTooltip;

    unsafe void OnShowTooltipDetur(AtkTooltipManager* thisPtr, AtkTooltipType type, ushort parentId, AtkResNode* targetNode, AtkTooltipArgs* tooltipArgs, delegate* unmanaged[Stdcall]<float*, float*, void*> unkDelegate, bool unk7, bool unk8)
    {
        tooltipHook.SetPetSheetData(null);
        actionTooltipHook.SetPetSheetData(null);

        showTooltipHook!.Original(thisPtr, type, parentId, targetNode, tooltipArgs, unkDelegate, unk7, unk8);
    }

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
        showTooltipHook?.Dispose();
        DalamudServices.GameGui.HoveredActionChanged -= OnHoveredActionChanged;
    }
}
