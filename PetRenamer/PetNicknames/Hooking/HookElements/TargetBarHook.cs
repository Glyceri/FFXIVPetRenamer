using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TargetBarHook : QuickHookableElement
{
    private readonly List<ITextHook> TextHooks = [];

    public TargetBarHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) 
        : base(services, petServices, userList, dirtyListener) 
    {
        PetServices.TargetManager.RegisterTargetChangedListener(OnTargetChanged);
    }

    public override void Init()
    {
        TextHooks.Add(Hook<TargetTextHook>    ("_TargetInfo",             [16],   Allowed,         allowColours: true,  isSoft: false).RegsterTarget(Target));
        TextHooks.Add(Hook<TargetTextHook>    ("_TargetInfo",             [7],    Allowed,         allowColours: true,  isSoft: false).RegsterTarget(TargetOfTarget));
        TextHooks.Add(Hook<TargetTextHook>    ("_TargetInfoMainTarget",   [10],   Allowed,         allowColours: true,  isSoft: false).RegsterTarget(Target));
        TextHooks.Add(Hook<TargetTextHook>    ("_TargetInfoMainTarget",   [7],    Allowed,         allowColours: true,  isSoft: false).RegsterTarget(TargetOfTarget));
        TextHooks.Add(Hook<TargetTextHook>    ("_FocusTargetInfo",        [10],   Allowed,         allowColours: true,  isSoft: false).RegsterTarget(FocusTarget));

        TextHooks.Add(Hook<TargetCastBarHook> ("_TargetInfo",             [12],   AllowedCastbar,  allowColours: false, isSoft: true ).RegisterTarget(Target));
        TextHooks.Add(Hook<TargetCastBarHook> ("_TargetInfoCastBar",      [4],    AllowedCastbar,  allowColours: false, isSoft: true ).RegisterTarget(Target));
        TextHooks.Add(Hook<TargetCastBarHook> ("_FocusTargetInfo",        [5],    AllowedCastbar,  allowColours: false, isSoft: true ).RegisterTarget(FocusTarget));

        _ = Hook<CastBarHook>("_CastBar", [4], AllowedCastbar, allowColours: false, isSoft: true);

        _ = Hook<NotebookHook>("MinionNoteBook",    [67],   AllowedNotebook, allowColours: false, isSoft: false);
        _ = Hook<NotebookHook>("MJIMinionNoteBook", [65],   AllowedNotebook, allowColours: false, isSoft: false);
        _ = Hook<NotebookHook>("LovmPaletteEdit",   [48],   AllowedNotebook, allowColours: false, isSoft: false);
        _ = Hook<NotebookHook>("LovmActionDetail",  [4],    AllowedNotebook, allowColours: false, isSoft: false);
        _ = Hook<NotebookHook>("YKWNote",           [28],   AllowedNotebook, allowColours: false, isSoft: false);
    }

    [Conditional("DEBUG")]
    private void LogTarget(IPettableEntity? entity, [CallerMemberName] string callSource = "")
    {
        if (entity == null)
        {
            PetServices.PetLog.LogVerbose($"{callSource} just got the target: NULL.");

            return;
        }

        if (entity is IPettablePet pet)
        {
            PetServices.PetLog.LogVerbose($"{callSource} just got the target: {pet.Name}.");
        }
        else if (entity is IPettableUser user)
        {
            PetServices.PetLog.LogVerbose($"{callSource} just got the target: {user.Name}.");
        }
    }

    private IPettableEntity? FocusTarget()
    {
        IPettableEntity? returner = PetServices.TargetManager.FocusTarget;

        LogTarget(returner);

        return returner;
    }

    private IPettableEntity? TargetOfTarget()
    {
        IPettableEntity? returner = PetServices.TargetManager.TargetOfLeadingTarget;

        LogTarget(returner);

        return returner;
    }

    private IPettableEntity? Target()
    {
        IPettableEntity? returner = PetServices.TargetManager.LeadingTarget;

        LogTarget(returner);

        return returner;
    }

    private bool Allowed(PetSkeleton id)              
        => PetServices.Configuration.showOnTargetBars;

    private bool AllowedCastbar(PetSkeleton id)       
        => PetServices.Configuration.showOnCastbars;

    private bool AllowedNotebook(PetSkeleton id)      
        => PetServices.Configuration.showNamesInMinionBook;

    private void OnTargetChanged()
    {
        PetServices.PetLog.LogVerbose("Received target status changed. Hooks will be refreshed.");

        foreach (ITextHook textHook in TextHooks)
        {
            textHook.Refresh();
        }
    }

    protected override void OnQuickDispose()
    {
        PetServices.TargetManager.DeregisterTargetChangedListener(OnTargetChanged);
    }
}
