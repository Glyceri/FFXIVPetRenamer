using Dalamud.Game.ClientState.Objects.Types;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TargetBarHook : QuickHookableElement
{
    public TargetBarHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) : base(services, petServices, userList, dirtyListener) { }

    public override void Init()
    {
        Hook<TargetTextHook>("_TargetInfo", [16], Allowed, allowColours: true).RegsterTarget(TargetObject);
        Hook<TargetTextHook>("_TargetInfo", [7], Allowed, allowColours: true).RegsterTarget(TargetOfTarget);

        Hook<TargetTextHook>("_TargetInfoMainTarget", [10], Allowed, allowColours: true).RegsterTarget(TargetObject);
        Hook<TargetTextHook>("_TargetInfoMainTarget", [7], Allowed, allowColours: true).RegsterTarget(TargetOfTarget);

        Hook<TargetTextHook>("_FocusTargetInfo", [10], Allowed, allowColours: true).RegsterTarget(FocusTargetPet);

        Hook<CastBarHook>("_CastBar", [4], AllowedCastbar, allowColours: false, isSoft: true);
        Hook<TargetCastBarHook>("_TargetInfo", [12], AllowedCastbar, allowColours: false, isSoft: true).RegsterTarget(() => UserList.GetUser(Target?.Address ?? nint.Zero));
        Hook<TargetCastBarHook>("_TargetInfoCastBar", [4], AllowedCastbar, allowColours: false, isSoft: true).RegsterTarget(() => UserList.GetUser(Target?.Address ?? nint.Zero));
        Hook<TargetCastBarHook>("_FocusTargetInfo", [5], AllowedCastbar, allowColours: false, isSoft: true).RegsterTarget(() => UserList.GetUser(FocusTarget?.Address ?? nint.Zero));

        Hook<NotebookHook>("MinionNoteBook", [67], AllowedNotebook, allowColours: false);
        Hook<NotebookHook>("MJIMinionNoteBook", [65], AllowedNotebook, allowColours: false);
        Hook<NotebookHook>("LovmPaletteEdit", [48], AllowedNotebook, allowColours: false);
        Hook<NotebookHook>("LovmActionDetail", [4], AllowedNotebook, allowColours: false);
        Hook<NotebookHook>("YKWNote", [28], AllowedNotebook, allowColours: false);
    }

    IPettablePet? FocusTargetPet() => UserList.GetPet(FocusTarget?.Address ?? nint.Zero);
    IPettablePet? TargetOfTarget() => UserList.GetPet(Target?.TargetObject?.Address ?? nint.Zero);
    IPettablePet? TargetObject() => UserList.GetPet(Target?.Address ?? nint.Zero);

    IGameObject? FocusTarget => DalamudServices.TargetManager.FocusTarget;
    IGameObject? Target => DalamudServices.TargetManager.SoftTarget ?? DalamudServices.TargetManager.Target;

    bool Allowed(int id) => PetServices.Configuration.showOnTargetBars;
    bool AllowedCastbar(int id) => PetServices.Configuration.showOnCastbars;
    bool AllowedNotebook(int id) => PetServices.Configuration.showNamesInMinionBook;
}
