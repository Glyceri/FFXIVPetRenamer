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
        Hook<TargetTextHook>    ("_TargetInfo",             [16],   Allowed,         allowColours: true,  isSoft: false).RegsterTarget(Target);
        Hook<TargetTextHook>    ("_TargetInfo",             [7],    Allowed,         allowColours: true,  isSoft: false).RegsterTarget(TargetOfTarget);
        Hook<TargetTextHook>    ("_TargetInfoMainTarget",   [10],   Allowed,         allowColours: true,  isSoft: false).RegsterTarget(Target);
        Hook<TargetTextHook>    ("_TargetInfoMainTarget",   [7],    Allowed,         allowColours: true,  isSoft: false).RegsterTarget(TargetOfTarget);
        Hook<TargetTextHook>    ("_FocusTargetInfo",        [10],   Allowed,         allowColours: true,  isSoft: false).RegsterTarget(FocusTarget);

        Hook<CastBarHook>       ("_CastBar",                [4],    AllowedCastbar,  allowColours: false, isSoft: true );

        Hook<TargetCastBarHook> ("_TargetInfo",             [12],   AllowedCastbar,  allowColours: false, isSoft: true ).RegisterTarget(Target);
        Hook<TargetCastBarHook> ("_TargetInfoCastBar",      [4],    AllowedCastbar,  allowColours: false, isSoft: true ).RegisterTarget(Target);
        Hook<TargetCastBarHook> ("_FocusTargetInfo",        [5],    AllowedCastbar,  allowColours: false, isSoft: true ).RegisterTarget(FocusTarget);

        Hook<NotebookHook>      ("MinionNoteBook",          [67],   AllowedNotebook, allowColours: false, isSoft: false);
        Hook<NotebookHook>      ("MJIMinionNoteBook",       [65],   AllowedNotebook, allowColours: false, isSoft: false);
        Hook<NotebookHook>      ("LovmPaletteEdit",         [48],   AllowedNotebook, allowColours: false, isSoft: false);
        Hook<NotebookHook>      ("LovmActionDetail",        [4],    AllowedNotebook, allowColours: false, isSoft: false);
        Hook<NotebookHook>      ("YKWNote",                 [28],   AllowedNotebook, allowColours: false, isSoft: false);
    }

    private IPettableEntity? FocusTarget()    => PetServices.TargetManager.FocusTarget;
    private IPettableEntity? TargetOfTarget() => PetServices.TargetManager.TargetOfTarget;
    private IPettableEntity? Target()         => PetServices.TargetManager.Target;

    private bool Allowed(int id)              => PetServices.Configuration.showOnTargetBars;
    private bool AllowedCastbar(int id)       => PetServices.Configuration.showOnCastbars;
    private bool AllowedNotebook(int id)      => PetServices.Configuration.showNamesInMinionBook;
}
