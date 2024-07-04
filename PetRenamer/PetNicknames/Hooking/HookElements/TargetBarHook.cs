using Dalamud.Game.ClientState.Objects.Types;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TargetBarHook : QuickHookableElement
{
    public TargetBarHook(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, petServices, userList) { }

    public override void Init()
    {
        Hook<TargetTextHook>("_TargetInfo", [16], Allowed).RegsterTarget(TargetObject);
        Hook<TargetTextHook>("_TargetInfo", [7], Allowed).RegsterTarget(TargetOfTarget);

        Hook<TargetTextHook>("_TargetInfoMainTarget", [10], Allowed).RegsterTarget(TargetObject);
        Hook<TargetTextHook>("_TargetInfoMainTarget", [7], Allowed).RegsterTarget(TargetOfTarget);

        Hook<TargetTextHook>("_FocusTargetInfo", [10], Allowed).RegsterTarget(FocusTargetPet);
        Hook<TargetTextHook>("_FocusTargetInfo", [5], Allowed).RegsterTarget(FocusTargetPet);

        Hook<SimpleTextHook>("_TargetInfoCastBar", [4], Allowed, true);

        Hook<SimpleTextHook>("_CastBar", [4], Allowed, true);
        Hook<SimpleTextHook>("_TargetInfo", [12], Allowed, true);
    }

    public override void Dispose()
    {

    }

    IPettablePet? FocusTargetPet() => UserList.GetPet(FocusTarget?.GameObjectId ?? 0);
    IPettablePet? TargetOfTarget() => UserList.GetPet(Target?.TargetObject?.GameObjectId ?? 0);
    IPettablePet? TargetObject() => UserList.GetPet(Target?.GameObjectId ?? 0);

    IGameObject? FocusTarget => DalamudServices.TargetManager.FocusTarget;
    IGameObject? Target => DalamudServices.TargetManager.SoftTarget ?? DalamudServices.TargetManager.Target;

    bool Allowed(int id) => true;
}
