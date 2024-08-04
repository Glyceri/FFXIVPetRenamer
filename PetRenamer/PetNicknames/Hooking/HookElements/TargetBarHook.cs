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
        Hook<TargetTextHook>("_TargetInfo", [16], Allowed).RegsterTarget(TargetObject);
        Hook<TargetTextHook>("_TargetInfo", [7], Allowed).RegsterTarget(TargetOfTarget);

        Hook<TargetTextHook>("_TargetInfoMainTarget", [10], Allowed).RegsterTarget(TargetObject);
        Hook<TargetTextHook>("_TargetInfoMainTarget", [7], Allowed).RegsterTarget(TargetOfTarget);

        Hook<TargetTextHook>("_FocusTargetInfo", [10], Allowed).RegsterTarget(FocusTargetPet);

        Hook<CastBarHook>("_CastBar", [4], AllowedCastbar, true);
        Hook<TargetCastBarHook>("_TargetInfo", [12], AllowedCastbar, true).RegsterTarget(() => UserList.GetUser(Target?.GameObjectId ?? 0));
        Hook<TargetCastBarHook>("_TargetInfoCastBar", [4], AllowedCastbar, true).RegsterTarget(() => UserList.GetUser(Target?.GameObjectId ?? 0));
        Hook<TargetCastBarHook>("_FocusTargetInfo", [5], AllowedCastbar, true).RegsterTarget(() => UserList.GetUser(FocusTarget?.GameObjectId ?? 0));

        Hook<NotebookHook>("MinionNoteBook", [67], AllowedNotebook);
        Hook<NotebookHook>("LovmPaletteEdit", [48], AllowedNotebook);
        Hook<NotebookHook>("LovmActionDetail", [4], AllowedNotebook);
        Hook<NotebookHook>("YKWNote", [28], AllowedNotebook);
    }

    IPettablePet? FocusTargetPet() => UserList.GetPet(FocusTarget?.GameObjectId ?? 0);
    IPettablePet? TargetOfTarget() => UserList.GetPet(Target?.TargetObject?.GameObjectId ?? 0);
    IPettablePet? TargetObject() => UserList.GetPet(Target?.GameObjectId ?? 0);

    IGameObject? FocusTarget => DalamudServices.TargetManager.FocusTarget;
    IGameObject? Target => DalamudServices.TargetManager.SoftTarget ?? DalamudServices.TargetManager.Target;

    bool Allowed(int id) => PetServices.Configuration.showOnTargetBars;
    bool AllowedCastbar(int id) => PetServices.Configuration.showOnCastbars;
    bool AllowedNotebook(int id) => PetServices.Configuration.showNamesInMinionBook;
}
