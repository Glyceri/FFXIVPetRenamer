using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;
using Dalamud.Plugin.Services;
using PetRenamer.Core.PettableUserSystem;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal unsafe class TargetBarHooking : QuickTextHookableElement
{
    internal override void OnQuickInit()
    {
        RegisterHook("_TargetInfoMainTarget",   10, Display,  -1, TargetUser);
        RegisterHook("_TargetInfoMainTarget",   7,  Display,  -1, TargetOfTargetUser);
        RegisterHook("_FocusTargetInfo",        10, Display,  -1, FocusTargetUser);
        RegisterHook("_TargetInfoCastBar",      4,  Allowed,  -1, TargetUser);
        RegisterHook("_FocusTargetInfo",        5,  Allowed,  -1, FocusTargetUser);
    }

    internal override void OnUpdate(IFramework framework) => OnBaseUpdate(framework);

    PettableUser TargetUser() => PluginLink.PettableUserHandler.GetUser(RequestTarget()?.Address ?? nint.Zero);
    PettableUser TargetOfTargetUser() => PluginLink.PettableUserHandler.GetUser(RequestTarget()?.TargetObject?.Address ?? GetAlternativeTargetOfTarget());
    PettableUser FocusTargetUser() => PluginLink.PettableUserHandler.GetUser(RequestFocusTarget()?.Address ?? nint.Zero);

    nint lastNint;
    ulong lastID;

    nint GetAlternativeTargetOfTarget()
    {
        ulong targetID = RequestTarget()?.TargetObjectId ?? 0;
        if (targetID == lastID) return lastNint;
        if (targetID == 0) return nint.Zero;
        string targetString = targetID.ToString("X");
        bool isCompanion = targetString.StartsWith("4");
        if (!isCompanion) return nint.Zero;
        targetString = targetString.TrimStart('4');
        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
            if (user.ObjectID.ToString("X") == targetString)
            {
                lastID = targetID;
                return lastNint = user.Minion.Pet;
            }
        return nint.Zero;
    }

    DGameObject RequestFocusTarget() => PluginHandlers.TargetManager.FocusTarget!;
    DGameObject RequestTarget() => PluginHandlers.TargetManager.SoftTarget! ?? PluginHandlers.TargetManager.Target!;

    bool Display(int id) => PluginLink.Configuration.displayCustomNames;
    bool Allowed(int id) => id < -1 && PluginLink.Configuration.allowCastBarPet && PluginLink.Configuration.displayCustomNames;
}
